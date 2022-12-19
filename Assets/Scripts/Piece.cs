using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChessInfo;

public class Piece : MonoBehaviour
{
    public PieceType pieceType;
    public PieceColor pieceColor;
    public PieceState pieceState;
    public int cx;
    public int cy;
    public int pieceID;

    // public Piece(Piece other)
    // {
    //     
    //     pieceType = other.pieceType;
    //     pieceColor = other.pieceColor;
    //     pieceState = other.pieceState;
    //     cx = other.cx;
    //     cy = other.cy;
    // }

    public void SetXY(int x, int y)
    {
        cx = x; cy = y;
    }

    public void RemoveSelf()
    {
        Debug.Log($"{pieceColor} {pieceType} at {cx},{cy} removed");
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
