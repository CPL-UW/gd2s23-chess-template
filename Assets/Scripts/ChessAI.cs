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

    protected abstract PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn);

    protected List<IPieceData> SimBoardCopy(ref List<IPieceData> pieces)
    {
        return pieces.Select(piece => new PieceInfo(piece)).Cast<IPieceData>().ToList();
    }

    public PieceMove BestMove(ref List<IPieceData> pieces, PieceColor turn)
    {
        var validMoves = new List<PieceMove>();
        var turnPieces = pieces.Where(piece => piece.Color() == turn);
        foreach (var piece in turnPieces)
        {
            validMoves.AddRange(GetValidMoves(ref pieces, piece));
        }
        validMoves.Shuffle();
        return BestScoredMove(ref pieces, validMoves, turn);
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
            var futurePieces = SimBoardCopy(ref pieces);
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