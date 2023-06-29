using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetavailableMoves(ref ChessPiece[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>(); ;

        //single push
        for (int i = currentx - 1, j = currenty - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (board[i, j] != null)
            {
                if (board[i, j].team != team)
                    r.Add(new Vector2Int(i, j));
                break;
            }
            else
                r.Add(new Vector2Int(i, j));
        }

        for (int i = currentx + 1, j = currenty - 1; i < tilecountx && j >= 0; i++, j--)
        {
            if (board[i, j] != null)
            {
                if (board[i, j].team != team)
                    r.Add(new Vector2Int(i, j));
                break;
            }
            else
                r.Add(new Vector2Int(i, j));
        }

        for (int i = currentx - 1, j = currenty + 1; i >= 0 && j < tilecounty; i--, j++)
        {
            if (board[i, j] != null)
            {
                if (board[i, j].team != team)
                    r.Add(new Vector2Int(i, j));
                break;
            }
            else
                r.Add(new Vector2Int(i, j));
        }

        for (int i = currentx + 1, j = currenty + 1; i < tilecountx && j < tilecounty; i++, j++)
        {
            if (board[i, j] != null)
            {
                if (board[i, j].team != team)
                    r.Add(new Vector2Int(i, j));
                break;
            }
            else
                r.Add(new Vector2Int(i, j));
        }


        return r;
    }
}
