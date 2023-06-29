using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;


public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class ChessBoard : MonoBehaviour
{

    bool isWhiteTurn;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private ChessPiece currentlyDragging;
    GameObject indexObject;
    [SerializeField] private Material tileMaterial;
    MixedRealityPose pose;
    MixedRealityPose pose2;
    private GameObject[,] tiles;//2d chess board
    private Vector2Int currentHover;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float tilesize = 0.1f;
    [SerializeField] private float yoffset = 0.2f;
    [SerializeField] private GameObject victoryscreen;
    private bool inputmouse = false;
    [SerializeField] private Material[] teamMaterial;
    private ChessPiece[,] chessPieces;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Vector2Int[]> moveslist = new List<Vector2Int[]>();
    private SpecialMove specialmove;
    bool wr1 = false;
    bool wr2 = false;
    bool br1 = false;
    bool br2 = false;
    bool[] c = { false, false };

    
    // Start is called before the first frame update
    private void Awake()
    {
        isWhiteTurn = true;
        //Debug.Log(tilesize);
        GenerateAllTiles(tilesize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPiece();
        PositionAllPieces();
    }
    // Update is called once per frame
    private void Update()
    {
        if (isWhiteTurn)
        {
            for (int i = 0; i < TILE_COUNT_X; i++)
            {
                for (int j = 0; j < TILE_COUNT_Y; j++)
                {
                    if (chessPieces[i, j] != null)
                    {
                        if (chessPieces[i, j].team == 1)
                        {
                            chessPieces[i, j].GetComponent<MeshCollider>().enabled = false;
                        }
                        else
                        {
                            chessPieces[i, j].GetComponent<MeshCollider>().enabled = true;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < TILE_COUNT_X; i++)
            {
                for (int j = 0; j < TILE_COUNT_Y; j++)
                {
                    if (chessPieces[i, j] != null)
                    {
                        if (chessPieces[i, j].team == 0)
                        {
                            chessPieces[i, j].GetComponent<MeshCollider>().enabled = false;

                        }
                        else
                        {
                            chessPieces[i, j].GetComponent<MeshCollider>().enabled = true;
                        }
                    }
                }
            }
        }
        if (isWhiteTurn)
        {
            Ray ray;
            RaycastHit info;
            if (InputRayUtils.TryGetHandRay(Handedness.Right, out ray))
            {
                if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
                {
                    Vector2Int hitposition = LookupTileIndex(info.collider.gameObject);
                    //Debug.Log(hitposition);

                    if (currentHover == -Vector2Int.one)
                    {
                        //Assign the value
                        currentHover = hitposition;

                        tiles[hitposition.x, hitposition.y].layer = LayerMask.NameToLayer("Hover");
                    }
                    //if already in the hovering a tile
                    if (currentHover != hitposition)
                    {

                        tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                        currentHover = hitposition;
                        tiles[hitposition.x, hitposition.y].layer = LayerMask.NameToLayer("Hover");
                    }



                    if (chessPieces[hitposition.x, hitposition.y] != null && chessPieces[hitposition.x, hitposition.y].onclick && currentlyDragging == null)
                    {

                        if ((chessPieces[hitposition.x, hitposition.y].team == 0 && isWhiteTurn) || (chessPieces[hitposition.x, hitposition.y].team == 1 && !isWhiteTurn))
                        {
                            currentlyDragging = chessPieces[hitposition.x, hitposition.y];
                            availableMoves = currentlyDragging.GetavailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                            specialmove = currentlyDragging.GetSpecialMove(ref chessPieces, ref moveslist, ref availableMoves);
                            preventcheck();
                            HighlightTiles();
                            inputmouse = true;
                        }
                    }
                    // Debug.Log(hitposition);
                    bool flag = false;
                    for (int i = 0; i < TILE_COUNT_X; i++)
                    {
                        for (int j = 0; j < TILE_COUNT_Y; j++)
                        {
                            if (chessPieces[i, j] != null && chessPieces[i, j].onclick)
                                flag = true;
                        }
                    }
                    inputmouse = flag;
                    if (currentlyDragging != null && !inputmouse)
                    {
                        Vector2Int previousposition = new Vector2Int(currentlyDragging.currentx, currentlyDragging.currenty);
                        bool validMove = MoveTo(currentlyDragging, hitposition.x, hitposition.y);

                        if (!validMove)
                        {
                            currentlyDragging.SetPosition(GetTileCenter(previousposition.x, previousposition.y));
                        }
                        else
                        {
                            if (currentlyDragging.type == ChessPieceType.Rook)
                            {
                                if(previousposition.x==0 && previousposition.y == 0)
                                {
                                    wr1 = true;
                                }
                                if(previousposition.x==7 && previousposition.y == 0)
                                {
                                    wr2 = true;
                                }
                            }
                        }

                        currentlyDragging = null;
                        RemoveHighlightTiles();
                    }
                }
                else
                {
                    if (currentHover != -Vector2Int.one)
                    {
                        tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                        currentHover = -Vector2Int.one;
                    }
                }
            }
            else if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
        else
        {
            string fen = FENgenerator();
           // Debug.Log(fen);
            string s = GetBestMove(fen);
            string move = "";
            int id = 9;
            while(s[id]!=' ')
            {
                move = move + s[id];
                id++;
            }
            int a = move[0] - 'a';
            int b = move[1] - '1';
            int c = move[2] - 'a';
            int d = move[3] - '1';

            Debug.Log(chessPieces[a, b].type);

            if (move.Length == 5)
            {
                chessPieces[a, b].SetScale(new Vector3(0, 0, 0));
                chessPieces[a, b] = null;
                if (chessPieces[c, d] != null)
                {
                    chessPieces[c, d].SetScale(new Vector3(0, 0, 0));
                    chessPieces[c, d] = null;
                }
                if (move[4] == 'q')
                {
                    chessPieces[c, d] = SpawnSinglePiece(ChessPieceType.Queen, 1);
                    PositionSinglePiece(c, d);
                }
                else if (move[4] == 'r')
                {
                    chessPieces[c, d] = SpawnSinglePiece(ChessPieceType.Rook, 1);
                    PositionSinglePiece(c, d);
                }
                else if (move[4] == 'b')
                {
                    chessPieces[c, d] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
                    PositionSinglePiece(c, d);
                }
                else
                {
                    chessPieces[c, d] = SpawnSinglePiece(ChessPieceType.Knight, 1);
                    PositionSinglePiece(c, d);
                }

            }
            else
            {
                if (chessPieces[a, b].type == ChessPieceType.King)
                {
                    if (Math.Abs(c-a)>1)
                    {
                        specialmove = SpecialMove.Castling;
                    }
                }
                if (chessPieces[a, b].type == ChessPieceType.Pawn)
                {
                    if(Math.Abs(c-a)>1 && Math.Abs(d-b)>1 && chessPieces[c, d] == null)
                    {
                        specialmove = SpecialMove.EnPassant;
                    }
                }
                if (chessPieces[a,b].type == ChessPieceType.Rook)
                {
                    if (a == 0 && b == 7)
                    {
                        br1 = true;
                    }
                    if (a == 7 && a == 7)
                    {
                        br2 = true;
                    }
                }
                ChessPieceType newt = chessPieces[a, b].type;
                chessPieces[a, b].SetScale(new Vector3(0, 0, 0));
                chessPieces[a, b] = null;
                if (chessPieces[c, d] != null)
                {
                    chessPieces[c, d].SetScale(new Vector3(0, 0, 0));
                    chessPieces[c, d] = null;
                }
                chessPieces[c, d] = SpawnSinglePiece(newt, 1);
                PositionSinglePiece(c, d);
            }
            isWhiteTurn = !isWhiteTurn;
        }
    }

    string GetBestMove(string forsythEdwardsNotationString)
    {
        var p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "Assets/stockfish_15.1_win_x64_popcnt/stockfish_15.1_win_x64_popcnt/stockfish-windows-2022-x86-64-modern.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        string setupString = "position fen " + forsythEdwardsNotationString;
        p.StandardInput.WriteLine(setupString);

        // Process for 5 seconds
        //string processString = "go movetime 5000";

        // Process 20 deep
        string processString = "go depth 20";

        p.StandardInput.WriteLine(processString);

        string bestMoveInAlgebraicNotation = p.StandardOutput.ReadLine();
        while(bestMoveInAlgebraicNotation.Length>=8 && bestMoveInAlgebraicNotation.Substring(0,8)!="bestmove" )
            bestMoveInAlgebraicNotation = p.StandardOutput.ReadLine();
        p.Close();

        return bestMoveInAlgebraicNotation;
    }
string FENgenerator() {
    string returner = "";
    for(int j = TILE_COUNT_X-1;j>=0; j--)
    {
            int iter = 0;
            for (int i = 0; i < TILE_COUNT_Y; i++)
            {
                if (chessPieces[i, j] == null)
                {
                    iter++;
                }
                else
                {
                    if (chessPieces[i, j].team == 0)
                    {
                        if (chessPieces[i, j].type == ChessPieceType.Bishop)
                        {
                            if (iter == 0)
                                returner = returner + "B";
                            else
                            {
                                returner = returner + (char)(iter + '0') + "B";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.Knight)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "N";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "N";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.Rook)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "R";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "R";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.Queen)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "Q";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "Q";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "K";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "K";
                            }
                            iter = 0;
                        }
                        else
                        {
                            if (iter == 0)
                            {
                                returner = returner + "P";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "P";
                            }
                            iter = 0;
                        }
                    }
                    else
                    {
                        if (chessPieces[i, j].type == ChessPieceType.Bishop)
                        {
                            if (iter == 0)
                                returner = returner + "b";
                            else
                            {
                                returner = returner + (char)(iter + '0') + "b";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.Knight)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "n";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "n";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.Rook)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "r";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "r";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.Queen)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "q";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "q";
                            }
                            iter = 0;
                        }
                        else if (chessPieces[i, j].type == ChessPieceType.King)
                        {
                            if (iter == 0)
                            {
                                returner = returner + "k";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "k";
                            }
                            iter = 0;
                        }
                        else
                        {
                            if (iter == 0)
                            {
                                returner = returner + "p";
                            }
                            else
                            {
                                returner = returner + (char)(iter + '0') + "p";
                            }
                            iter = 0;
                        }
                    }
                }
            }
            if (iter == 0)
                returner = returner + "/";
            else
                returner = returner + (char)(iter + '0') + "/";
    }
        returner = returner.Substring(0, returner.Length - 1);
        returner = returner + " ";
        if (c[0])
        {
            returner = returner + "-";
        }
        else
        {
            if (!wr1)
                returner = returner + "K";
            if(!wr2)
                returner = returner + "Q";
        }

        if (
            c[1])
        {
            returner = returner + "-";
        }
        else
        {
            if (!br1)
                returner = returner + "k";
            if (!br2)
                returner = returner + "q";
        }
        Debug.Log(returner);
        return returner;
}

    private void GenerateAllTiles(float tilesize, int tilecountx, int tilecounty)
    {
        tiles = new GameObject[tilecountx, tilecounty];
        for (int x = 0; x < tilecountx; x++)
        {
            for (int y = 0; y < tilecounty; y++)
            {
                tiles[x, y] = GenerateSingleTile(tilesize, x, y);

            }
        }

    }

    private GameObject GenerateSingleTile(float tilesize, int x, int y)
    {
        GameObject tileobject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileobject.transform.parent = transform;//chessboard transform(basically if we move the board the tiles as well move0
        Mesh mesh = new Mesh();
        tileobject.AddComponent<MeshFilter>().mesh = mesh;
        tileobject.AddComponent<MeshRenderer>().material=tileMaterial;


        Vector3[] vertex = new Vector3[4];
        vertex[0] = new Vector3(x * tilesize -0.05f, -0.5f, 1+y * tilesize - 0.05f);
        vertex[1] = new Vector3(x * tilesize - 0.05f, -0.5f, 1+(y + 1) * tilesize - 0.05f);
        vertex[2] = new Vector3((x + 1) * tilesize - 0.05f, -0.5f,1+ y * tilesize - 0.05f);
        vertex[3] = new Vector3((x + 1) * tilesize - 0.05f, -0.5f, 1+(y + 1) * tilesize - 0.05f);
        
        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 }; // creates  triangle . triangle 1 = v0 v1 v2 and triangle 2 = v1 v3 v2 which basically gives the square
        mesh.vertices = vertex;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
        tileobject.layer = LayerMask.NameToLayer("Tile");
        tileobject.AddComponent<BoxCollider>();
        return tileobject;
    }

    private Vector2Int LookupTileIndex(GameObject hitinfo)
    {
        for(int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++) {
                if (tiles[i, j] == hitinfo)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return -Vector2Int.one;
    }

    private void SpawnAllPiece()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        int whiteteam = 0, blackteam = 1;
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteteam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteteam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteteam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteteam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteteam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteteam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteteam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteteam);

        
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteteam);
        }
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackteam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackteam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackteam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackteam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackteam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackteam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackteam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackteam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackteam);
        }
    }

    

    private void PositionAllPieces()
    {
        for(int i = 0; i < TILE_COUNT_X; i++)
        {
            for(int j = 0; j < TILE_COUNT_Y; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    PositionSinglePiece(i,j, true);
                }
            }
        }
    }

    private void PositionSinglePiece(int x,int y,bool force = false)
    {
        chessPieces[x, y].currentx = x;
        chessPieces[x, y].currenty = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x,y));
    }

    private Vector3 GetTileCenter(int x,int y)
    {
        return new Vector3(x * tilesize , -0.7f + yoffset, 1 + y * tilesize );
    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterial[team];
        return cp;
    }

   private bool MoveTo(ChessPiece cp,int x,int y)
    {
        if (!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
        {
            return false;
        }
        Vector2Int previousPosition = new Vector2Int(cp.currentx, cp.currenty);
       if (chessPieces[x, y] != null)
        {
            ChessPiece ocp = chessPieces[x, y];
            if (cp.team == ocp.team)
                return false;
            else
            {
                if (ocp.type == ChessPieceType.King)
                {
                    CheckMate(cp.team);
                }
                chessPieces[x, y].SetScale(new Vector3(0, 0, 0));
                chessPieces[x, y] = null;
            }
        }
        chessPieces[x, y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;
        
        PositionSinglePiece(x, y);
        isWhiteTurn = !isWhiteTurn;
        moveslist.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });
        ProcessSpecialMove();
        if (checkforcheckmate())
            CheckMate(cp.team);
        return true;
    }

    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }

    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningteam)
    {
        if (winningteam == 0)
        {
            victoryscreen.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            victoryscreen.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void ProcessSpecialMove()
    {
        if (specialmove == SpecialMove.EnPassant)
        {
            var newMove = moveslist[moveslist.Count - 1];
            ChessPiece mypawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetpawnposition = moveslist[moveslist.Count - 2];
            ChessPiece enemypawn = chessPieces[targetpawnposition[1].x, targetpawnposition[1].y];
            if (mypawn.currentx == enemypawn.currentx)
            {
                if (mypawn.currenty == enemypawn.currenty + 1 || (mypawn.currenty == enemypawn.currenty - 1))
                {

                    enemypawn.SetScale(new Vector3(0, 0, 0));
                    chessPieces[targetpawnposition[1].x, targetpawnposition[1].y] = null;
                }
            }

        }
        if (specialmove == SpecialMove.Promotion)
        {
            var lastmove = moveslist[moveslist.Count - 1];
            ChessPiece targetpawn = chessPieces[lastmove[1].x, lastmove[1].y];
            if (targetpawn.type == ChessPieceType.Pawn)
            {
                Debug.Log("promotion");
                if (targetpawn.team == 0 && lastmove[1].y == 7)
                {
                    targetpawn.SetScale(new Vector3(0, 0, 0));
                    chessPieces[lastmove[1].x, lastmove[1].y] = null;
                    ChessPiece newqueen = SpawnSinglePiece(ChessPieceType.Queen, 0);
                    chessPieces[lastmove[1].x, lastmove[1].y] = newqueen;
                    PositionSinglePiece(lastmove[1].x, lastmove[1].y);
                }
                if (targetpawn.team == 1 && lastmove[1].y == 0)
                {
                    targetpawn.SetScale(new Vector3(0, 0, 0));
                    chessPieces[lastmove[1].x, lastmove[1].y] = null;
                    ChessPiece newqueen = SpawnSinglePiece(ChessPieceType.Queen, 1);
                    chessPieces[lastmove[1].x, lastmove[1].y] = newqueen;
                    PositionSinglePiece(lastmove[1].x, lastmove[1].y);
                }
            }
        }
        else if (specialmove == SpecialMove.Castling)
        {
            Debug.Log("CASTLING");
            var lastmove = moveslist[moveslist.Count - 1];
            //left castle
            if (lastmove[1].x == 2 && (lastmove[1].y == 7 || lastmove[1].y == 0))
            {
                ChessPiece rook = chessPieces[0, lastmove[1].y];
                c[rook.team] = true;
                chessPieces[3, lastmove[1].y] = rook;
                PositionSinglePiece(3, lastmove[1].y);
                chessPieces[0, lastmove[1].y] = null;

                ;
                


            }
            if (lastmove[1].x == 6 && (lastmove[1].y == 7 || lastmove[1].y == 0))
            {
                ChessPiece rook = chessPieces[7, lastmove[1].y];
                c[rook.team] = true;
                chessPieces[5, lastmove[1].y] = rook;
                PositionSinglePiece(5, lastmove[1].y);
                chessPieces[7, lastmove[1].y] = null;
            }
        }
    }

    private void preventcheck()
    {
        ChessPiece targetking = null;
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                if (chessPieces[i, j] != null)
                    if (chessPieces[i, j].type == ChessPieceType.King)
                    {
                        if (chessPieces[i, j].team == currentlyDragging.team)
                        {
                            targetking = chessPieces[i, j];
                        }
                    }
            }
        }
        SimulateMoveforsinglepiece(currentlyDragging, ref availableMoves, targetking);
    }

    private void SimulateMoveforsinglepiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetking)
    {
        int actualx = cp.currentx;
        int actualy = cp.currenty;

        List<Vector2Int> movestoremove = new List<Vector2Int>();

        for (int i = 0; i < moves.Count; i++)
        {
            int simx = moves[i].x;
            int simy = moves[i].y;
            Vector2Int kingpositionthissim = new Vector2Int(targetking.currentx, targetking.currenty);
            if (cp.type == ChessPieceType.King)
                kingpositionthissim = new Vector2Int(simx, simy);
            //shallow copy of chess board
            ChessPiece[,] simulation = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPiece> simattackingpieces = new List<ChessPiece>();
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].team != cp.team)
                            simattackingpieces.Add(simulation[x, y]);

                    }
                }
            }
            simulation[actualx, actualy] = null;
            cp.currentx = simx;
            cp.currenty = simy;
            simulation[simx, simy] = cp;

            var deadPiece = simattackingpieces.Find(c => c.currentx == simx && c.currenty == simy);
            if (deadPiece != null)
                simattackingpieces.Remove(deadPiece);

            //get all the simulated attacking pieces moves 

            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int a = 0; a < simattackingpieces.Count; a++)
            {
                var pieceMoves = simattackingpieces[a].GetavailableMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
                for (int b = 0; b < pieceMoves.Count; b++)
                {
                    simMoves.Add(pieceMoves[b]);
                }
            }
            if (ContainsValidMove(ref simMoves, kingpositionthissim))
            {
                movestoremove.Add(moves[i]);
            }

            cp.currentx = actualx;
            cp.currenty = actualy;
        }



        for (int i = 0; i < movestoremove.Count; i++)
            moves.Remove(movestoremove[i]);
    }

    private bool checkforcheckmate()
    {
        var lastmove = moveslist[moveslist.Count - 1];
        int targetteam = (chessPieces[lastmove[1].x, lastmove[1].y].team == 0) ? 1 : 0;

        List<ChessPiece> attackingpieces = new List<ChessPiece>();
        List<ChessPiece> defendingpieces = new List<ChessPiece>();

        ChessPiece targetking = null;
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    if (chessPieces[i, j].team == targetteam)
                    {
                        defendingpieces.Add(chessPieces[i, j]);
                        if (chessPieces[i, j].type == ChessPieceType.King)
                            targetking = chessPieces[i, j];
                    }
                    else
                    {
                        attackingpieces.Add(chessPieces[i, j]);
                    }
                }
            }
        }
        List<Vector2Int> currentlyAvailableMoves = new List<Vector2Int>();
        for (int i = 0; i < attackingpieces.Count; i++)
        {
            var pieceMoves = attackingpieces[i].GetavailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            for (int b = 0; b < pieceMoves.Count; b++)
            {
                currentlyAvailableMoves.Add(pieceMoves[b]);
            }
        }
        //check if the king is in check
        if (ContainsValidMove(ref currentlyAvailableMoves, new Vector2Int(targetking.currentx, targetking.currenty)))
        {
            for (int i = 0; i < defendingpieces.Count; i++)
            {
                List<Vector2Int> defendingmoves = defendingpieces[i].GetavailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                SimulateMoveforsinglepiece(defendingpieces[i], ref defendingmoves, targetking);
                if (defendingmoves.Count != 0)
                    return false;
            }
            return true;
        }
        return false;
    }

}
