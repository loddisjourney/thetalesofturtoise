using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class Cell : MonoBehaviour
{
    public float entropy;

    //Sammle alle Valid Neighbors aus dem Ressource Ordner  und erstelle eine Liste daraus
    /*
     * Load items from folder https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233 27.12.2023
    */
    private TileData[] _validTiles; //[HideInInspector]

    private TileData[] validTiles
    {
        get
        {
            if (_validTiles == null)
            {
                _validTiles = Resources.LoadAll<TileData>("Test");
                
            }
            return _validTiles;
        }
    }
    private List<TileData> _validNeighbors;
    public List<TileData> validNeighbors
    {
        get
        {
            if (_validNeighbors == null)
            {
                _validNeighbors = new List<TileData>();
                for (int i = 0; i < validTiles.Length; i++)
                {
                    _validNeighbors.Add(validTiles[i]);
                }
            }
            return _validNeighbors;
        }
    }
    public TileData defaultTile;

    public List<TileData> tilesTicketList;

    public int collapsedTile;

    public bool collapsed = false;
    public bool isNeighbor = false;

    public bool newCheck;

    [SerializeField] private GameObject mapParent;



    private void Awake()
    {
        //Nicht optimal ggf. wenn es nicht gefundne wird eins erstellen unter LevelGenerator
        mapParent = GameObject.Find("LevelMap");
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        //if(collapsed&&newCheck)
        //{
        //    CheckNewValidTiles();
        //}
        if (collapsed) isNeighbor = false;
        if(isNeighbor)
        {
            GameObject child = this.transform.GetChild(0).gameObject;
            child.SetActive(true);
        }
        else
        {
            GameObject child = this.transform.GetChild(0).gameObject;
            child.SetActive(false);
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
        //Weight ist die Wahrscheinlichkeit das das Tile gew?hlt werden soll, je h?her desto h?her die Wahrscheinlichkeit
        entropy = 0;
        //Summe der gesammten Weights
        float totalWeight = 0;
        for(int i = 0; i < validNeighbors.Count; i++)
        {
            totalWeight += validNeighbors[i].weight;
        }

        //Die Entropy ist die Summe der Wahrscheinlichkeit der m?glichen Tiles multipliziert mit dem Logarithmus der ahrscheinlichkeit des Tiles, das Ergebnis wird negiert
        //Die Wahrscheinlihckeit ist das Weight des aktuellen m?glichen Tiles durch die addierte gesammten Weightrs alller m?glichen Weights
        for (int i = 0; i < validNeighbors.Count; i++)
        {
            float wahrscheinlichkeit = validNeighbors[i].weight / totalWeight;
            entropy += wahrscheinlichkeit * Mathf.Log(wahrscheinlichkeit, 2);
        }

        //Debug.Log("Result of Entropy "+ -entropy + "Count of valid neighbors" + validNeighbors.Count);   
        return -entropy; 
    }

    public void ChooseRandomTile()
    {
        //Choose a random Tile if its not empty
        if(validNeighbors.Count != 0)
        {
            //use the weight to choose a tile based on it --> ggf bessere und effizientere art
            for(int i = 0; i < validNeighbors.Count; i++)
            {
                int probaility = validNeighbors[i].weight;
                for(int j = 0; j < probaility; j++)
                {
                    tilesTicketList.Add(validNeighbors[i]);
                }
            }
            
            int randTile = UnityEngine.Random.Range(0, tilesTicketList.Count); // statt valid neighbor
            
            //Place random Tile at this Cells Position
            GameObject chosenTile = tilesTicketList[randTile].meshObj;
            GameObject parentTile = Instantiate(chosenTile, this.transform.position, Quaternion.identity);
            GameObject child = parentTile.transform.GetChild(0).gameObject;
            child.transform.rotation = Quaternion.Euler(0, (int)tilesTicketList[randTile].tileRotation, 0);
            //Speichere den gew?hlten Index
            parentTile.transform.parent = mapParent.transform;
            collapsedTile = randTile;
        }
        else
        {
            //Place air Tile at this Cells Position
            Instantiate(defaultTile.meshObj, this.transform.position, Quaternion.identity);

            //collapsedTile = 0;
            //validNeighbors.Add(defaultTile);
        }
        
        //Set it to collapsed
        collapsed = true;
        
    }

    public void FirstTile()
    {

        //use the weight to choose a tile based on it --> ggf bessere und effizientere art --> erster aufruf zum initialisieren fuer nachbar abfrage
        for (int i = 0; i < validNeighbors.Count; i++)
        {
            int probaility = validNeighbors[i].weight;
            for (int j = 0; j < probaility; j++)
            {
                tilesTicketList.Add(validNeighbors[i]);
            }
        }

        //Place first tile with specific gras ground

        int grasTile = 0;
        for(int i = 0; i < tilesTicketList.Count; i++)
        {
            if(tilesTicketList[i].meshObj.name =="grass_base")
            {
                grasTile = i;
                collapsedTile = i;
            }
        }

        
        //Place random Tile at this Cells Position
        GameObject chosenTile = tilesTicketList[grasTile].meshObj;
        GameObject parentTile = Instantiate(chosenTile, this.transform.position, Quaternion.identity);
        GameObject child = parentTile.transform.GetChild(0).gameObject;
        child.transform.rotation = Quaternion.Euler(0, (int)tilesTicketList[grasTile].rotation, 0);
        //Speichere den gewaehlten Index
        parentTile.transform.parent = mapParent.transform;
        collapsed = true;
       
    }

    //public void CheckNewValidTiles()
    //{
    //    //only if collapsed it should to this
    //    //check ob das gew?hlte tile an den nachbarn das gleiche hat wenn nicht streiche das mesh name aus dem array
    //    if (newCheck)
    //    {
    //        newCheck = false;
    //    }
    //}

}
