using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
struct Node
{
    public int x, y;
   // public int X { get => x; set => x = value; }
   // public int Y { get => y; set => y = value; }
}
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
    public int[,] Copy { get => arr; set => arr = value; }
    // Start is called before the first frame update
    void Awake()
    {
        CreateMap();
    }

    public void CreateMap()
    {
        Arr = new int[mapX, mapY];
        Copy = new int[mapX, mapY];
        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                Arr[i, j] = 1; //1이 빈공간, 0이 벽인듯
                if (i == 0 || j == 0 || i == mapX - 1 || j == mapY - 1)
                {
                    Arr[i, j] = 0;
                }
            }
        }
        Node a;
        a.x = 0;
        a.y = 0;
        Node b;
        b.x = mapX - 1;
        b.y = mapY - 1;
        BSP(a, b);
        SquareWall();
        DrawTile();
        PlayerInstantiate();
        //SpawnMonster();
    }

    void BSP(Node st,Node en)
    {
        if ((en.x - st.x) * (en.y - st.y) <= 1000)
        {
            return;
        }
        else
        {
            Node mid;
            mid.x = (st.x + en.x) / 2;
            mid.y = (st.y + en.y) / 2;
            Node nd1, nd2;
            int random = UnityEngine.Random.Range(0,5);
            for(int i = st.x; i <= en.x; i++)
            {
                for(int j = st.y; j <= en.y; j++)
                {
                    if (i == st.x || i == en.x||j==st.y||j==en.y) Arr[i, j] = 0;
                    //Arr[i, j] = 1; 
                }
            }
            if (en.y - st.y > en.x- st.x)
            {
                mid.y = mid.y + random - 2;
                for(int i = st.x; i <= en.x; i++)
                {
                    Arr[i,mid.y] = 0;
                }
                nd1 = st;
                nd2.x = en.x;
                nd2.y = mid.y;
                BSP(nd1, nd2);
                nd1.x = st.x;
                nd1.y = mid.y;
                nd2 =en;
                BSP(nd1, nd2);

            }
            else
            {
                mid.x = mid.x + random - 2;
                for (int i = st.y; i <= en.y; i++)
                {
                    Arr[mid.x,i] = 0;
                }
                nd1 = st;
                nd2.x = mid.x;
                nd2.y = en.y;
                BSP(nd1, nd2);
                nd1.x = mid.x;
                nd1.y = st.y;
                nd2 = en;
                BSP(nd1, nd2);
            }
        }
        return;
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
