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

    private static bool ValidXY(int x, int y)
    {
        return x is >= 1 and <= 8 && y is >= 1 and <= 8;
    }
    
    public static Piece[,] MovePiece(Piece[,] board, int startX, int startY, int targetX, int targetY)
    {
        if (!ValidXY(startX, startY) || !ValidXY(targetX, targetY)) return board;
        if (null != board[targetX, targetY])
            board[targetX, targetY].SetXY(startX, startY);
        if (null != board[startX, startY])
            board[startX, startY].SetXY(targetX, targetY);
        (board[startX, startY], board[targetX, targetY]) = (board[targetX, targetY], board[startX, startY]);
        return board;
    }

    public static void MoveOnePiece(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        MovePiece(GetBoard(pieces), pieceToMove.cx, pieceToMove.cy, pieceToMove.cx + dcx, pieceToMove.cy + dcy);
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
