using static ChessInfo;

public interface IPieceData
{
    public void SetXY(int x, int y);
    public int X();
    public int Y();
    public bool Alive();
    public PieceColor Color();
    public PieceType PType();
    public int PieceID();
    public void RemoveSelf();
}

public class PieceInfo : IPieceData
{
    private readonly PieceType _pieceType;
    private readonly PieceColor _pieceColor;
    private PieceState _pieceState;
    private int _cx;
    private int _cy;
    private readonly int _pieceID;

    public PieceInfo(IPieceData pieceData)
    {
        _pieceType = pieceData.PType();
        _pieceColor = pieceData.Color();
        _pieceState = pieceData.Alive() ? PieceState.ALIVE : PieceState.DEAD;
        _cx = pieceData.X();
        _cy = pieceData.Y();
        _pieceID = pieceData.PieceID();
    }

    public void SetXY(int x, int y) { _cx = x; _cy = y; }
    
    public int X() { return _cx; }
    public int Y() { return _cy; }
    public int PieceID() { return _pieceID; }

    public bool Alive() { return _pieceState == PieceState.ALIVE; }
    public PieceColor Color() { return _pieceColor; }
    public PieceType PType() { return _pieceType; }

    public void RemoveSelf()
    {
        _pieceState = PieceState.DEAD;
    }
}
