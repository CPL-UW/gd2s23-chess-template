using UnityEngine;
using static ChessInfo;


public class Piece :  MonoBehaviour 
{
    public PieceType pieceType;
    public PieceColor pieceColor;
    public PieceState pieceState;
    public int cx;
    public int cy;
    public int pieceID;
    public PieceInfo pieceInfo;
    
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

    public void SetXY(int x, int y)
    {
        cx = x; cy = y;
    }
    
    
    public int X() { return cx; }
    public int Y() { return cy; }

    public bool Alive() { return pieceState == PieceState.ALIVE; }
    public PieceColor Color() { return pieceColor; }
    public PieceType PType() { return pieceType; }

    public void RemoveSelf()
    {
        // Debug.Log($"RemoveSelf: {pieceColor} {pieceType} at {cx},{cy}");
        if (pieceState != PieceState.ALIVE) return;
        pieceState = PieceState.DEAD;
        GetComponent<SpriteRenderer>().enabled = false;
        SetXY(-1,-1);
    }
    
    void Start()
    {
        name = $"{pieceColor}_{pieceType}_{cx}_{cy}";
        pieceState = PieceState.ALIVE;
        pieceID = Random.Range(10000000, 100000000);
    }
    
}
