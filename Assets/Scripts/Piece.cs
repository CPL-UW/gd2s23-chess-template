using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChessInfo;

public class Piece : MonoBehaviour
{
    public ChessInfo.PieceType pieceType = ChessInfo.PieceType.NONE;
    public ChessInfo.PieceColor pieceColor = ChessInfo.PieceColor.NONE;
    public ChessInfo.PieceState pieceState = ChessInfo.PieceState.NONE;
    // public ChessInfo.CX cx = ChessInfo.CX.NONE;
    // public ChessInfo.CY cy = ChessInfo.CY.NONE;
    public int cx;
    public int cy;
    public int pieceID;

    public Piece(Piece other)
    {
        
        pieceType = other.pieceType;
        pieceColor = other.pieceColor;
        pieceState = other.pieceState;
        cx = other.cx;
        cy = other.cy;
    }

    public void SetXY(int x, int y)
    {
        cx = x; cy = y;
    }

    public void RemoveSelf()
    {
        if (pieceState != PieceState.ALIVE) return;
        pieceState = PieceState.DEAD;
        GetComponent<Renderer>().enabled = false;
    }
    
    void Start()
    {
        name = $"{pieceColor}_{pieceType}_{cx}_{cy}";
        pieceState = PieceState.ALIVE;
        pieceID = Random.Range(10000000, 100000000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
