using System;
using System.Collections.Generic;
using System.Linq;
using static ChessInfo;
using static ChessRules;

public abstract class ChessAI
{
    protected string aiName = "ChessAI";
    public string GetAIDescription()
    {
        return aiName;
    }
    
    public static int BoardScore(ref List<IPieceData> pieces, PieceColor turn)
    {
        var score = 0;
        var livePieces = pieces.Where(p => p.Alive());
        foreach (var piece in livePieces)
        {
            var localScore = piece.PType() switch
            {
                PieceType.PAWN => 1,
                PieceType.KING => 1000,
                PieceType.QUEEN => 100,
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
        return pieces.Select(piece => new PieceInfo(piece)).Cast<IPieceData>().ToList();
    }

    public PieceMove BestMove(ref List<IPieceData> pieces, PieceColor turn)
    {
        return BestScoredMove(ref pieces, GetValidMovesByTurn(ref pieces, turn), turn);
    }
}

public class ChessAIDeep : ChessAI
{
    private static int _maxMoves = 5;
    private static int _maxDepth = 3;

    public ChessAIDeep(int maxMoves, int maxDepth)
    {
        _maxMoves = maxMoves;
        _maxDepth = maxDepth;
        aiName = "Deep AI";
    }

    private static Tuple<int, List<IPieceData>> SimulateMove(List<IPieceData> pieces, PieceColor turn, int startX, int startY, int dcx, int dcy)
    {
        var simBoard = SimBoardCopy(pieces);
        MoveXY(ref simBoard, startX, startY, dcx, dcy);
        return new Tuple<int, List<IPieceData>>(BoardScore(ref simBoard, turn), simBoard);
    }

    public static PieceMove BestMoveDeep(List<IPieceData> pieces, PieceColor turn, int depth)
    {
        if (pieces.Count == 0 || depth <= 0) return new PieceMove();
        var validMoves = GetValidMovesByTurn(ref pieces, turn);
        if (validMoves.Count == 0) return new PieceMove();
        
        // also could map to a tuple of move and score
        foreach (var move in validMoves)
        {
            var simBoard = SimBoardCopy(pieces.Where(piece => piece.Alive()));
            (move.score, move.simBoard) = SimulateMove(simBoard, turn, move.piece.X(), move.piece.Y(), move.x, move.y);
        }
        
        // if (validMoves.Count > MAX_MOVES)
        // {
        validMoves = validMoves.OrderByDescending(move => move.score).Take(_maxMoves).ToList();
        // }
        // validMoves = validMoves.OrderByDescending(m => m.score).ToList().GetRange(0, MAX_MOVES);
        
        foreach (var move in validMoves)
        {
            move.score += -1 * BestMoveDeep(move.simBoard, OtherColor(turn), depth - 1).score;
            move.simBoard = null;
        }

        return validMoves.OrderByDescending(m => m.score).FirstOrDefault();
    }

    protected override PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        return BestMoveDeep(pieces, turn, _maxDepth);
    }
}

public class ChessAISimple : ChessAI
{
    public ChessAISimple() => aiName = "Simple AI";
    protected override PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        return ChessAIDeep.BestMoveDeep(pieces, turn, 1);
    }
}

public class ChessAIRandom : ChessAI
{
    public ChessAIRandom() => aiName = "Random AI";
    
    protected override PieceMove BestScoredMove(ref List<IPieceData> pieces, List<PieceMove> moves, PieceColor turn)
    {
        if (pieces == null || moves == null || moves.Count == 0 || pieces.Count == 0) return null;
        return moves[0];
    }
}