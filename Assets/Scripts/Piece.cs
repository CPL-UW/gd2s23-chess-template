using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public ChessInfo.PieceType pieceType = ChessInfo.PieceType.NONE;
    public ChessInfo.PieceColor pieceColor = ChessInfo.PieceColor.NONE;
    public ChessInfo.PieceState pieceState = ChessInfo.PieceState.NONE;
    public ChessInfo.CX cx = ChessInfo.CX.NONE;
    public ChessInfo.CY cy = ChessInfo.CY.NONE;
    
    void Start()
    {
        name = $"{pieceColor}_{pieceType}_{cx}_{cy}";
        pieceState = ChessInfo.PieceState.ALIVE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
