using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece

{
    public override List<Vector2Int> GetavailableMoves(ref ChessPiece[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>(); ;

        //single push
        int[] x = new int[] { 1, 1, -1, -1, 0, 0, 1, -1 };
        int[] y = new int[] { 1, -1, 1, -1, 1, -1, 0, -0 };

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

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> movelist, ref List<Vector2Int> availablemoves)
    {
        SpecialMove r = SpecialMove.None;
        var kingmove = movelist.Find(m => m[0].x == 4 && m[0].y == ((team == 0) ? 0 : 7));
        var leftrookmove = movelist.Find(m => m[0].x == 0 && m[0].y == ((team == 0) ? 0 : 7));
        var rightrookmove = movelist.Find(m => m[0].x == 7 && m[0].y == ((team == 0) ? 0 : 7));

        if (kingmove == null && currentx == 4)
        {
            if (team == 0)
            {
                if (leftrookmove == null)
                {
                    if (board[0, 0].team == 0)
                    {
                        if (board[3, 0] == null && board[2, 0] == null && board[1, 0] == null)
                        {
                            availablemoves.Add(new Vector2Int(2, 0));
                            r = SpecialMove.Castling;
                        }
                    }
                }
                if (rightrookmove == null)
                {
                    if (board[7, 0].team == 0)
                    {
                        if (board[6, 0] == null && board[5, 0] == null)
                        {
                            availablemoves.Add(new Vector2Int(6, 0));
                            r = SpecialMove.Castling;
                        }
                    }
                }
            }
            else
            {
                if (leftrookmove == null)
                {
                    if (board[0, 7].team == 1)
                    {
                        if (board[3, 7] == null && board[2, 7] == null && board[1, 7] == null)
                        {
                            availablemoves.Add(new Vector2Int(2, 7));
                            r = SpecialMove.Castling;
                        }
                    }
                }
                if (rightrookmove == null)
                {
                    if (board[7, 7].team == 1)
                    {
                        if (board[6, 7] == null && board[5, 7] == null)
                        {
                            availablemoves.Add(new Vector2Int(6, 7));
                            r = SpecialMove.Castling;
                        }
                    }
                }
            }
        }
        return r;
    }
}
