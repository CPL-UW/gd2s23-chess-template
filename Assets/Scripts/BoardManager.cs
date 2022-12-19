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
    

    private void GetPieceGridBounds()
    {
        _xMax = _xMin = 0;
        _yMax = _yMin = 0;
        foreach (var p in pieces.Select(piece => piece.transform.position))
        {
            if (p.x > _xMax)
                _xMax = p.x;
            if (p.x < _xMin)
                _xMin = p.x;
            if (p.y > _yMax)
                _yMax = p.y;
            if (p.y < _yMin)
                _yMin = p.y;
        }

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
                piece.transform.position = new Vector3(x, y, 0);
            }
        }
    }

    private void DoRandomBoardMove()
    {
        
        var turnPieces = pieces.Where(piece => piece.pieceState == PieceState.ALIVE && piece.pieceColor == _turn).ToList();
        if (turnPieces.Any())
        {
            // var randomPiece = turnPieces.ElementAt(Random.Range(0, turnPieces.Count()));
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
            GetPieceGridBounds();
            DoRandomBoardMove();
        }
    }
}