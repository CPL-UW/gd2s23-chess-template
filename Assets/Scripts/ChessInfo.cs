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
        public List<IPieceData> simBoard = null;

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
    
   public static string PieceListToString(List<IPieceData> pieces)
   {
       var output = "";
       for (var y = 8; y >= 1; y--)
       {
           for (var x = 1; x <= 8; x++)
           {
               var piece = ChessRules.GetPieceAt(ref pieces, x, y);
               if (null == piece) output += ".";
               else
               {
                   var t = piece.PType() switch
                   {
                       PieceType.PAWN => "p",
                       PieceType.KING => "k",
                       PieceType.QUEEN => "q",
                       PieceType.ROOK => "r",
                       PieceType.BISHOP => "b",
                       PieceType.KNIGHT => "n",
                       _ => "."
                   };
                   if (piece.Color() == PieceColor.WHITE) t = t.ToUpper();
                   output += t;
               }
           }
           output += "\n";
       }

       return output;
   }
    
    public static readonly string [] PIECE_COLOR = {"white", "black"};
    public static readonly int BOARD_SIZE = 8;
   
}
