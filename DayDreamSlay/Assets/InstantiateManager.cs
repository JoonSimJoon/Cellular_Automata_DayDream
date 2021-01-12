using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateManager : MonoBehaviour
{

   private static InstantiateManager instance;
   public static InstantiateManager Instance { get{
            if (instance == null)
                instance = FindObjectOfType<InstantiateManager>();
            return instance;
        }
    }
    [SerializeField] bool isSpawn = true;
    [SerializeField] float spawnTime;
    MapCreate mapCreate;
    public int stageNum;
    public Camera mainCam;
    public GameObject player;
    public GameObject []monsters = new GameObject[10];
    public void Awake()
    {
        StartCoroutine(InstantiateMob());
        mapCreate = GetComponentInChildren<MapCreate>();
    }

    IEnumerator InstantiateMob()
    {
        while (isSpawn)
        {
            yield return new WaitForSeconds(spawnTime);
            int x = Random.Range(-20, 21);
            int y = Random.Range(-15, 21);
            Vector3Int vec3 = mapCreate.wallTilemap.WorldToCell(PlayerMove.Instance.transform.position);
            x += vec3.x;
            y += vec3.y;
            Vector3 screenPoint = mainCam.WorldToViewportPoint(new Vector3(x, y, 0));
            if((screenPoint.x <= 0 || screenPoint.x >= 1) && (screenPoint.y <=0 || screenPoint.y >= 1))
            {
                if( x < mapCreate.mapX - 3 && x > 1 && y < mapCreate.mapY - 3 && y > 1)
                {
                    if(mapCreate.Arr[x, y] > 0 && mapCreate.Arr[x+1,  y] > 0 && mapCreate.Arr[x - 1, y] > 0 
                        && mapCreate.Arr[x, y+1] > 0 && mapCreate.Arr[x + 1, y+1] > 0 && mapCreate.Arr[x - 1, y+1] > 0
                         && mapCreate.Arr[x, y -1] > 0 && mapCreate.Arr[x + 1, y - 1] > 0 && mapCreate.Arr[x - 1, y - 1] > 0)
                    {
                        GameObject mob = Instantiate(monsters[0]);
                        mob.transform.position = new Vector3(x, y);
                    }
                }
            }
        }
    }
}
