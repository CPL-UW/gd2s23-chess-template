using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ChessRules;
using static ChessInfo;

// chess x = a-h, y = 1-8
public class BoardManager : MonoBehaviour
{
    public List<Piece> pieces;
    // public List<Piece> p2Pieces;

    private float _screenWidth;
    private const float TOLERANCE = 0.01f;
    // private Rect _pieceBounds = Rect.zero;
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
    

    private void GetPieceGridBounds()
    {
        // Debug.Log($"parent.localScale: {pieces.ElementAt(0).transform.parent.localScale}; localPosition: {pieces.ElementAt(0).transform.localPosition}");
        
        _xMax = _yMax = 0.77f;
        _yMin = _xMin = -0.77f;
        // TODO pretty effing brittle!
        
        
        // foreach (var pos in pieces.Select(piece => piece.transform.localPosition))
        // {
        //     if (pos.x > _xMax) _xMax = pos.x;
        //     if (pos.x < _xMin) _xMin = pos.x;
        //     if (pos.y > _yMax) _yMax = pos.y;
        //     if (pos.y < _yMin) _yMin = pos.y;
        // }

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

    private void DoRandomBoardMove()
    {
        _ai ??= new ChessAIDeep();
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

    private void EndTurn()
    {
        UpdatePieceLocations();
        _turn = _turn switch
        {
            PieceColor.WHITE => PieceColor.BLACK,
            _ => PieceColor.WHITE
        };
    }

    
    

    void FixedUpdate()
    {
        if (!_weAreLive) return;
        if (UpdatePieceLocations()) return;
        if (++_ticksSinceLastMove % ticksPerMove != 0) return;
        DoRandomBoardMove();
        // CheckDuplicates();
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
            DoRandomBoardMove();
        }
    }
}