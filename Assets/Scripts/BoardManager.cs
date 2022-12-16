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

    

    private void GetPieceGridBounds()
    {
        _xMax = _xMin = 0;
        foreach (var p in pieces.Select(piece => piece.transform.position))
        {
            if (p.x > _xMax)
                _xMax = p.x;
            if (p.x < _xMin)
                _xMin = p.x;
        }

        _dx = (_xMax - _xMin) / 7;
        _dy = _dx;
        Debug.Log($"BOUNDS_UPDATE: {_xMin}=>{_xMax} dx={_dx}");
    }

    // private Vector2Int GetGridPosition(Vector3 pos)
    // {
    //     return new Vector2Int((int) ((_pieceBounds.xMax + pos.x) / _dx), (int) ((_pieceBounds.yMax + pos.y) / _dy));
    // }

    // private void UpdatePiecesFromBoard(Piece[,] board)
    // {
    //     for (var i = 1; i <= 8; i++)
    //     {
    //         for (var j = 1; j <= 8; j++)
    //         {
    //         }
    //     }
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(_screenWidth - Screen.width) > TOLERANCE)
        {
            GetPieceGridBounds();
            _screenWidth = Screen.width;
        }

        // if (Input.GetKeyUp(KeyCode.Space))
        // {
        //     GetPieceGridBounds();
        //     if (_turn == PieceColor.WHITE)
        //     {
        //         var randomPiece = p1Pieces.ElementAt(Random.Range(0, p1Pieces.Count));
        //         randomPiece.transform.Translate(0, _dy, 0);
        //         _turn = PieceColor.BLACK;
        //     }
        //     else
        //     {
        //         var randomPiece = p2Pieces.ElementAt(Random.Range(0, p2Pieces.Count));
        //         randomPiece.transform.Translate(0, -_dy, 0);
        //         _turn = PieceColor.WHITE;
        //     }
        // }
    }
}