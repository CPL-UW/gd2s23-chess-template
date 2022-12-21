using UnityEngine;
using static ChessInfo;


public class Piece :  MonoBehaviour, IPieceData
{
    public PieceType pieceType;
    public PieceColor pieceColor;
    public PieceState pieceState;
    public int cx;
    public int cy;
    public int pieceID;
    
    private Piece()
    {
    }
    //
    // public Piece(Piece other)
    // {
    //     pieceType = other.pieceType;
    //     pieceColor = other.pieceColor;
    //     pieceState = other.pieceState;
    //     cx = other.cx;
    //     cy = other.cy;
    //     pieceID = Random.Range(10000000, 100000000);
    //     // Debug.Log($"Cloning {other.pieceID} => {pieceID}");
    // }

    public void SetXY(int x, int y) { cx = x; cy = y; }
    
    public int X() { return cx; }
    public int Y() { return cy; }

    public bool Alive() { return pieceState == PieceState.ALIVE; }
    public PieceColor Color() { return pieceColor; }
    public PieceType PType() { return pieceType; }
    public int PieceID()
    {
        return pieceID;
    }

    public void RemoveSelf()
    {
        // Debug.Log($"RemoveSelf: {pieceColor} {pieceType} at {cx},{cy}");
        if (pieceState != PieceState.ALIVE) return;
        pieceState = PieceState.DEAD;
        GetComponent<SpriteRenderer>().enabled = false;
        SetXY(-1,-1);
    }

    public string LocID()
    {
        var output = PType().ToString();
        output += (char)('a' + (X() - 1));
        output += Y().ToString();
        return output;
    }

    void Start()
    {
        name = $"{pieceColor}_{pieceType}_{cx}_{cy}";
        pieceState = PieceState.ALIVE;
        pieceID = Random.Range(10000000, 100000000);
    }
    
}
