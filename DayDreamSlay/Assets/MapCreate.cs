using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Node
{
    public int x, y;
   // public int X { get => x; set => x = value; }
   // public int Y { get => y; set => y = value; }
}
public struct BspNd
{
    public Node nd;
    public int randval;
}
public struct PairNode
{
    public Node first;
    public Node second;
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
    public PairNode []BspTree = new PairNode[210];
    public int componentcnt = 0;

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
        BSP(a, b,1);
        
        for (int i = 2; i < 2 * componentcnt; i+=2)
        {
            Connect(BspTree[i], BspTree[i + 1]);
        }
        //for (int i = 0; i < 50; i++) Cellular_Automata(a, b);


        SquareWall();
        DrawTile();
        PlayerInstantiate();
        //SpawnMonster();
    }

    void BSP(Node st,Node en,int node_num)
    {
        PairNode tmp;
        tmp.first = st;
        //tmp.first.y = st.y;
        tmp.second = en;
        //tmp.second.x = en.y;
        BspTree[node_num] = tmp;
        if ((en.x - st.x) * (en.y - st.y) <= 1200)
        {
            for(int i = st.x; i <= en.x; i++)
            {
                for(int j = st.y; j <= en.y; j++)
                {
                    Arr[i, j] = 0;
                }
            }
            int ranle = UnityEngine.Random.Range(0, 3);
            st.x += ranle;
            ranle = UnityEngine.Random.Range(0, 3);
            st.y += ranle;
            ranle = UnityEngine.Random.Range(0, 3);
            en.x -= ranle;
            ranle = UnityEngine.Random.Range(0, 3);
            en.y -= ranle;
            for (int i = st.x+1; i <= en.x-1; i++)
            {
                for (int j = st.y+1; j <= en.y-1; j++)
                {
                    Arr[i, j] = 1;
                }
            }
            Create_Noise_2(st, en);
            componentcnt++;
            return;
        }
        else
        {
            Node mid;
            mid.x = (st.x + en.x) / 2;
            mid.y = (st.y + en.y) / 2;
            Node nd1, nd2;
            int random = UnityEngine.Random.Range(0,9);
            random -= 3;
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
                mid.y = mid.y + random;
                for(int i = st.x; i <= en.x; i++)
                {
                    Arr[i,mid.y] = 0;
                }
                nd1 = st;
                nd2.x = en.x;
                nd2.y = mid.y;
                BSP(nd1, nd2,node_num*2);
                nd1.x = st.x;
                nd1.y = mid.y;
                nd2 =en;
                BSP(nd1, nd2,node_num*2+1);

            }
            else if (en.y - st.y < en.x - st.x)
            {
                mid.x = mid.x + random;
                for (int i = st.y; i <= en.y; i++)
                {
                    Arr[mid.x,i] = 0;
                }
                nd1 = st;
                nd2.x = mid.x;
                nd2.y = en.y;
                BSP(nd1, nd2,node_num*2);
                nd1.x = mid.x;
                nd1.y = st.y;
                nd2 = en;
                BSP(nd1, nd2,node_num*2+1);
            }
            else
            {
                int casecheck = UnityEngine.Random.Range(0, 2);
                if (casecheck == 0)
                {
                    mid.y = mid.y + random;
                    for (int i = st.x; i <= en.x; i++)
                    {
                        Arr[i, mid.y] = 0;
                    }
                    nd1 = st;
                    nd2.x = en.x;
                    nd2.y = mid.y;
                    BSP(nd1, nd2, node_num * 2);
                    nd1.x = st.x;
                    nd1.y = mid.y;
                    nd2 = en;
                    BSP(nd1, nd2, node_num * 2 + 1);
                }
                else
                {
                    mid.x = mid.x + random;
                    for (int i = st.y; i <= en.y; i++)
                    {
                        Arr[mid.x, i] = 0;
                    }
                    nd1 = st;
                    nd2.x = mid.x;
                    nd2.y = en.y;
                    BSP(nd1, nd2, node_num * 2);
                    nd1.x = mid.x;
                    nd1.y = st.y;
                    nd2 = en;
                    BSP(nd1, nd2, node_num * 2 + 1);
                }
            }
        }
        return;
    }

   void Create_Noise_1(Node st,Node en) 
    {
        int CAper = 52;
        //Debug.Log(CAper);
        int xrand = 0, yrand = 0;
        int diff_len = en.x - 5 - st.x;
        int forl = (5 * diff_len * CAper) / 100;
        for(int i =0; i <forl; i++)
        {
            while (true)
            {
                xrand = UnityEngine.Random.Range(0, diff_len);
                yrand = UnityEngine.Random.Range(0, 5);
                if (Arr[st.x + xrand, st.y + yrand] == 0) continue;
                else
                {
                    Arr[st.x + xrand, st.y + yrand] = 0;
                    break;
                }
            }
        }
        for (int i = 0; i < forl; i++)
        {
            while (true)
            {   
                xrand = UnityEngine.Random.Range(0, diff_len);
                yrand = UnityEngine.Random.Range(0, 5);
                if (Arr[st.x + 5 + xrand, en.y - 5 + yrand] == 0) continue;
                else
                {
                    Arr[st.x + 5 + xrand, en.y - 5 + yrand] = 0;
                    break;
                }
            }
        }
        diff_len = en.y - 5 - st.y;
        forl = (5 * diff_len * CAper) / 100;
        for (int i = 0; i < forl; i++)
        {
            while (true)
            {
                xrand = UnityEngine.Random.Range(0, 5);
                yrand = UnityEngine.Random.Range(0, diff_len);
                if (Arr[en.x - 5 + xrand, st.y + yrand] == 0) continue;
                else
                {
                    Arr[en.x - 5 + xrand, st.y + yrand] = 0;
                    break;
                }
            }
        }
        for (int i = 0; i < forl; i++)
        {
            while (true)
            {
                xrand = UnityEngine.Random.Range(0, 5);
                yrand = UnityEngine.Random.Range(0, diff_len);
                if (Arr[st.x + xrand, st.y + 5 + yrand] == 0) continue;
                else
                {
                    Arr[st.x + xrand, st.y + 5 + yrand] = 0;
                    break;
                }
            }
        }
    }
    void Create_Noise_2(Node St, Node En)
    {
        List < BspNd> bsplist = new List<BspNd>();
        Node st = St;
        Node en = En;
        BspNd nd;
        int CAper = 81;
        int cnt = 0;
        while (cnt < 8)
        {
            int len = 1;
            int colorA = 0, colorB = 1;
            int diff_len = en.x - len - st.x - 1;
            int ndl;
            int forl = (len * diff_len * CAper) / 100;


            //좌측하단 가로
            for (int i = st.x + 1; i < en.x - len; i++) //무작위 배열 만듬
            {
                for (int j = st.y + 1; j <= st.y + len; j++)
                {
                    nd.nd.x = i;
                    nd.nd.y = j;
                    nd.randval = UnityEngine.Random.Range(0, len * diff_len);
                    bsplist.Add(nd);
                }
            }
            bsplist.Sort(delegate (BspNd first, BspNd second)
            {
                if (first.randval < second.randval) return 1;
                else if (first.randval > second.randval) return -1;
                else return 0;
            });
            ndl = bsplist.Count;
            for (int i = 0; i < forl; i++)
            {
                Arr[bsplist[i].nd.x, bsplist[i].nd.y] = colorA;
                //    Debug.Log(bsplist[i].nd.x + " " + bsplist[i].nd.y + " " + bsplist[i].randval);

            }
            //우측 상단 가로
            bsplist = new List<BspNd>();
            for (int i = st.x + len + 1; i < en.x; i++) //무작위 배열 만듬
            {
                for (int j = en.y - len; j < en.y; j++)
                {
                    nd.nd.x = i;
                    nd.nd.y = j;
                    nd.randval = UnityEngine.Random.Range(0, len * diff_len);
                    bsplist.Add(nd);
                }
            }
            bsplist.Sort(delegate (BspNd first, BspNd second)
            {
                if (first.randval < second.randval) return 1;
                else if (first.randval > second.randval) return -1;
                else return 0;
            });
            ndl = bsplist.Count;
            for (int i = 0; i < forl; i++)
            {
                Arr[bsplist[i].nd.x, bsplist[i].nd.y] = colorA;
                //    Debug.Log(bsplist[i].nd.x + " " + bsplist[i].nd.y + " " + bsplist[i].randval);
            }
            //우측 하단 세로
            bsplist = new List<BspNd>();
            diff_len = en.y - len - st.y - 1;
            forl = (len * diff_len * CAper) / 100;
            for (int i = en.x - len; i < en.x; i++) //무작위 배열 만듬
            {
                for (int j = st.y + 1; j <= en.y - len - 1; j++)
                {
                    nd.nd.x = i;
                    nd.nd.y = j;
                    nd.randval = UnityEngine.Random.Range(0, len * diff_len);
                    bsplist.Add(nd);
                }
            }
            bsplist.Sort(delegate (BspNd first, BspNd second)
            {
                if (first.randval < second.randval) return 1;
                else if (first.randval > second.randval) return -1;
                else return 0;
            });
            ndl = bsplist.Count;
            for (int i = 0; i < forl; i++)
            {
                Arr[bsplist[i].nd.x, bsplist[i].nd.y] = colorA;
                //   Debug.Log(bsplist[i].nd.x + " " + bsplist[i].nd.y + " " + bsplist[i].randval);
            }
            //좌측 상단 세로
            bsplist = new List<BspNd>();
            diff_len = en.y - len - st.y - 1;
            forl = (len * diff_len * CAper) / 100;
            for (int i = st.x + 1; i <= st.x + len; i++) //무작위 배열 만듬
            {
                for (int j = st.y + len + 1; j < en.y; j++)
                {
                    nd.nd.x = i;
                    nd.nd.y = j;
                    nd.randval = UnityEngine.Random.Range(0, len * diff_len);

                    bsplist.Add(nd);

                }
            }
            bsplist.Sort(delegate (BspNd first, BspNd second)
            {
                if (first.randval < second.randval) return 1;
                else if (first.randval > second.randval) return -1;
                else return 0;
            });
            ndl = bsplist.Count;
            for (int i = 0; i < forl; i++)
            {
                Arr[bsplist[i].nd.x, bsplist[i].nd.y] = colorA;
                //    Debug.Log(bsplist[i].nd.x + " " + bsplist[i].nd.y + " " + bsplist[i].randval);
            }
            CAper -= 6;
            st.x++;
            st.y++;
            en.x--;
            en.y--;
            cnt++;
        }

    }
    void Cellular_Automata(Node st,Node en)
    {
        int[] dx = { 1, 1, 1, 0, 0, 0, -1, -1, -1 };
        int[] dy = { 1, 0, -1, 1, 0, -1, 1, 0, -1 };
        int z_cnt = 0;
        int v_cnt = 0;
        for (int i = st.x; i <= en.x; i++)
        {
            for (int j = st.y; j <= en.y; j++)
            {
                z_cnt = 0;
                v_cnt = 0;
                for (int xcnt = 0; xcnt < 9; xcnt++)
                {
                    if (i + dx[xcnt] < st.x || i + dx[xcnt] > en.x || j + dy[xcnt] < st.y || j + dy[xcnt] > en.y)
                    {
                        continue;
                    }
                    else v_cnt++;
                    if (Arr[i + dx[xcnt], j + dy[xcnt]] == 0) z_cnt++;
                }
                if (z_cnt > v_cnt/2)
                {
                    Copy[i, j] = 0;
                }

                else if(z_cnt<v_cnt/2)
                {
                    if (Copy[i, j] == 99)
                    { }
                    else
                    {
                        Copy[i, j] = 1;
                    }
                }
                
            }
        }
        for (int i = st.x; i <= en.x; i++)
        {
            for (int j = st.y; j <= en.y; j++)
            {
                Arr[i, j] = Copy[i, j];
            }
        }

    }
    void Connect(PairNode First,PairNode Second)
    {
        Node firstmid;
        firstmid.x = (First.first.x + First.second.x) / 2;
        firstmid.y = (First.first.y + First.second.y) / 2;
        Node secondmid;
        secondmid.x = (Second.first.x + Second.second.x) / 2;
        secondmid.y = (Second.first.y + Second.second.y) / 2;
        for(int i = firstmid.x - 1;i<= secondmid.x + 1; i++)
        {
            for(int j = firstmid.y - 1; j <= secondmid.y + 1; j++)
            {
                Arr[i, j] = 1;
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
