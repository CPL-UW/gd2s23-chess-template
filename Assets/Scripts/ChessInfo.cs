using System;
using System.Collections.Generic;

public static class ChessInfo
{
    public enum PieceType
    {
        NONE, PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING
    }
    public enum PieceState
    {
        NONE, ALIVE, DEAD
    }

    public enum PieceColor
    {
        NONE, BLACK, WHITE
    }
    
    public enum CX { NONE, A, B, C, D, E, F, G, H }
    public enum CY { NONE, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT }
    
    
    public static readonly string [] PIECE_COLOR = {"white", "black"};
    public static readonly int BOARD_SIZE = 8;
    public static readonly Dictionary<string, PieceType> PIECE_MAP = new() 
        {
            { "none", PieceType.NONE },
            { "bishop", PieceType.BISHOP },
            { "king", PieceType.KING },
            { "knight", PieceType.KNIGHT },
            { "pawn", PieceType.PAWN },
            { "queen", PieceType.QUEEN },
            { "rook", PieceType.ROOK }
        };
}
