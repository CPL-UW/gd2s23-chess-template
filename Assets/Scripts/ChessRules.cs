using System.Collections.Generic;
using static ChessInfo;

public static class ChessRules 
{
    
    private static Piece[,] BlankBoard()
    {
        var board = new Piece[9, 9];
        for (var i = 0; i <= 8; i++)
        {
            for (var j = 0; j <= 8; j++)
            {
                board[i, j] = null;
            }
        }
        return board;
    }
    
    public static Piece[,] GetBoard(List<Piece> pieces)
    {
        var board = BlankBoard();
        foreach (var piece in pieces)
        {
            board[piece.cx, piece.cy] = piece;
        }
        return board;
    }
    //
    // public static PieceInfo[,] MovePiece(PieceInfo[,] board, int startX, int startY, int targetX, int targetY)
    // {
    //     board[targetX, targetY].SetXY(startX, startY);
    //     board[startX, startY].SetXY(targetX, targetY);
    //     (board[startX, startY], board[targetX, targetY]) = (board[targetX, targetY], board[startX, startY]);
    //     return board;
    // }
}
