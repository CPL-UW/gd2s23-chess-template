using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// chess x = a-h, y = 1-8
public class BoardManager : MonoBehaviour
{
    public List<Piece> p1Pieces;
    public List<Piece> p2Pieces;
    // public GameObject piecePrefab;

    // void MakeNewPieceFromSprite(Sprite piece)
    // {
    //     var newPiece = Instantiate(piecePrefab, transform);
    //     newPiece.GetComponent<SpriteRenderer>().sprite = piece;
    //     Debug.Log(newPiece.GetComponent<SpriteRenderer>().sprite.texture);
    //     newPiece.transform.Translate(Random.Range(-3,3), Random.Range(0, 3), 0);
    //     newPiece.name = piece.name;
    // }
    
    void Start()
    {
        // foreach (var piece in p1Pieces)
        // {
        //     
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
