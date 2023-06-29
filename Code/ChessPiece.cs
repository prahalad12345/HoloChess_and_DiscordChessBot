using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;


public enum ChessPieceType
{
    None =0,
    Pawn =1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviour , IMixedRealityPointerHandler
{
    public int team;
    public bool onclick=false;
    public int currentx, currenty;
    public ChessPieceType type;
    private Vector3 desiredPosition;
    private Vector3 desiredScale;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("Clicked a Piece");
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        Debug.Log("On Clicking");
        onclick = true;
       // foreach (var source in Microsoft.MixedReality.Toolkit.Input.De
       
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
       // Debug.Log("Draggin the piece");
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Released the piece");
        onclick = false ;
    }

    public virtual void SetPosition(Vector3 position)
    {
        desiredPosition = position;
        transform.position = desiredPosition;
    }

    public virtual void SetScale(Vector3 scale)
    {
        desiredScale = scale;
        transform.localScale = desiredScale;
    }

    public virtual List<Vector2Int> GetavailableMoves(ref ChessPiece[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>(); ;
        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));

        return r;
    }

    public virtual SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> movelist, ref List<Vector2Int> availablemoves)
    {
        return SpecialMove.None;
    }
}
