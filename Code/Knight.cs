using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetavailableMoves(ref ChessPiece[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>(); ;

        //single push
        int[] x = new int[] { 1, 1, -1, -1, 2, 2, -2, -2 };
        int[] y = new int[] { 2, -2, 2, -2, 1, -1, 1, -1 };

        for (int i = 0; i < 8; i++)
        {
            int newx = currentx + x[i];
            int newy = currenty + y[i];
            if (newx >= 0 && newx < tilecountx && newy >= 0 && newy < tilecounty)
            {
                if (board[newx, newy] == null || (board[newx, newy].team != team))
                {
                    r.Add(new Vector2Int(newx, newy));
                }
            }
        }

        return r;
    }
}
