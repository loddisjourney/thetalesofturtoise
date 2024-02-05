using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class Cell : MonoBehaviour
{
    /*
     * Grid Zelle, welche es mit der Wave Function Collapse zu fuellen gilt
     * 
     * Entropy ist dér Wert des errechneten Lowest Entropy
     * 
     * Liste der möglichen Tile besteht aus Array, welches automatisch alle Tiles aus einem Ordner liest, und einer Liste, welche dann vom Array gefuellt wird.
     * Sobald das Array oder die Liste vom Code angesprochen wird, wird es mit einem Getter initialisiert -> da es erst zur Runtime aus dem Ordner gefuellt wird.
     * 
     * *
     */
    
    public float entropy;

    //Sammle alle Valid Neighbors aus dem Ressource Ordner  und erstelle eine Liste daraus
    /*
     * Load items from folder https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233 27.12.2023
    */
    private TileData[] _validTiles;

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
    private List<TileData> _validTilesList;
    public List<TileData> validTilesList
    {
        get
        {
            if (_validTilesList == null)
            {
                _validTilesList = new List<TileData>();
                for (int i = 0; i < validTiles.Length; i++)
                {
                    _validTilesList.Add(validTiles[i]);
                }
            }
            return _validTilesList;
        }
    }
    public TileData defaultTile;

    public List<TileData> tilesTicketList;

    public int collapsedTile;

    public bool collapsed = false;
    public bool isNeighbor = false;

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
        for(int i = 0; i < validTilesList.Count; i++)
        {
            totalWeight += validTilesList[i].weight;
        }

        //Die Entropy ist die Summe der Wahrscheinlichkeit der m?glichen Tiles multipliziert mit dem Logarithmus der ahrscheinlichkeit des Tiles, das Ergebnis wird negiert
        //Die Wahrscheinlihckeit ist das Weight des aktuellen m?glichen Tiles durch die addierte gesammten Weightrs alller m?glichen Weights
        for (int i = 0; i < validTilesList.Count; i++)
        {
            float wahrscheinlichkeit = validTilesList[i].weight / totalWeight;
            entropy += wahrscheinlichkeit * Mathf.Log(wahrscheinlichkeit, 2);
        }

        //Debug.Log("Result of Entropy "+ -entropy + "Count of valid neighbors" + validTilesList.Count);   
        return -entropy; 
    }

    public void ChooseRandomTile()
    {
        //Choose a random Tile if its not empty
        if(validTilesList.Count != 0)
        {
            //use the weight to choose a tile based on it --> ggf bessere und effizientere art
            for(int i = 0; i < validTilesList.Count; i++)
            {
                int probaility = validTilesList[i].weight;
                for(int j = 0; j < probaility; j++)
                {
                    tilesTicketList.Add(validTilesList[i]);
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
            //validTilesList.Add(defaultTile);
        }
        
        //Set it to collapsed
        collapsed = true;
        
    }

    public void FirstTile()
    {

        //use the weight to choose a tile based on it --> ggf bessere und effizientere art --> erster aufruf zum initialisieren fuer nachbar abfrage
        for (int i = 0; i < validTilesList.Count; i++)
        {
            int probaility = validTilesList[i].weight;
            for (int j = 0; j < probaility; j++)
            {
                tilesTicketList.Add(validTilesList[i]);
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


}
