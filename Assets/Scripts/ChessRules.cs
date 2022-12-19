using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static ChessInfo;
using Random = UnityEngine.Random;

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
    
    private static Piece[,] GetBoard(IEnumerable<Piece> pieces)
    {
        var board = BlankBoard();
        var livePieces = pieces.Where(piece => piece.pieceState == PieceState.ALIVE);
        foreach (var piece in livePieces)
        {
            board[piece.cx, piece.cy] = piece;
        }
        return board;
    }

    // public static Piece[,] CopyBoard(List<Piece> pieces)
    // {
    //     var board = BlankBoard();
    //     foreach (var piece in pieces)
    //     {
    //         board[piece.cx, piece.cy] = new(piece);
    //     }
    //     return board;
    // }
    
    private static bool ValidXY(int x, int y)
    {
        return x is >= 1 and <= 8 && y is >= 1 and <= 8;
    }

    // private static void MovePiece(Piece[,] board, int startX, int startY, int targetX, int targetY)
    // {
    //     if (!ValidXY(startX, startY) || !ValidXY(targetX, targetY)) return;
    //     var movingPiece = board[startX, startY];
    //     if (movingPiece == null) return;
    //     var targetPiece = board[targetX, targetY];
    //     if (null != targetPiece)
    //     {
    //         targetPiece.RemoveSelf();
    //     }
    //     movingPiece.SetXY(targetX, targetY);
    // }

    private static bool MovePiece(List<Piece> pieces, int startX, int startY, int targetX, int targetY)
    {
        if (!ValidXY(startX, startY) || !ValidXY(targetX, targetY)) return false;
        var movingPiece = pieces.Find(piece => piece.cx == startX && piece.cy == startY);
        if (movingPiece == null)
        {
            Debug.Log("Moving piece is null");
            return false;
        }
        var targetPiece = pieces.Find(piece => piece.cx == targetX && piece.cy == targetY);
        if (null != targetPiece)
        {
            Debug.Log($"REMOVING {targetPiece}");
            targetPiece.RemoveSelf();
        }
        else
        {
            // TODO CLEAR DUP BUG!!
            Debug.Log(pieces.Aggregate("L: ", (output, next) => $"{output}, ({next.cx},{next.cy})"));
        }
        movingPiece.SetXY(targetX, targetY);
        return true;
    }

    public static void MoveOnePiece(List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        var board = GetBoard(pieces);
        if (CheckValidMove(board,pieceToMove,dcx,dcy))
        {
            if (MovePiece(pieces, pieceToMove.cx, pieceToMove.cy, pieceToMove.cx + dcx, pieceToMove.cy + dcy))
            {
                // Debug.Log($"SUCCESS {pieceToMove.cx},{pieceToMove.cy} to {pieceToMove.cx + dcx},{pieceToMove.cy + dcy}");
            }
            else
            {
                Debug.Log($"FAIL {pieceToMove.cx},{pieceToMove.cy} to {pieceToMove.cx + dcx},{pieceToMove.cy + dcy}");
            }
        }
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
        if (1 != Mathf.Abs(dcx) || dcy != validYDir) return false;
        var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
        return null != targetPiece && targetPiece.pieceColor != pieceToMove.pieceColor;
    }

    private static bool RuleKing(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (Mathf.Abs(dcx) > 1 || Mathf.Abs(dcy) > 1) return false;
        var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
        if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
            return true;
        return false;
    }
    
    private static bool RuleQueen(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        return RuleRook(board, pieceToMove, dcx, dcy) || RuleBishop(board, pieceToMove, dcx, dcy);
    }
    
    private static bool RuleRook(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        // only move in the x or y direction
        if ((0 != dcx || 0 == dcy) && (0 != dcy || 0 == dcx)) return false;
        
        // cannot be anyone on the way there
        var yDir = dcy != 0 ? dcy / Mathf.Abs(dcy) : 0;
        var xDir = dcx != 0 ? dcx / Mathf.Abs(dcx) : 0;
        for (int x = pieceToMove.cx + xDir, y = pieceToMove.cy + yDir; 
                 x != pieceToMove.cx + dcx && y != pieceToMove.cy + dcy; 
                 x += xDir, y += yDir)
        {
            if (null != board[x, y])
                return false;
        }
        
        // if the way was clean, check the target
        var targetPiece = board[pieceToMove.cx, pieceToMove.cy + dcy];
        if (null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor)
            return true;
        return false;
    }
    
    private static bool RuleBishop(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        // only move diagonal
        if (dcx == 0 || Mathf.Abs(dcx) != Mathf.Abs(dcy)) return false;
        
        // cannot be anyone on the way there
        var yDir = dcy != 0 ? dcy / Mathf.Abs(dcy) : 0;
        var xDir = dcx / Mathf.Abs(dcx);
        for (int x = pieceToMove.cx + xDir, y = pieceToMove.cy + yDir; x != pieceToMove.cx + dcx; x += xDir, y += yDir)
        {
            if (null != board[x, y]) 
                return false;
        }
        
        // if the way was clean, check the target
        var targetPiece = board[pieceToMove.cx + dcx, pieceToMove.cy + dcy];
        return null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor;
    }
    
    private static bool RuleKnight(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if ((Mathf.Abs(dcx) == 2 && Mathf.Abs(dcy) == 1) || 
            (Mathf.Abs(dcx) == 1 && Mathf.Abs(dcy) == 2))
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

    private static bool CheckValidMove(Piece[,] board, Piece pieceToMove, int dcx, int dcy)
    {
        if (null == pieceToMove || 
            !ValidXY(pieceToMove.cx,pieceToMove.cy) || 
            !ValidXY(pieceToMove.cx + dcx, pieceToMove.cy + dcy)) 
            return false;
        return CheckPieceRules(board, pieceToMove, dcx, dcy);
    }

    private static IEnumerable<PieceMove> GetValidMoves(Piece[,] board, Piece pieceToMove)
    {
        var validMoves = new List<PieceMove>();
        for (var dx = -7; dx <= 7; dx++)
        {
            for (var dy = -7; dy <= 7; dy++)
            {
                if (CheckValidMove(board, pieceToMove, dx, dy))
                {
                    validMoves.Add(new PieceMove(pieceToMove, dx, dy));
                }
            }
        }
        return validMoves;
    }
    
    
    public static PieceMove BestMove(List<Piece> pieces)
    {
        var board = GetBoard(pieces);
        var validMoves = new List<PieceMove>();
        foreach (var piece in pieces)
        {
            validMoves.AddRange(GetValidMoves(board, piece));
        }
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
