using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static ChessInfo;
using Random = UnityEngine.Random;

public static class ChessRules
{
    // private static Piece[,] BlankBoard()
    // {
    //     var board = new Piece[9, 9];
    //     for (var i = 0; i <= 8; i++)
    //     {
    //         for (var j = 0; j <= 8; j++)
    //         {
    //             board[i, j] = null;
    //         }
    //     }
    //     return board;
    // }
    //
    // private static Piece[,] GetBoard(IEnumerable<Piece> pieces)
    // {
    //     var board = BlankBoard();
    //     var livePieces = pieces.Where(piece => piece.pieceState == PieceState.ALIVE);
    //     foreach (var piece in livePieces)
    //     {
    //         board[piece.cx, piece.cy] = piece;
    //     }
    //     return board;
    // }

    // public static Piece[,] CopyBoard(List<Piece> pieces)
    // {
    //     var board = BlankBoard();
    //     foreach (var piece in pieces)
    //     {
    //         board[piece.cx, piece.cy] = new(piece);
    //     }
    //     return board;
    // }

    public static bool ValidXY(int x, int y)
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

    private static bool DuplicatesExist(ref List<Piece> pieces)
    {
        return pieces
            .GroupBy(x => x.cx + x.cy * 100).Count(g => g.Count() > 1) > 0;
    }

    private static bool MovePiece(ref List<Piece> pieces, int startX, int startY, int targetX, int targetY)
    {
        if (!ValidXY(startX, startY) || !ValidXY(targetX, targetY)) return false;
        if (DuplicatesExist(ref pieces))
        {
            Debug.Log("MovePiece: Duplicates exist! TOP");
            return false;
        }

        var movingPiece = pieces.Find(piece => piece.cx == startX && piece.cy == startY);
        if (movingPiece == null)
        {
            Debug.Log("Moving piece is null");
            return false;
        }

        var targetPiece = pieces.Find(piece => piece.cx == targetX && piece.cy == targetY);
        if (null != targetPiece)
        {
            // Debug.Log($"REMOVING {targetPiece}");
            targetPiece.RemoveSelf();
        }

        movingPiece.SetXY(targetX, targetY);
        // if (DuplicatesExist(ref pieces))
        // {
        //     Debug.Log("MovePiece: Duplicates exist! BOTTOM");
        // }
        return true;
    }

    public static void MoveOnePiece(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        if (CheckValidMove(ref pieces, pieceToMove, dcx, dcy))
        {
            if (MovePiece(ref pieces, pieceToMove.cx, pieceToMove.cy, pieceToMove.cx + dcx, pieceToMove.cy + dcy))
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
            Debug.Log(
                $"Invalid Move: {pieceToMove.pieceType} ({pieceToMove.cx},{pieceToMove.cy}) to ({pieceToMove.cx + dcx},{pieceToMove.cy + dcy})");
        }
    }

    private static Piece GetPieceAt(ref List<Piece> pieces, int x, int y)
    {
        return pieces.Find(piece => piece.cx == x && piece.cy == y);
    }

    private static bool AnyPieceAt(ref List<Piece> pieces, int x, int y)
    {
        return pieces.Any(piece => piece.cx == x && piece.cy == y);
    }

    private static bool RulePawn(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        var validYDir = pieceToMove.pieceColor == PieceColor.WHITE ? 1 : -1;
        if (0 == dcx && dcy == validYDir && !AnyPieceAt(ref pieces, pieceToMove.cx, pieceToMove.cy + dcy))
            return true;
        if (1 != Mathf.Abs(dcx) || dcy != validYDir) return false;
        var targetPiece = GetPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy);
        return null != targetPiece && targetPiece.pieceColor != pieceToMove.pieceColor;
    }

    private static bool RuleKing(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        if (Mathf.Abs(dcx) > 1 || Mathf.Abs(dcy) > 1) return false;
        var targetPiece = GetPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy);
        return !AnyPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy) ||
               targetPiece.pieceColor != pieceToMove.pieceColor;
    }

    private static bool RuleQueen(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        return RuleRook(ref pieces, pieceToMove, dcx, dcy) || RuleBishop(ref pieces, pieceToMove, dcx, dcy);
    }

    private static bool RuleRook(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        // only move in the x or y direction
        // if (!((0 != dcx && 0 == dcy) || (0 == dcx && 0 != dcy))) return false;
        if (dcx * dcy != 0 || (dcx == 0 && dcy == 0)) return false;
        
        // cannot be anyone on the way there
        var xDir = dcx != 0 ? dcx / Mathf.Abs(dcx) : 0;
        var yDir = dcy != 0 ? dcy / Mathf.Abs(dcy) : 0;
    
        var startX = pieceToMove.cx + xDir;
        var startY = pieceToMove.cy + yDir;
        for (int x  = startX,  y  = startY; 
                 x != pieceToMove.cx + dcx || y != pieceToMove.cy + dcy; 
                 x += xDir,                   y += yDir)
        {
            if (AnyPieceAt(ref pieces,x,y))
                return false;
        }
        
        // if the way was clean, check the target
        var targetPiece = GetPieceAt(ref pieces,pieceToMove.cx + dcx, pieceToMove.cy + dcy);
        return null == targetPiece || targetPiece.pieceColor != pieceToMove.pieceColor;
    }

    // private static bool RuleRook(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    // {
    //     var startX = pieceToMove.cx;
    //     var startY = pieceToMove.cy;
    //     var targetX = pieceToMove.cx + dcx;
    //     var targetY = pieceToMove.cy + dcy;
    //
    //     if (startX == targetX && startY == targetY) return false;
    //     if (!((startX == targetX && startY != targetY) || (startX != targetX && startY == targetY))) return false;
    //
    //     var xDir = startX == targetX ? 0 : (targetX - startX) / Mathf.Abs(targetX - startX);
    //     var yDir = startY == targetY ? 0 : (targetY - startY) / Mathf.Abs(targetY - startY);
    //
    //     for (int x = startX + xDir, y = startY + yDir; x != targetX || y != targetY; x += xDir, y += yDir)
    //     {
    //         if (AnyPieceAt(ref pieces, x, y))
    //             return false;
    //     }
    //
    //     if (!AnyPieceAt(ref pieces, targetX, targetY)) return true;
    //     var targetPiece = GetPieceAt(ref pieces, targetX, targetY);
    //     return targetPiece.pieceColor != pieceToMove.pieceColor;
    // }

    private static bool RuleBishop(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        // only move diagonal
        if (dcx == 0 || dcy == 0 || Mathf.Abs(dcx) != Mathf.Abs(dcy)) return false;

        // cannot be anyone on the way there
        var yDir = dcy / Mathf.Abs(dcy);
        var xDir = dcx / Mathf.Abs(dcx);
        for (int x = pieceToMove.cx + xDir, y = pieceToMove.cy + yDir; x != pieceToMove.cx + dcx; x += xDir, y += yDir)
        {
            if (AnyPieceAt(ref pieces, x, y))
                return false;
        }

        // if the way was clean, check the target
        var targetPiece = GetPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy);
        return !AnyPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy) ||
               targetPiece.pieceColor != pieceToMove.pieceColor;
    }

    private static bool RuleKnight(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        if ((Mathf.Abs(dcx) == 2 && Mathf.Abs(dcy) == 1) ||
            (Mathf.Abs(dcx) == 1 && Mathf.Abs(dcy) == 2))
        {
            var targetPiece = GetPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy);
            if (!AnyPieceAt(ref pieces, pieceToMove.cx + dcx, pieceToMove.cy + dcy) ||
                targetPiece.pieceColor != pieceToMove.pieceColor)
                return true;
        }

        return false;
    }


    private static bool CheckPieceRules(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        if (!ValidXY(pieceToMove.cx + dcx, pieceToMove.cy + dcy)) return false;
        return pieceToMove.pieceType switch
        {
            PieceType.PAWN => RulePawn(ref pieces, pieceToMove, dcx, dcy),
            PieceType.KING => RuleKing(ref pieces, pieceToMove, dcx, dcy),
            PieceType.QUEEN => RuleQueen(ref pieces, pieceToMove, dcx, dcy),
            PieceType.ROOK => RuleRook(ref pieces, pieceToMove, dcx, dcy),
            PieceType.BISHOP => RuleBishop(ref pieces, pieceToMove, dcx, dcy),
            PieceType.KNIGHT => RuleKnight(ref pieces, pieceToMove, dcx, dcy),
            _ => false
        };
    }

    private static bool CheckValidMove(ref List<Piece> pieces, Piece pieceToMove, int dcx, int dcy)
    {
        if (null == pieceToMove ||
            !ValidXY(pieceToMove.cx, pieceToMove.cy) ||
            !ValidXY(pieceToMove.cx + dcx, pieceToMove.cy + dcy))
            return false;
        return CheckPieceRules(ref pieces, pieceToMove, dcx, dcy);
    }

    private static IEnumerable<PieceMove> GetValidMoves(ref List<Piece> pieces, Piece pieceToMove)
    {
        var validMoves = new List<PieceMove>();
        for (var dx = -7; dx <= 7; dx++)
        {
            for (var dy = -7; dy <= 7; dy++)
            {
                if (CheckValidMove(ref pieces, pieceToMove, dx, dy))
                {
                    validMoves.Add(new PieceMove(pieceToMove, dx, dy));
                }
            }
        }

        return validMoves;
    }


    public static PieceMove BestMove(ref List<Piece> pieces, PieceColor turn)
    {
        var validMoves = new List<PieceMove>();
        var turnPieces = pieces.Where(piece => piece.pieceColor == turn);
        foreach (var piece in turnPieces)
        {
            validMoves.AddRange(GetValidMoves(ref pieces, piece));
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