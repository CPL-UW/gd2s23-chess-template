using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// chess x = a-h, y = 1-8
public class BoardManager : MonoBehaviour
{
    public List<Piece> p1Pieces;
    public List<Piece> p2Pieces;

    private float _screenWidth;
    private const float TOLERANCE = 0.1f;
    private Rect _pieceBounds = Rect.zero;
    private float xMax;
    private float yMax;
    private float xMin;
    private float yMin;
    private float _dx = 1;
    private float _dy = 1;
    private ChessInfo.PieceColor _turn = ChessInfo.PieceColor.WHITE;


    private void GetPieceGridBounds()
    {
        xMax = xMin = 0;
        foreach (var p in p1Pieces.Select(piece => piece.transform.position))
        {
            if (p.x > xMax)
                xMax = p.x;
            if (p.x < xMin)
                xMin = p.x;
        }

        _dx = (xMax - xMin) / 7;
        _dy = _dx;
        Debug.Log($"BOUNDS_UPDATE: {xMin}=>{xMax} dx={_dx}");
    }

    private Vector2Int GetGridPosition(Vector3 pos)
    {
        return new Vector2Int((int) ((_pieceBounds.xMax + pos.x) / _dx), (int) ((_pieceBounds.yMax + pos.y) / _dy));
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
            if (_turn == ChessInfo.PieceColor.WHITE)
            {
                var randomPiece = p1Pieces.ElementAt(Random.Range(0, p1Pieces.Count));
                randomPiece.transform.Translate(0, _dy, 0);
                _turn = ChessInfo.PieceColor.BLACK;
            }
            else
            {
                var randomPiece = p2Pieces.ElementAt(Random.Range(0, p2Pieces.Count));
                randomPiece.transform.Translate(0, -_dy, 0);
                _turn = ChessInfo.PieceColor.WHITE;
            }
        }
    }
}