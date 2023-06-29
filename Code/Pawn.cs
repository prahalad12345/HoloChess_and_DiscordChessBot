using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetavailableMoves(ref ChessPiece[,] board, int tilecountx, int tilecounty)
    {
        List<Vector2Int> r = new List<Vector2Int>(); ;

        int direction = (team == 0) ? 1 : -1;
        //single push
        if (board[currentx, currenty + direction] == null)
        {
            r.Add(new Vector2Int(currentx, currenty + direction));
        }
        //double push
        if (board[currentx, currenty + direction] == null)
        {
            if (((team == 0 && currenty == 1) || (team == 1 && currenty == 6)) && board[currentx, currenty + direction * 2] == null)
            {
                r.Add(new Vector2Int(currentx, currenty + 2 * direction));
            }
        }

        //kill move 
        if (currentx != tilecountx - 1)
        {
            if (board[currentx + 1, currenty + direction] != null && board[currentx + 1, currenty + direction].team != team)
            {
                r.Add(new Vector2Int(currentx + 1, currenty + direction));
            }
        }

        if (currentx != 0)
        {
            if (board[currentx - 1, currenty + direction] != null && board[currentx - 1, currenty + direction].team != team)
            {
                r.Add(new Vector2Int(currentx - 1, currenty + direction));
            }
        }

        return r;
    }

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> movelist, ref List<Vector2Int> availablemoves)
    {

        int direction = (team == 0) ? 1 : -1;

        if ((team == 0 && currenty == 6) || (team == 1 && currenty == 1))
        {
            return SpecialMove.Promotion;
        }
        if (movelist.Count > 0)
        {
            Vector2Int[] lastmove = movelist[movelist.Count - 1];
            if (board[lastmove[1].x, lastmove[1].y].type == ChessPieceType.Pawn)
            {
                if (lastmove[1].y == currenty && Mathf.Abs(lastmove[0].y - lastmove[1].y) == 2)
                {
                    if (lastmove[1].x == currentx - 1)
                    {
                        availablemoves.Add(new Vector2Int(currentx - 1, currenty + direction));
                        return SpecialMove.EnPassant;
                    }
                    if (lastmove[1].x == currentx + 1)
                    {
                        availablemoves.Add(new Vector2Int(currentx + 1, currenty + direction));
                        return SpecialMove.EnPassant;
                    }
                }
            }
        }
        return SpecialMove.None;
    }
}
