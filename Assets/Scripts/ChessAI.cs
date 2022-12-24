using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ChessInfo;
using static ChessRules;

public abstract class ChessAI
{
    protected static int BoardScore(ref List<IPieceData> pieces, PieceColor turn)
    {
        var score = 0;
        var livePieces = pieces.Where(p => p.Alive());
        foreach (var piece in livePieces)
        {
            var localScore = piece.PType() switch
            {
                PieceType.PAWN => 1,
                PieceType.KING => 1000,
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

    protected abstract PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn);

    protected static List<IPieceData> SimBoardCopy(IEnumerable<IPieceData> pieces)
    {
        // return pieces.Select(piece => new PieceInfo(piece)).Cast<IPieceData>().ToList();
        var newList = new List<PieceInfo>();
        foreach (var piece in pieces)
        {
            var newPiece = new PieceInfo();
            newPiece.CopyInfo(piece);
            newList.Add(newPiece);
        }

        return newList.Cast<IPieceData>().ToList();
    }

    public PieceMove BestMove(ref List<IPieceData> pieces, PieceColor turn)
    {
        return BestScoredMove(ref pieces, GetValidMovesByTurn(ref pieces, turn), turn);
    }
}

public class ChessAIDeep : ChessAI
{
    private static PieceMove BestMoveDeep(List<IPieceData> pieces, PieceColor turn, int depth)
    {
        if (pieces == null || depth < 0) return new PieceMove();

        var validMoves = GetValidMovesByTurn(ref pieces, turn);
        if (validMoves.Count == 0) return new PieceMove();
        foreach (var move in validMoves)
        {
            var simBoard = SimBoardCopy(pieces.Where(piece => piece.Alive()));
            if (depth > 1 && MoveXY(ref simBoard, move.piece.X(), move.piece.Y(), move.x, move.y))
            {
                var deepMove = BestMoveDeep(simBoard, OtherColor(turn), depth - 1);
                move.score = -1 * deepMove?.score ?? BoardScore(ref pieces, turn);
            }
            else
            {
                move.score = BoardScore(ref pieces, turn);
            }
        }

        return validMoves.OrderByDescending(m => m.score).FirstOrDefault();
    }

    protected override PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        return BestMoveDeep(pieces, turn, 3);
    }
}

public class ChessAIDumb : ChessAI
{
    protected override PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        if (pieces == null || moves == null || moves.Count == 0) return null;
        var scores = "";
        var lastScore = -1;
        PieceMove bestMove = null;
        foreach (var move in moves)
        {
            var futurePieces = SimBoardCopy(pieces);
            var pieceToMove = GetPieceAt(ref futurePieces, move.piece.X(), move.piece.Y());
            MoveOnePiece(ref futurePieces, pieceToMove, move.x, move.y);
            var curScore = BoardScore(ref futurePieces, turn);
            if (lastScore != curScore)
            {
                scores += $"{curScore} ";
                lastScore = curScore;
            }

            if (null == bestMove || curScore >= bestMove.score)
            {
                bestMove = move;
                bestMove.score = curScore;
            }
        }

        Debug.Log($"SCORES (out of {moves.Count}): {scores}");
        return bestMove;
    }
}

public class ChessAIRandom : ChessAI
{
    protected override PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        if (pieces == null || moves == null || moves.Count == 0) return null;
        return moves[0];
    }
}