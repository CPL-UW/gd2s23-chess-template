using System.Collections.Generic;
using UnityEngine;
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

    public static Piece[,] CopyBoard(List<Piece> pieces)
    {
        var board = BlankBoard();
        foreach (var piece in pieces)
        {
            board[piece.cx, piece.cy] = new(piece);
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
        var board = GetBoard(pieces);
        if (CheckValidMove(board,pieceToMove,dcx,dcy))
            MovePiece(board, pieceToMove.cx, pieceToMove.cy, pieceToMove.cx + dcx, pieceToMove.cy + dcy);
        else
        {
            Debug.Log($"Invalid Move: {pieceToMove.pieceType} ({pieceToMove.cx},{pieceToMove.cy}) to ({pieceToMove.cx + dcx},{pieceToMove.cy + dcy})");
        }
    }

    private static bool RulePawn(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        var validYDir = pieceToMove.pieceColor == PieceColor.WHITE ? 1 : -1;
        if (0 == dcx && dcy == validYDir && null == board[pieceToMove.cx, pieceToMove.cy + dcy])
            return true;
        if (1 == Mathf.Abs(dcx) && dcy == validYDir)
        {
            var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
            if (null != targetPiece && targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        return false;
    }

    private static bool RuleKing(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (Mathf.Abs(dcx) <= 1 && Mathf.Abs(dcy) <= 1)
        {
            var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
            if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        return false;
    }
    
    private static bool RuleQueen(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        return RuleRook(board, pieceToMove, dcx, dcy) || RuleBishop(board, pieceToMove, dcx, dcy);
    }
    
    private static bool RuleRook(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (0 == dcx && 0 != dcy)
        {
            var yDir = dcy / Mathf.Abs(dcy);
            for (var y = pieceToMove.cy + yDir; y != pieceToMove.cy + dcy; y += yDir)
            {
                if (null != board[pieceToMove.cx, y])
                    return false;
            }
            var targetPiece = board[pieceToMove.cx, pieceToMove.cy + dcy];
            if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        else if (0 == dcy && 0 != dcx)
        {
            var xDir = dcx / Mathf.Abs(dcx);
            for (var x = pieceToMove.cx + xDir; x != pieceToMove.cx + dcx; x += xDir)
            {
                if (null != board[x, pieceToMove.cy])
                    return false;
            }
            var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy];
            if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        return false;
    }
    
    private static bool RuleBishop(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (dcx != 0 && Mathf.Abs(dcx) == Mathf.Abs(dcy))
        {
            var xDir = dcx / Mathf.Abs(dcx);
            var yDir = dcy / Mathf.Abs(dcy);
            for (int x = pieceToMove.cx + xDir, y = pieceToMove.cy + yDir; x != pieceToMove.cx + dcx; x += xDir, y += yDir)
            {
                if (null != board[x, y])
                    return false;
            }
            var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
            if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        return false;
    }
    
    private static bool RuleKnight(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (Mathf.Abs(dcx) == 2 && Mathf.Abs(dcy) == 1)
        {
            var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
            if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        else if (Mathf.Abs(dcx) == 1 && Mathf.Abs(dcy) == 2)
        {
            var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
            if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }
        return false;
    }
    
    
    private static bool CheckPieceRules(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        return pieceToMove.pieceType switch
        {
            PieceType.PAWN => RulePawn(board, pieceToMove, dcx, dcy),
            PieceType.KING => RuleKing(board, pieceToMove, dcx, dcy),
            PieceType.QUEEN => RuleQueen(board, pieceToMove, dcx, dcy),
            PieceType.ROOK => RuleRook(board, pieceToMove, dcx, dcy),
            PieceType.BISHOP => RuleBishop(board, pieceToMove, dcx, dcy),
            PieceType.KNIGHT => RuleKnight(board, pieceToMove, dcx, dcy),
            _ => false
        };
    }
    
    public static bool CheckValidMove(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (null == pieceToMove || 
            !ValidXY(pieceToMove.cx,pieceToMove.cy) || 
            !ValidXY(pieceToMove.cx + dcx, pieceToMove.cy + dcy)) return false;
        return CheckPieceRules(board, pieceToMove, dcx, dcy);
    }
    
    public static List<PieceMove> GetValidMoves(Piece[,] board, Piece pieceToMove)
    {
        var validMoves = new List<PieceMove>();
        for (var dx = -7; dx <= 7; dx++)
        {
            for (var dy = -7; dy <= 7; dy++)
            {
                if (CheckValidMove(board, pieceToMove, dx, dy))
                {
                    validMoves.Add(new PieceMove(dx, dy));
                }
            }
        }
        return validMoves;
    }
    
    
    public static PieceMove BestMove(List<Piece> pieces, Piece pieceToMove)
    {
        var validMoves = GetValidMoves(GetBoard(pieces), pieceToMove);
        return validMoves.Count == 0 ? null : validMoves[Random.Range(0, validMoves.Count)];
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
