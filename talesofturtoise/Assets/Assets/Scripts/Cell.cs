using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float entropy;
    private TileData[] validTiles; //[HideInInspector]
    public TileData defaultTile;
    /*
     * Load items from folder https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233 27.12.2023
     */
    private void Start()
    {
        //validTiles = Resources.LoadAll<TileData>("Tiles");
        validTiles = Resources.LoadAll<TileData>("Example");
    }

    public TileData[] GetTileData()
    { 
        return validTiles; 
    }

    // this is based on the entropy calculation of WFC entropy sum explained by Boris "Wave Function Collapse explained - Least Entropy", under: https://www.boristhebrave.com/2020/04/13/wave-function-collapse-explained/ and entropy calculation way of DV Gen on YouTube, under: https://youtu.be/zIRTOgfsjl0?si=J4xOnB-HwOMk4Caf&t=932
    public float CalculateEntropy()
    {
        //Calculates the Entropy of the Cell with the Count and Weight of the valid Tiles
        //Weight ist die Wahrscheinlichkeit das das Tile gewählt werden soll, je höher desto höher die Wahrscheinlichkeit

        //Summe der gesammten Weights
        float totalWeight = 0;
        for(int i = 0; i < validTiles.Length; i++)
        {
            totalWeight += validTiles[i].weight;
        }

        //Die Entropy ist die Summe der Wahrscheinlichkeit der möglichen Tiles multipliziert mit dem Logarithmus der ahrscheinlichkeit des Tiles, das Ergebnis wird negiert
        //Die Wahrscheinlihckeit ist das Weight des aktuellen möglichen Tiles durch die addierte gesammten Weightrs alller möglichen Weights
        for (int i = 0; i < validTiles.Length; i++)
        {
            float wahrscheinlichkeit = validTiles[i].weight / totalWeight;
            entropy += wahrscheinlichkeit * Mathf.Log(wahrscheinlichkeit, 2);
        }

        Debug.Log("Result "+ -entropy);   
        return -entropy; 
    }

}
