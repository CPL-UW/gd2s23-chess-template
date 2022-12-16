using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    void Start()
    {
        name = $"{pieceColor}_{pieceType}_{cx}_{cy}";
        pieceState = ChessInfo.PieceState.ALIVE;
        pieceID = Random.Range(10000000, 100000000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
