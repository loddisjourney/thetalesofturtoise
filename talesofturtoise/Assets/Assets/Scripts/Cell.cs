using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float entropy = 0;
    public TileData[] validTiles = new TileData[0];
    public TileData defaultTile;
    /*
     * Load items from folder https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233 27.12.2023
     */
    private void Start()
    {
        validTiles = Resources.LoadAll<TileData>("Tiles");
    }


}
