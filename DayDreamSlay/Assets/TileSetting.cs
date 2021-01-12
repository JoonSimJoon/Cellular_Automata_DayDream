using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetting : MonoBehaviour
{
    public bool wall;
    public int x;
    public int y;
    public int roomNum;
    BoxCollider2D box;
    public SpriteRenderer renderer;

    private void Awake()
    {
        this.transform.position = new Vector3(x, y, 0);
        box = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }
}
