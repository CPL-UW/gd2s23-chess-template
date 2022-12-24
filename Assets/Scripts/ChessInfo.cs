using System.Collections.Generic;
using System.Linq;
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
        public readonly IPieceData piece;
        public int score;

        public PieceMove(IPieceData curPiece, int cx, int cy)
        {
            piece = curPiece;
            x = cx;
            y = cy;
        }

        public PieceMove()
        {
            x = y = 0;
            score = 0;
        }

        public bool NotZero()
        {
            return 0 != x || 0 != y;
        }
    }
    
   public static string PieceListToString(IEnumerable<IPieceData> pieces)
   {
       return pieces.OrderByDescending(piece => piece.Y() * 100 + piece.X()).Aggregate("", (current, piece) => current + piece.LocID() + ":");
   }
    
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
