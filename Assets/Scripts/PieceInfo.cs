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
    public string LocID();
}

public class PieceInfo : IPieceData
{
    private PieceType _pieceType;
    private PieceColor _pieceColor;
    private PieceState _pieceState;
    private int _cx;
    private int _cy;
    private int _pieceID;

    public PieceInfo(IPieceData pieceData)
    {
        CopyInfo(pieceData);
    }

    public PieceInfo()
    {
    }

    public void CopyInfo(IPieceData other)
    {
        _pieceType = other.PType();
        _pieceColor = other.Color();
        _pieceState = other.Alive() ? PieceState.ALIVE : PieceState.DEAD;
        _cx = other.X();
        _cy = other.Y();
        _pieceID = other.PieceID();
    }

    public void SetXY(int x, int y) { _cx = x; _cy = y; }
    
    public int X() { return _cx; }
    public int Y() { return _cy; }
    public int PieceID() { return _pieceID; }

    public bool Alive() { return _pieceState == PieceState.ALIVE; }
    public PieceColor Color() { return _pieceColor; }
    public PieceType PType() { return _pieceType; }

    public string LocID()
    {
        var output = PType().ToString();
        output += (char)('a' + (X() - 1));
        output += Y().ToString();
        return output;
    }
    
    public void RemoveSelf()
    {
        _pieceState = PieceState.DEAD;
        SetXY(-1,-1);
    }
}
