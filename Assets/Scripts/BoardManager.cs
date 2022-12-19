using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using static ChessRules;
using static ChessInfo;

// chess x = a-h, y = 1-8
public class BoardManager : MonoBehaviour
{
    public List<Piece> pieces;
    // public List<Piece> p2Pieces;

    private float _screenWidth;
    private const float TOLERANCE = 0.1f;
    // private Rect _pieceBounds = Rect.zero;
    private float _xMax;
    private float _yMax;
    private float _xMin;
    private float _yMin;
    private float _dx = 1;
    private float _dy = 1;
    private PieceColor _turn = PieceColor.WHITE;
    private int _ticksSinceLastMove = 0;
    public int ticksPerMove = 10;
    private bool _weAreLive = false;
    

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

    private float cx2tx(int cx)
    {
        return _xMin + (cx - 1) * _dx;
    }
    
    private float cy2ty(int cy)
    {
        return _yMin + (cy - 1) * _dy;
    }
    
    private void UpdatePieceLocations()
    {
        foreach (var piece in pieces)
        {
            if (piece.cx < 1 || piece.cy < 1 || piece.pieceState == PieceState.DEAD)
            {
                piece.RemoveSelf();
            }
            else
            {
                var x = cx2tx(piece.cx);
                var y = cy2ty(piece.cy);
                piece.transform.localPosition = new Vector3(x, y, 0);
            }
        }
    }

    private void DoRandomBoardMove()
    {
        
        var turnPieces = pieces.Where(piece => piece.pieceColor == _turn).ToList();
        if (turnPieces.Any())
        {
            var bestMove = BestMove(turnPieces);
            if (bestMove != null && bestMove.NotZero())
            {
                MoveOnePiece(turnPieces, bestMove.piece, bestMove.x, bestMove.y);
            }
        }
        else
        {
            Debug.Log($"No pieces for {_turn}");
        }

        EndTurn();
    }

    private void EndTurn()
    {
        _turn = _turn switch
        {
            PieceColor.WHITE => PieceColor.BLACK,
            _ => PieceColor.WHITE
        };

        UpdatePieceLocations();
    }

    void FixedUpdate()
    {
        if (!_weAreLive) return;
        if (++_ticksSinceLastMove % ticksPerMove != 0) return;
        DoRandomBoardMove();
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

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _weAreLive = true;
            DoRandomBoardMove();
        }
    }
}