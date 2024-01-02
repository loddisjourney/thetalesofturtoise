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
        i = gridList[12].gameObject.GetComponent<Cell>().GetTileData().Length;
        Debug.Log("Grid " + gridList[12].gameObject.GetComponent<Cell>().name + " "+ i);
        gridList[12].gameObject.GetComponent<Cell>().CalculateEntropy();
        
        
        gameManager.gameState = GameManager.GameState.PlaceTile;
    }

    public void FindLowestEntropy()
    {
        //Go through all GridCells and Find the lowest Entropy.. safe it in a list/array

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < length; x++)
            {
                for (int z = 0; z < width; z++)
                {
                }
            }
        }


        //check the count of the list with the loweste entropy delete those which a higher then this entropy
        // choose a rasndom cell of the lowest enntropy
    }
}
