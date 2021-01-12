using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSet : TileBase
{
    [SerializeField] private bool wall;
    [SerializeField] private Sprite wallSprite;
    [SerializeField] private Sprite norSprite;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        return base.StartUp(position, tilemap, go);
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if (wall)
            tileData.sprite = wallSprite;
        else
            tileData.sprite = norSprite;
    }
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Tiles/ThisTile")]
    public static void CreateWallTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save WallTile", "New WallTile", "asset", "Save WallTile", "Assets");
        if (path == "")
        {
            return;
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TileSet>(), path);
    }

#endif
}

