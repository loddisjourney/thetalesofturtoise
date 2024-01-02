using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    public float entropy;
    private TileData[] validTiles; //[HideInInspector]
    public TileData defaultTile;
    public bool collapsed;
    /*
     * Load items from folder https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233 27.12.2023
     */
    private void Start()
    {
        validTiles = Resources.LoadAll<TileData>("Tiles");
        //validTiles = Resources.LoadAll<TileData>("Example");
    }

    public TileData[] GetTileData()
    { 
        return validTiles; 
    }

    // this is based on the entropy calculation of WFC entropy sum explained by Boris "Wave Function Collapse explained - Least Entropy", under: https://www.boristhebrave.com/2020/04/13/wave-function-collapse-explained/ and entropy calculation way of DV Gen on YouTube, under: https://youtu.be/zIRTOgfsjl0?si=J4xOnB-HwOMk4Caf&t=932
    public float CalculateEntropy()
    {
        //Calculates the Entropy of the Cell with the Count and Weight of the valid Tiles
        //Weight ist die Wahrscheinlichkeit das das Tile gew�hlt werden soll, je h�her desto h�her die Wahrscheinlichkeit

        //Summe der gesammten Weights
        float totalWeight = 0;
        for(int i = 0; i < validTiles.Length; i++)
        {
            totalWeight += validTiles[i].weight;
        }

        //Die Entropy ist die Summe der Wahrscheinlichkeit der m�glichen Tiles multipliziert mit dem Logarithmus der ahrscheinlichkeit des Tiles, das Ergebnis wird negiert
        //Die Wahrscheinlihckeit ist das Weight des aktuellen m�glichen Tiles durch die addierte gesammten Weightrs alller m�glichen Weights
        for (int i = 0; i < validTiles.Length; i++)
        {
            float wahrscheinlichkeit = validTiles[i].weight / totalWeight;
            entropy += wahrscheinlichkeit * Mathf.Log(wahrscheinlichkeit, 2);
        }

        Debug.Log("Result "+ -entropy);   
        return -entropy; 
    }

    public void ChooseRandomTile()
    {
        //Choose a random Tile if its not empty
        if(validTiles.Length == 0)
        {
            int randTile = UnityEngine.Random.Range(0, validTiles.Length);
            //Place random Tile at this Cells Position
            //Instantiate(validTiles[randTile].pY, this.transform.position, Quaternion.identity);
        }
        else
        {
            //Place air Tile at this Cells Position
            Instantiate(defaultTile, this.transform.position, Quaternion.identity);
        }

        //Set it to collapsed
        collapsed = true;
    }

}
