using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreate : MonoBehaviour
{
    public int mapX;
    public int mapY;
    public float addX;
    public Grid grid;
    public float addY;
    int[,] arr;
    public int []spawnMob = new int[5];
    public Tilemap wallTilemap;
    public Tilemap dirtTilemap;
    public Tilemap[] wallCrossTilemap = new Tilemap[4];
    public Tilemap backgroundTilemap;
    public TileBase []norTile = new TileBase[10];
    public TileBase []wallTile = new TileBase[10];
    public TileBase[] dirtTile = new TileBase[3];
    public TileBase[] groundTile = new TileBase[20];
    public TileBase HardwallTile;
    public TileBase sadariTile;
    int componentcnt = 0;
    public  List<Vector2Int> v = new List<Vector2Int>();
    Queue<Vector2Int> q = new Queue<Vector2Int>();

    public int[,] Arr { get => arr; set => arr = value; }

    // Start is called before the first frame update
    void Awake()
    {
        CreateMap();
    }

    public void CreateMap()
    {
        Arr = new int[mapX, mapY];
        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                Arr[i, j] = 1;
                if (i == 0 || j == 0 || i == mapX - 1 || j == mapY - 1)
                {
                    Arr[i, j] = 0;
                }
            }
        }
        CreateWall();
        DyedWall();
        CCC();
        Connect1();
        DyedWall();
        SquareWall();
        DrawTile();
        PlayerInstantiate();
        //SpawnMonster();
    }


    void CreateWall()
    {
        for (int i = 0; i < mapX * mapY / 100 * 60; i++)
        {
            while (true)
            {
                
                int x = UnityEngine.Random.Range(1, mapX); int y = UnityEngine.Random.Range(1, mapY);
                if (Arr[x, y] == 1)
                {
                    Debug.Log(x + " " + y);
                    Arr[x, y] = 0;
                    break;
                }

            }
        }
    }

    void DyedWall()
    {
        int[] dx = { 1, 1, 1, 0, 0, 0, -1, -1, -1 };
        int[] dy = { 1, 0, -1, 1, 0, -1, 1, 0, -1 };
        int z_cnt = 0;
        for (int i = 1; i < mapX - 1; i++)
        {
            for (int j = 1; j < mapY - 1; j++)
            {
                z_cnt = 0;
                for (int xcnt = 0; xcnt < 9; xcnt++)
                {
                    if (Arr[i + dx[xcnt], j + dy[xcnt]] == 0) z_cnt++;
                }
                if (z_cnt > 4)
                {
                    Arr[i, j] = 0;
                }

                else
                {
                    if(Arr[i, j] == 99)
                    { }
                    else
                    {
                        Arr[i, j] = 1;
                    }
                    Debug.Log(i + " " + j);
                }
            }
        }
    }

    void CCC()
    {
        Vector2Int nd = new Vector2Int(); ;
        
        for (int j = 0; j < mapY; j++)
            {
            for (int i = 0; i < mapX; i++)
            {
                if (Arr[i, j] == 1)
                {
                    Debug.Log("CCC");
                    BFS(i, j);
                    nd.x = i;
                    nd.y = j;
                    v.Add(nd);
                }

            }
        }
    }

    void BFS(int x, int y)
    {
        Vector2Int now = new Vector2Int();
        Vector2Int next = new Vector2Int();
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        now.x = x;
        now.y = y;
        q.Enqueue(now);
        //printf("%d %d", x, y);
        while (q.Count != 0)
        {
            now = q.Peek();
            q.Dequeue();
            Arr[(int)now.x, (int)now.y] = componentcnt + 2;
            for (int i = 0; i < 4; i++)
            {
                next.x = now.x + dx[i];
                next.y = now.y + dy[i];
                if (next.x > 0 && next.x < mapX && next.y >= 0 && next.y < mapY && Arr[(int)next.x, (int)next.y] == 1)
                {
                    q.Enqueue(next);
                    Arr[(int)next.x, (int)next.y] = componentcnt + 2;
                    Debug.Log(Arr[(int)next.x, (int)next.y]);
                }
            }
        }
        componentcnt++;
    }

    void Connect1()
    {
        int vl = v.Count;
        Vector2Int st, en;
        int len, dx, dy;
        for (int tt = 0; tt < Mathf.Min(3, vl); tt++)
        {
           if (tt + 1 >= vl)
                break;
            st = v[tt];
            en = v[tt + 1];
            len =Mathf.Abs(st.x - en.x) + Mathf.Abs(st.y - en.y);
            if (en.x - st.x < 0) dx = -1;
            else if (en.x - st.x > 0) dx = 1;
            else dx = 0;
            if (en.y - st.y < 0) dy = -1;
            else if (en.y - st.y > 0) dy = 1;
            else dy = 0;
            for (int i = 0; i < len + 2; i++)
            {
                if (st.y == en.y && i < len)
                {
                    st.x += dx;
                }
                else if (st.x == en.x && i < len)
                {
                    st.y += dy; 
                }
                else if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    st.x += dx;
                }
                else
                {
                    st.y += dy;
                }
                if (st.x > 0 && st.y > 0 && st.x < mapX - 1 && st.y < mapY - 1)
                {
                    Debug.Log(tt + 2 + "St : " + st);
                    Arr[st.x, st.y] = tt + 2;
                    if (st.x + 1 != mapX && st.y + 1 != mapY && st.y - 1 != 0)
                    {
                            Arr[st.x + 1, st.y] = 1;
                            Arr[st.x, st.y + 1] = 1;
                            Arr[st.x, st.y - 1] = 1;
                        Debug.Log("AY!");
                    }
                }
                else
                    break;
            }
        }

        for (int tt = 0; tt < vl - 3; tt++)
        {
           if (tt + 3 >= vl)
                break;
            st = v[tt];
            en = v[tt + 3];
            len = Mathf.Abs(st.x - en.x) + Mathf.Abs(st.y - en.y);
            if (en.x - st.x < 0) dx = -1;
            else if (en.x - st.x > 0) dx = 1;
            else dx = 0;
            if (en.y - st.y < 0) dy = -1;
            else if (en.y - st.y > 0) dy = 1;
            else dy = 0;
            for (int i = 0; i < len + 2; i++)
            {
                if (st.y == en.y && i < len)
                {
                    st.x += dx;
                }
                else if (st.x == en.x && i < len)
                {
                    st.y += dy; 
                }
                else if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    st.x += dx;
                }
                else
                {
                    st.y += dy; 
                }
                if (st.x > 0 && st.y > 0 && st.x < mapX-1 && st.y < mapY-1)
                {
                    Debug.Log(tt+2+"St : " + st);
                    Arr[st.x, st.y] = tt + 2;
                    if (st.x + 1 != mapX && st.y + 1 != mapY && st.y - 1 != 0)
                    {
                        Arr[st.x + 1, st.y] = 1;
                        Arr[st.x, st.y + 1] = 1;
                        Arr[st.x, st.y - 1] = 1;

                        Debug.Log("AY!");
                    }
                }
                else
                    break;
            }
        }
    }
   

    void DrawTile()
    {
        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                Vector3Int vec3;
                
                if (Arr[i, j] == 0)// 벽타일
                {
                    vec3 = wallTilemap.WorldToCell(new Vector3(i + addX, j + addY, 0));
                    if (Arr[i, j+1] > 0 && Arr[i, j - 1] > 0 && Arr[i+1, j] > 0 && Arr[i-1, j] > 0) // 다 막혀있을 때 
                    {
                        wallTilemap.SetTile(vec3, groundTile[12]);
                    }
                    else if (Arr[i, j + 1] <= 0 && Arr[i, j - 1] > 0 && Arr[i + 1, j] > 0 && Arr[i - 1, j] > 0) // 한 방향만 뚫려있을 때 
                    {
                        wallTilemap.SetTile(vec3, groundTile[20]);
                    }
                    else if (Arr[i, j + 1] > 0 && Arr[i, j - 1] <= 0 && Arr[i + 1, j] > 0 && Arr[i - 1, j] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[6]);
                    }
                    else if (Arr[i, j + 1] > 0 && Arr[i, j - 1] > 0 && Arr[i + 1, j] <= 0 && Arr[i - 1, j] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[17]);
                    }
                    else if (Arr[i, j + 1] > 0 && Arr[i, j - 1] > 0 && Arr[i + 1, j] > 0 && Arr[i - 1, j] <= 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[19]);
                    }
                    else if (Arr[i, j + 1] > 0 && Arr[i, j - 1] > 0 && Arr[i + 1, j] <= 0 && Arr[i - 1, j] <= 0) // 양 옆이 뚫려있을 때
                    {
                        wallTilemap.SetTile(vec3, groundTile[18]);
                    }
                    else if (Arr[i, j + 1] <= 0 && Arr[i, j - 1] <= 0 && Arr[i + 1, j] > 0 && Arr[i - 1, j] > 0) // 위 아래가 뚤려있을 때 
                    {
                        wallTilemap.SetTile(vec3, groundTile[13]);
                    }
                    //
                    if (Arr[i, j + 1] <= 0 && Arr[i + 1, j] <= 0 && Arr[i + 1, j + 1] > 0) // 대각선 연결 
                    {
                        wallCrossTilemap[0].SetTile(vec3, groundTile[10]);
                    }
                    else if (Arr[i, j + 1] <= 0 && Arr[i - 1, j] <= 0 && Arr[i - 1, j + 1] > 0)
                    {
                        wallCrossTilemap[1].SetTile(vec3, groundTile[11]);
                    }
                    if (Arr[i, j - 1] <= 0  && Arr[i + 1, j] <= 0 && Arr[i + 1, j - 1] > 0)
                    {
                        wallCrossTilemap[2].SetTile(vec3, groundTile[3]);
                    }
                    else if (Arr[i, j - 1] <= 0  && Arr[i - 1, j] <= 0 && Arr[i - 1, j - 1] > 0)
                    {
                        wallCrossTilemap[3].SetTile(vec3, groundTile[4]);
                    }
                    //
                    if (Arr[i - 1, j] > 0 && Arr[i + 1, j] <= 0 && Arr[i, j + 1] > 0 && Arr[i, j - 1] <= 0) // ㄱ자 모양들 
                    {
                        wallTilemap.SetTile(vec3, groundTile[0]);
                    }
                    else if (Arr[i + 1, j] > 0 && Arr[i - 1, j] <= 0 && Arr[i, j + 1] > 0 && Arr[i, j - 1] <= 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[2]);
                    }
                    else if (Arr[i - 1, j] > 0 && Arr[i + 1, j] <= 0 && Arr[i, j + 1] <= 0 && Arr[i, j - 1] > 0) // ㄴ자 모양들 
                    {
                        wallTilemap.SetTile(vec3, groundTile[14]);
                    }
                    else if (Arr[i + 1, j] > 0 && Arr[i - 1, j] <= 0 && Arr[i, j + 1] <= 0 && Arr[i, j - 1] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[16]);
                    }
                    else if (Arr[i + 1, j] <= 0 && Arr[i - 1, j] <= 0 && Arr[i, j + 1] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[1]);
                    }
                    else if (Arr[i + 1, j] <= 0 && Arr[i - 1, j] <= 0 && Arr[i, j - 1] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[15]);
                    }
                    else if (Arr[i, j + 1] <= 0 && Arr[i, j - 1] <= 0 && Arr[i + 1, j] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[9]);
                    }
                    else if (Arr[i, j + 1] <= 0 && Arr[i, j - 1] <= 0 && Arr[i - 1, j] > 0)
                    {
                        wallTilemap.SetTile(vec3, groundTile[7]);
                    }
                    int dirtCount = UnityEngine.Random.Range(3, 5);
                    bool dirtCheck = false;
                    for(int h = 1; h<=dirtCount; h++)
                    {
                        if(i+h < mapX )
                        {
                            if (Arr[i+h, j] <= 0 )
                            {

                            }
                            else
                            {
                                dirtCheck = true;
                                break;
                            }
                        }
                        if (i - h >= 0 )
                        {
                            if (Arr[i - h, j] <= 0)
                            {

                            }
                            else
                            {
                                dirtCheck = true;
                                break;
                            }
                        }
                        if ( j + h < mapY)
                        {
                            if (Arr[i, j + h] <= 0)
                            {

                            }
                            else
                            {
                                dirtCheck = true;
                                break;
                            }
                        }
                        if (j - h >= 0)
                        {
                            if (Arr[i, j - h] <= 0)
                            {


                            }
                            else
                            {
                                dirtCheck = true;
                                break;
                            }
                        }
                    }
                    if(dirtCheck)
                           dirtTilemap.SetTile(vec3, dirtTile[0]);
                    else
                        dirtTilemap.SetTile(vec3, dirtTile[1]);
                }
                else if(Arr[i, j] == 1)// 기본타일
                {
                    vec3 = backgroundTilemap.WorldToCell(new Vector3(i + addX, j + addY, 0));
                    backgroundTilemap.SetTile(vec3, norTile[InstantiateManager.Instance.stageNum]);
                }
                else if (Arr[i, j] == -10)// 강철벽.
                {
                    vec3 = wallTilemap.WorldToCell(new Vector3(i + addX, j + addY, 0));
                    dirtTilemap.SetTile(vec3, HardwallTile);
                }
                else// 기본타일
                {
                    vec3 = backgroundTilemap.WorldToCell(new Vector3(i + addX, j + addY, 0));
                    backgroundTilemap.SetTile(vec3, sadariTile);
                }
            }
        }
    }
    void SpawnMonster()
    {
        int mobnum = 0;
        int nowmobnum = 0;
        for (int j = 1; j < mapY-2; j++)
        {
            if (nowmobnum < mobnum)
                j += UnityEngine.Random.Range(3, 5);
            nowmobnum = mobnum;
            for (int i = 1; i < mapX-1; i++)
            {
                if(Arr[i, j] == 0 && Arr[i, j+1] > 0 && Arr[i+1, j] == 0 && Arr[i+1, j + 1] > 0 && Arr[i + 1, j+2] > 0 && Arr[i, j + 2] > 0)
                {
                    GameObject mob =  Instantiate(InstantiateManager.Instance.monsters[0]);
                    mob.transform.position = new Vector3(i + 0.5f, j + 1f);
                    mobnum++;
                    if (mobnum >= 16)
                        return;
                    i += UnityEngine.Random.Range(30, 70);
                }
            }
        }
    }

    void SquareWall()
    {
        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                if (i == 0 || j == 0 || i == mapX - 1 || j == mapY - 1)
                {
                    Arr[i, j] = -10;
                }
            }
        }
    }
    void ConsoleLog()
    {
        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                Debug.Log(i + " " + j + " " + Arr[i, j]);
            }
        }
    }
    void PlayerInstantiate()
    {
        for (int i = 1; i < mapX - 1; i++)
        {
            for (int j = 1; j < mapY - 1; j++)
            {
                if (Arr[i, j] != 0 && Arr[i, j + 1] != 0)
                {
                    GameObject pc = Instantiate(InstantiateManager.Instance.player);
                    pc.transform.position = new Vector3(i + addX, j + addY);
                    return;
                }
            }
        }
    }
}
