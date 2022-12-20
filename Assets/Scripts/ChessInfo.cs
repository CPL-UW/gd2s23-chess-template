using System.Collections.Generic;
using UnityEngine;


public static class ChessInfo
{

    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;  
        while (n-- > 1) {
            var k = Random.Range(0,n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }
    
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

    public class PieceMove
    {
        public readonly int x;
        public readonly int y;
        public readonly Piece piece;
        public float score;

        public PieceMove(Piece curPiece, int cx, int cy)
        {
            piece = curPiece;
            x = cx;
            y = cy;
        }

        public bool NotZero()
        {
            return 0 != x || 0 != y;
        }
    }
    
    // public enum CX { NONE, A, B, C, D, E, F, G, H }
    // public enum CY { NONE, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT }
    //
    // public class PieceInfo
    // {
    //     public PieceType type;
    //     public PieceState state;
    //     public PieceColor color;
    //     public int x;
    //     public int y;
    //     public int pieceID;
    //
    //     public PieceInfo()
    //     {
    //         color = PieceColor.NONE;
    //         type = PieceType.NONE;
    //         state = PieceState.NONE;
    //         y = -1;
    //         x = -1;
    //     }
    //     
    //     public PieceInfo(Piece piece)
    //     {
    //         color = piece.pieceColor;
    //         type = piece.pieceType;
    //         state = piece.pieceState;
    //         x = piece.cx;
    //         y = piece.cy;
    //         pieceID = piece.pieceID;
    //     }
    //     
    //     public bool Equals(PieceInfo other)
    //     {
    //         return pieceID == other.pieceID;
    //     }
    //     
    //     public bool Equals(Piece other)
    //     {
    //         return pieceID == other.pieceID;
    //     }
    //
    //     public void SetXY(int tx, int ty)
    //     {
    //         x = tx;
    //         y = ty;
    //     }
    // }

    
    
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
