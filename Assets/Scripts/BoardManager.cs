using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static ChessRules;
using static ChessInfo;

// chess x = a-h, y = 1-8
public class BoardManager : MonoBehaviour
{
    public List<Piece> pieces;

    private float _screenWidth;
    private const float TOLERANCE = 0.01f;
    private float _xMax;
    private float _yMax;
    private float _xMin;
    private float _yMin;
    private float _dx = 1;
    private float _dy = 1;
    private PieceColor _turn = PieceColor.WHITE;
    private int _ticksSinceLastMove;
    public int ticksPerMove = 10;
    private bool _weAreLive;
    private ChessAI _ai;
    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;
    

    private void GetPieceGridBounds()
    {
        _xMax = _yMax = 0.77f;
        _yMin = _xMin = -0.77f;
        _dx = (_xMax - _xMin) / 7;
        _dy = (_yMax - _yMin) / 7;
        Debug.Log($"BOUNDS_UPDATE: {_xMin}=>{_xMax} dx={_dx}");
    }

    // ReSharper disable once InconsistentNaming
    private float cx2tx(int cx)
    {
        return _xMin + (cx - 1) * _dx;
    }
    
    // ReSharper disable once InconsistentNaming
    private float cy2ty(int cy)
    {
        return _yMin + (cy - 1) * _dy;
    }
    
    private Vector3 PieceTargetPos(Piece piece)
    {
        return new Vector3(cx2tx(piece.cx), cy2ty(piece.cy), 0);
    }
    
    private float DistanceToTarget(Piece piece)
    {
        return Vector3.Distance(piece.transform.localPosition, PieceTargetPos(piece));
    }
    
    private bool UpdatePieceLocations()
    {
        var moved = false;
        
        foreach( var piece in pieces.Where(piece => piece.Alive() && DistanceToTarget(piece) > TOLERANCE))
        {
            piece.transform.localPosition = Vector3.Lerp(piece.transform.localPosition, PieceTargetPos(piece), 0.25f);
            moved = true;
        }

        return moved;
    }

    private void DoAIBoardMove()
    {
        _ai ??= new ChessAISimple();
        var livePieces = pieces.Cast<IPieceData>().Where(piece => piece.Alive()).ToList();
        if (livePieces.Any(piece => piece.Color() == _turn))
        {
            var bestMove = _ai.BestMove(ref livePieces, _turn);
            if (bestMove != null && bestMove.NotZero())
            {
                var pieceToMove = pieces.FirstOrDefault(piece => piece.X() == bestMove.piece.X() && piece.Y() == bestMove.piece.Y());
                MoveOnePiece(ref livePieces, pieceToMove, bestMove.x, bestMove.y);
            }
        }
        else
        {
            Debug.Log($"No pieces!");
            Debug.Break();
        }

        EndTurn();
    }

    public void OnClick(string buttonName)
    {
        Debug.Log($"BoardManager.OnClick: {buttonName}");
        _ai = buttonName switch
        {
            "random" => new ChessAIRandom(),
            "simple" => new ChessAISimple(),
            "deep" => new ChessAIDeep(3,10),
            _ => _ai
        };
    }

    private string TopText()
    {
        var livePieces = pieces.Cast<IPieceData>().Where(piece => piece.Alive()).ToList();
        return new[] {PieceColor.WHITE, PieceColor.BLACK}.Aggregate("", (current, turn) => current + $"{turn,6}:{ChessAI.BoardScore(ref livePieces, turn),-6:G}");
    }
    
    private void UpdateText()
    {
        if (!_weAreLive) return;
        topText.text = TopText();
        bottomText.text = _ai.GetAIDescription();
    }

    private void EndTurn()
    {
        UpdatePieceLocations();
        UpdateText();
        _turn = OtherColor(_turn);
    }

    
    

    void FixedUpdate()
    {
        if (!_weAreLive) return;
        if (UpdatePieceLocations()) return;
        if (++_ticksSinceLastMove % ticksPerMove != 0) return;
        DoAIBoardMove();
        _ticksSinceLastMove = 0;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(_screenWidth - Screen.width) > TOLERANCE)
        {
            GetPieceGridBounds();
            _screenWidth = Screen.width;
        }
        
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            _weAreLive = true;
            DoAIBoardMove();
        }
    }
}