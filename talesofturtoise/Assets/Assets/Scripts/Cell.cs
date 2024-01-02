using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class Cell : MonoBehaviour
{
    public float entropy;
    private TileData[] validTiles; //[HideInInspector]
    public TileData defaultTile;
    public bool collapsed = false;
    public bool newCheck;
    public List<TileData> validNeighbors;
    public int collapsedTile;
    /*
     * Load items from folder https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233 27.12.2023
     */
    private void Start()
    {
        //Sammle alle Valid Neighbors aus dem Ressource Ordner  und erstelle eine Liste daraus
        validTiles = Resources.LoadAll<TileData>("Tiles");
        for (int i = 0; i < validTiles.Length; i++)
        {
            validNeighbors.Add(validTiles[i]);
        }
        Debug.Log("Neighhbor List "+ validNeighbors.Count);
        //validTiles = Resources.LoadAll<TileData>("Example");
    }
    private void Update()
    {
        if(collapsed&&newCheck)
        {
            CheckNewValidTiles();
        }
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
        entropy = 0;
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

        Debug.Log("Result of Entropy "+ -entropy);   
        return -entropy; 
    }

    public void ChooseRandomTile()
    {
        //Choose a random Tile if its not empty
        if(validTiles.Length != 0)
        {
            int randTile = UnityEngine.Random.Range(0, validTiles.Length);
            //GEt name of MeshObj of the Tile
            //string meshName = validTiles[randTile].meshObj.name;
            //Debug.Log("Name " + meshName);
            //Get the rotation of the tile => muss nochmal verbessert werden indem die enums integer zugeweisen werden und dann statt verzweigung direkt in das instantiate (int)rotation aufgerufen wird; Problem: 200x neu rot setzen https://discussions.unity.com/t/use-an-integer-as-an-enum/63612/2
            //add new enum und lösche das andere am ende der aufgabe... falls dadurch alle tile infos raus gehen
            int rot = 0;
            if(validTiles[randTile].rotation == TileData.Rotation.r0)
            {
                rot = 0;
            }
            else if(validTiles[randTile].rotation == TileData.Rotation.r90)
            {
                rot = 90;
            }
            else if (validTiles[randTile].rotation == TileData.Rotation.r180)
            {
                rot = 180;
            }
            else if (validTiles[randTile].rotation == TileData.Rotation.r270)
            {
                rot = 270;
            }
            //Place random Tile at this Cells Position
            GameObject chosenTile = validTiles[randTile].meshObj;
            Instantiate(chosenTile, this.transform.position, Quaternion.Euler(0,rot,0));
            //Speichere den gewählten Index
            collapsedTile = randTile;
        }
        else
        {
            //Place air Tile at this Cells Position
            Instantiate(defaultTile.meshObj, this.transform.position, Quaternion.identity);
        }
        
        //Set it to collapsed
        collapsed = true;
    }

    public void FirstTile()
    {
        //Place first tile with specific gras ground
        
        int grasTile = 0;
        for(int i = 0; i < validTiles.Length; i++)
        {
            if(validTiles[i].meshObj.name =="ground_base")
            {
                grasTile = i;
                collapsedTile = i;
            }
        }
        int rot = 0;
        if (validTiles[grasTile].rotation == TileData.Rotation.r0)
        {
            rot = 0;
        }
        else if (validTiles[grasTile].rotation == TileData.Rotation.r90)
        {
            rot = 90;
        }
        else if (validTiles[grasTile].rotation == TileData.Rotation.r180)
        {
            rot = 180;
        }
        else if (validTiles[grasTile].rotation == TileData.Rotation.r270)
        {
            rot = 270;
        }
        //Place random Tile at this Cells Position
        GameObject chosenTile = validTiles[grasTile].meshObj;
        Instantiate(chosenTile, this.transform.position, Quaternion.Euler(0, rot, 0));
        //Speichere den gewählten Index
        
        collapsed = true;     
    }

    public void CheckNewValidTiles()
    {
        //only if collapsed it should to this
        //check ob das gewählte tile an den nachbarn das gleiche hat wenn nicht streiche das mesh name aus dem array
        if(newCheck)
        {
            newCheck = false;
        }
    }

}
