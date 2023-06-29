using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetavailableMoves(ref ChessPiece[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>(); ;

        //single push
        for (int i = currenty - 1; i >= 0; i--)
        {
            if (board[currentx, i] != null)
            {
                if (board[currentx, i].team != team)
                    r.Add(new Vector2Int(currentx, i));
                break;
            }
            else
                r.Add(new Vector2Int(currentx, i));
        }

        for (int i = currenty + 1; i < tilecounty; i++)
        {
            if (board[currentx, i] != null)
            {
                if (board[currentx, i].team != team)
                    r.Add(new Vector2Int(currentx, i));
                break;
            }
            else
                r.Add(new Vector2Int(currentx, i));
        }

        for (int i = currentx - 1; i >= 0; i--)
        {
            if (board[i, currenty] != null)
            {
                if (board[i, currenty].team != team)
                    r.Add(new Vector2Int(i, currenty));
                break;
            }
            else
                r.Add(new Vector2Int(i, currenty));
        }

        for (int i = currentx + 1; i < tilecountx; i++)
        {
            if (board[i, currenty] != null)
            {
                if (board[i, currenty].team != team)
                    r.Add(new Vector2Int(i, currenty));
                break;
            }
            else
                r.Add(new Vector2Int(i, currenty));
        }


        return r;
    }
}
