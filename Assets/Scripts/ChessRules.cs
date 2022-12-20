using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ChessInfo;

public static class ChessRules
{
    

    // private static List<PieceInfo> CopyBoard(IEnumerable<Piece> pieces)
    // {
    //     return pieces.Select(piece => new Piece(piece)).ToList();
    // }

    public static bool ValidXY(int x, int y)
    {
        return x is >= 1 and <= 8 && y is >= 1 and <= 8;
    }

    
    private static bool DuplicatesExist(ref List<IPieceData> pieces)
    {
        return pieces
            .GroupBy(x => x.X() + x.Y() * 100).Count(g => g.Count() > 1) > 0;
    }

    private static bool MovePiece(ref List<IPieceData> pieces, int startX, int startY, int targetX, int targetY)
    {
        if (!ValidXY(startX, startY) || !ValidXY(targetX, targetY)) return false;
        if (DuplicatesExist(ref pieces))
        {
            Debug.Log("MovePiece: Duplicates exist! TOP");
            return false;
        }

        var movingPiece = pieces.Find(piece => piece.X() == startX && piece.Y() == startY);
        if (movingPiece == null)
        {
            Debug.Log("Moving piece is null");
            return false;
        }

        var targetPiece = pieces.Find(piece => piece.X() == targetX && piece.Y() == targetY);
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

    public static void MoveOnePiece(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        if (CheckValidMove(ref pieces, pieceToMove, dcx, dcy))
        {
            if (MovePiece(ref pieces, pieceToMove.X(), pieceToMove.Y(), pieceToMove.X() + dcx, pieceToMove.Y() + dcy))
            {
                // Debug.Log($"SUCCESS {pieceToMove.X()},{pieceToMove.Y()} to {pieceToMove.X() + dcx},{pieceToMove.Y() + dcy}");
            }
            else
            {
                Debug.Log($"FAIL {pieceToMove.X()},{pieceToMove.Y()} to {pieceToMove.X() + dcx},{pieceToMove.Y() + dcy}");
            }
        }
        else
        {
            Debug.Log(
                $"Invalid Move: {pieceToMove.PType()} ({pieceToMove.X()},{pieceToMove.Y()}) to ({pieceToMove.X() + dcx},{pieceToMove.Y() + dcy})");
        }
    }

    private static IPieceData GetPieceAt(ref List<IPieceData> pieces, int x, int y)
    {
        return pieces.Find(piece => piece.X() == x && piece.Y() == y);
    }

    private static bool AnyPieceAt(ref List<IPieceData> pieces, int x, int y)
    {
        return pieces.Any(piece => piece.X() == x && piece.Y() == y);
    }

    private static bool RulePawn(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        var validYDir = pieceToMove.Color() == PieceColor.WHITE ? 1 : -1;
        if (0 == dcx && dcy == validYDir && !AnyPieceAt(ref pieces, pieceToMove.X(), pieceToMove.Y() + dcy))
            return true;
        if (1 != Mathf.Abs(dcx) || dcy != validYDir) return false;
        var targetPiece = GetPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy);
        return null != targetPiece && targetPiece.Color() != pieceToMove.Color();
    }

    private static bool RuleKing(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        if (Mathf.Abs(dcx) > 1 || Mathf.Abs(dcy) > 1) return false;
        var targetPiece = GetPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy);
        return !AnyPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy) ||
               targetPiece.Color() != pieceToMove.Color();
    }

    private static bool RuleQueen(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        return RuleRook(ref pieces, pieceToMove, dcx, dcy) || RuleBishop(ref pieces, pieceToMove, dcx, dcy);
    }

    private static bool RuleRook(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        // only move in the x or y direction
        if (dcx * dcy != 0 || (dcx == 0 && dcy == 0)) return false;
        
        // cannot be anyone on the way there
        var xDir = dcx != 0 ? dcx / Mathf.Abs(dcx) : 0;
        var yDir = dcy != 0 ? dcy / Mathf.Abs(dcy) : 0;
    
        var startX = pieceToMove.X() + xDir;
        var startY = pieceToMove.Y() + yDir;
        for (int x  = startX,  y  = startY; 
                 x != pieceToMove.X() + dcx || y != pieceToMove.Y() + dcy; 
                 x += xDir,                   y += yDir)
        {
            if (AnyPieceAt(ref pieces,x,y))
                return false;
        }
        
        // if the way was clean, check the target
        var targetPiece = GetPieceAt(ref pieces,pieceToMove.X() + dcx, pieceToMove.Y() + dcy);
        return null == targetPiece || targetPiece.Color() != pieceToMove.Color();
    }
    

    private static bool RuleBishop(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        // only move diagonal
        if (dcx == 0 || dcy == 0 || Mathf.Abs(dcx) != Mathf.Abs(dcy)) return false;

        // cannot be anyone on the way there
        var yDir = dcy / Mathf.Abs(dcy);
        var xDir = dcx / Mathf.Abs(dcx);
        for (int x = pieceToMove.X() + xDir, y = pieceToMove.Y() + yDir; x != pieceToMove.X() + dcx; x += xDir, y += yDir)
        {
            if (AnyPieceAt(ref pieces, x, y))
                return false;
        }

        // if the way was clean, check the target
        var targetPiece = GetPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy);
        return !AnyPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy) ||
               targetPiece.Color() != pieceToMove.Color();
    }

    private static bool RuleKnight(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        if ((Mathf.Abs(dcx) == 2 && Mathf.Abs(dcy) == 1) ||
            (Mathf.Abs(dcx) == 1 && Mathf.Abs(dcy) == 2))
        {
            var targetPiece = GetPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy);
            if (!AnyPieceAt(ref pieces, pieceToMove.X() + dcx, pieceToMove.Y() + dcy) ||
                targetPiece.Color() != pieceToMove.Color())
                return true;
        }

        return false;
    }


    private static bool CheckPieceRules(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        if (!ValidXY(pieceToMove.X() + dcx, pieceToMove.Y() + dcy)) return false;
        return pieceToMove.PType() switch
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

    private static bool CheckValidMove(ref List<IPieceData> pieces, IPieceData pieceToMove, int dcx, int dcy)
    {
        if (null == pieceToMove ||
            !ValidXY(pieceToMove.X(), pieceToMove.Y()) ||
            !ValidXY(pieceToMove.X() + dcx, pieceToMove.Y() + dcy))
            return false;
        return CheckPieceRules(ref pieces, pieceToMove, dcx, dcy);
    }

    private static IEnumerable<PieceMove> GetValidMoves(ref List<IPieceData> pieces, IPieceData pieceToMove)
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
    
    private static int BoardScore(ref List<IPieceData> pieces, PieceColor turn)
    {
        var score = 0;
        var livePieces = pieces.Where(p => p.Alive()).ToList();
        foreach (var piece in livePieces)
        {
            var localScore = piece.PType() switch
            {
                PieceType.PAWN => 1,
                PieceType.KING => 100,
                PieceType.QUEEN => 9,
                PieceType.ROOK => 5,
                PieceType.BISHOP => 3,
                PieceType.KNIGHT => 3,
                _ => 0
            };
            if (piece.Color() != turn) localScore *= -1;
            score += localScore;
        }

        return score;
    }

    // private static List<PieceInfo> GetTestBoard(List<PieceInfo> pieces)
    // {
    //     return pieces.Select(piece => new PieceInfo(piece)).ToList();
    // }

    private static PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        if (pieces == null || moves == null || moves.Count == 0) return null;
        // var scores = "";
        // var lastScore = -1;
        // PieceMove bestMove = null;
        // foreach (var move in moves)
        // {
        //     var futurePieces = CopyBoard(pieces);
        //     var pieceToMove = GetPieceAt(ref futurePieces, move.piece.X(), move.piece.Y());
        //     MoveOnePiece(ref futurePieces, pieceToMove, move.x, move.y);
        //     var curScore = BoardScore(ref futurePieces, turn);
        //     if (lastScore != curScore)
        //     {
        //         scores += $"{curScore} ";
        //         lastScore = curScore;
        //     }
        //     if (null == bestMove || curScore >= bestMove.score)
        //     {
        //         bestMove = move;
        //         bestMove.score = curScore;
        //     }
        // }
        // Debug.Log($"SCORES (out of {moves.Count}): {scores}"); // TODO always same scores :( 
        // return bestMove;
        return moves[0];
    }
    
    public static PieceMove BestMove(ref List<IPieceData> pieces, PieceColor turn)
    {
        var validMoves = new List<PieceMove>();
        var turnPieces = pieces.Where(piece => piece.Color() == turn);
        foreach (var piece in turnPieces)
        {
            validMoves.AddRange(GetValidMoves(ref pieces, piece));
        }
        validMoves.Shuffle();
        // return validMoves.Count == 0 ? null : validMoves[Random.Range(0, validMoves.Count)];
        return BestScoredMove(ref pieces, validMoves, turn);
    }
    
}