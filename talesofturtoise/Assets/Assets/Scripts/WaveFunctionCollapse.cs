using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Level Info")]
    [SerializeField] private int width = 10; //z
    [SerializeField] private int height = 2; //y
    [SerializeField] private int length = 10; //x

    [Header("Grid Info")]
    //[SerializeField] GameObject gridElement;
    public Cell cell;
    int i;
    [SerializeField] GameObject gridCell;
    private GameObject currentCell;
    public List<GameObject> gridList; // liste der grid objekte, um auf die cell infos zu kommen
    public List<GameObject> validGridCellList; //not collapsed Grid Cells
    public List<GameObject> lowestGridCellList;

    [Header ("Ground Generation")]
    //This will be replaced by the worlds atmosphere
    [SerializeField] private GameObject waterTile;
    [SerializeField] private GameObject waterParent;
    private GameObject currentWaterElement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGrid()
    {
       
        //Generate Grid and create a List of all Grid Elements
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < length; x++)
            {
                for(int z = 0; z < width; z++)
                {
                    //Create a Grid Cell at x y z position and use this as identifier + set as child of this object
                    currentCell = Instantiate(gridCell, new Vector3(x, y, z), Quaternion.identity);
                    currentCell.name = $"Cell {x} {y} {z}";
                    currentCell.transform.parent = this.transform;
                    gridList.Add(currentCell);
                   // Debug.Log(gridList.Count);
                    
                }
            }
        }

        //Generate a Base Floor of water under the Grid and place it under the group of WaterParent
        for(int x2 = 0; x2 < length; x2++)
        {
            for(int z2 = 0;z2 < width; z2++)
            {
                currentWaterElement = Instantiate(waterTile, new Vector3(x2, length - (length+1), z2), Quaternion.identity);
                currentWaterElement.transform.parent = waterParent.transform;
            }
        }

        //Test des Entropy und Zugriff eines Gridelement
        //i = gridList[12].gameObject.GetComponent<Cell>().GetTileData().Length;
       // Debug.Log("Grid " + gridList[12].gameObject.GetComponent<Cell>().name + " "+ i);
        //gridList[12].gameObject.GetComponent<Cell>().CalculateEntropy();

        gameManager.gameState = GameManager.GameState.PlaceTile;
    }

    public void FindLowestEntropy()
    {
        //Go through all GridCells which are not collapsed already and Find the lowest Entropy.. safe it in a list/array
        //Create List of not collapsed Grid Cells
        if(validGridCellList != null) validGridCellList.Clear();
        for (int g = 0; g < gridList.Count; g++)
        {
            if(gridList[g].gameObject.GetComponent<Cell>().collapsed == false)
            {
                validGridCellList.Add(gridList[g]);
            }
        }
        Debug.Log("Count " +validGridCellList.Count);
        if(validGridCellList.Count != 0)
        { 
        //First get the Lowest Entropy  of this new List
        float lowestEntropy = float.PositiveInfinity; //https://forum.unity.com/threads/finding-the-index-of-the-lowest-valued-element-in-an-array.295195/
        for (int g = 0; g < validGridCellList.Count; g++)
        {
           float currentEntropy;
           currentEntropy = validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy();
           
            if(currentEntropy < lowestEntropy)
            {
                lowestEntropy = currentEntropy;
            }           
        }
        Debug.Log("Lowest Entropy "+lowestEntropy);

        //Then generate List of all lowest Entropy
        if (lowestGridCellList != null) lowestGridCellList.Clear();
        for (int g = 0; g < validGridCellList.Count; g++)
        { 
            if(lowestEntropy == validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy())
            {
                lowestGridCellList.Add(validGridCellList[g]);
            }
        }
        Debug.Log(lowestGridCellList.Count);
        //Choose a Random of the lowest Entropy Grid Cells
        int randGridCell = UnityEngine.Random.Range(0, lowestGridCellList.Count); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //Place a Random Tile add this GridCell
        PlaceTile(randGridCell);
        }
    }

    public void PlaceTile(int randIndex)
    {
        //Choose a random Tile of the chosen Grid Cell and place it
        lowestGridCellList[randIndex].gameObject.GetComponent<Cell>().ChooseRandomTile();
        
        gameManager.gameState = GameManager.GameState.CheckNeighbors;
    }
}
