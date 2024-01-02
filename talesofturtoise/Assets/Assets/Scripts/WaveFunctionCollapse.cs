using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public List<GameObject> collapsedGridCellList;

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

        //check if its the first placment
        if(gridList.Count == validGridCellList.Count)
        {
            //First Tile, vorerst setze Gras Tile
            //Dies kann ersetzt werden durch Rahmen oder ähnliche vor definierte Level, ggf. auch nur die inneren position oder eine konkrete position als start

            int randX = UnityEngine.Random.Range(0, length);
            int randZ = UnityEngine.Random.Range(0, width);
            int firstGridCell = 0;
            Vector3 firstPos = new Vector3 (randX, 0, randZ);
            for(int i = 0; i < gridList.Count; i++)
            {
                if(firstPos == gridList[i].transform.position)
                { 
                    firstGridCell = i;
                }
            }
            //Platziere Tile
            gridList[firstGridCell].gameObject.GetComponent<Cell>().FirstTile();
            //Füge Cell zur Liste hinzu
            collapsedGridCellList.Add(gridList[firstGridCell]);

            gameManager.gameState = GameManager.GameState.CheckNeighbors;
        }
        else if(validGridCellList.Count == 0)
        {
            //Completed Placement
            //setze level laden auf true aus dem loadings screen level
            gameManager.gameState = GameManager.GameState.FreeGame;
        }
        else
        {
            //Continue Placement
            //First get the Lowest Entropy  of this new List
            float lowestEntropy = float.PositiveInfinity; //https://forum.unity.com/threads/finding-the-index-of-the-lowest-valued-element-in-an-array.295195/
            for (int g = 0; g < validGridCellList.Count; g++)
            {
                float currentEntropy;
                currentEntropy = validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy();

                if (currentEntropy < lowestEntropy)
                {
                    lowestEntropy = currentEntropy;
                }
            }
            Debug.Log("Lowest Entropy " + lowestEntropy);

            //Then generate List of all lowest Entropy
            if (lowestGridCellList != null) lowestGridCellList.Clear();
            for (int g = 0; g < validGridCellList.Count; g++)
            {
                if (lowestEntropy == validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy())
                {
                    lowestGridCellList.Add(validGridCellList[g]);
                }
            }
            Debug.Log(lowestGridCellList.Count);
            //Choose a Random of the lowest Entropy Grid Cells
            int randGridCell = UnityEngine.Random.Range(0, lowestGridCellList.Count); 
            //Place a Random Tile add this GridCell
            PlaceTile(randGridCell);
        }
    }

    public void PlaceTile(int randIndex)
    {
        //Choose a random Tile of the chosen Grid Cell and place it
        lowestGridCellList[randIndex].gameObject.GetComponent<Cell>().ChooseRandomTile();
        //füge Grid Cell zur Liste hinzu -> ggf von cells steuern
        collapsedGridCellList.Add(lowestGridCellList[randIndex]);
        gameManager.gameState = GameManager.GameState.CheckNeighbors;
    }

    public void CheckNeighbors()
    {
        //Durchlaufe alle collapsed Cells
        for(int i = 0; i < collapsedGridCellList.Count; i++)
        {
            GameObject currentGridCell = collapsedGridCellList[i];
            string currentSocket;

            //Nachbar Pos X----------------------------------------------------------------------------------------------- Pos X == Neg X?
            currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX;
            Vector3 neighborFrontPos = currentGridCell.transform.position += new Vector3(1, 0, 0);

            PosXNeighbor(currentSocket, neighborFrontPos);
            
            /*...*/

            //Nachbar Neg X ----------------------------------------------------------------------------------------------- Neg X == Pos X?           
            currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nX;
            Vector3 neighborBackPos = currentGridCell.transform.position += new Vector3(-1, 0, 0);

            NegXNeighbor(currentSocket, neighborBackPos);

            //Nachbar Pos Z ----------------------------------------------------------------------------------------------- Pos Z == Neg Z?
            currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pZ;
            Vector3 neighborRightPos = currentGridCell.transform.position += new Vector3(0, 0, 1);

            PosZNeighbor(currentSocket, neighborRightPos);

            //Nachbar Neg Z ----------------------------------------------------------------------------------------------- Neg Z == Pos Z?
            currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nZ;
            Vector3 neighborLeftPos = currentGridCell.transform.position += new Vector3(0, 0, -1);

            NegZNeighbor(currentSocket, neighborLeftPos);

            //Nachbar Pos Y ----------------------------------------------------------------------------------------------- Pos Y == Neg Y?
            currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pY;
            Vector3 neighborTopPos = currentGridCell.transform.position += new Vector3(0, 1, 0);

            PosYNeighbor(currentSocket, neighborTopPos);

            //Nachbar Neg Y  ----------------------------------------------------------------------------------------------- Neg Y == Pos Y?
            currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nY;
            Vector3 neighborBottomPos = currentGridCell.transform.position += new Vector3(0, -1, 0);

            NegYNeighbor(currentSocket, neighborBottomPos);
            

            //placetile!!!!!!!!!!!!!!!!!!!!!!!!!!
            gameManager.gameState = GameManager.GameState.FreeGame;
        }
    }

    public void PosXNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Find this neighbor
        GameObject neighborPosX = null;
        for (int xP = 0; xP < gridList.Count; xP++)
        {
            if (gridList[xP].transform.position == neighborPos)
            {
                neighborPosX = gridList[xP];
            }
        }
        if (neighborPosX != null)
        {
            //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                Debug.Log("Current " + currentSocket);

                string neighborSocket = neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX;
                Debug.Log("Neighbor " + neighborSocket);

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    Debug.Log("is flipped");
                    //Check if neighbor is none flipped
                    if(currentSocket == neighborSocket + "f")
                    {
                        Debug.Log("is sym");
                    }
                }
                else if(isNoneFlipped)
                {
                    Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        Debug.Log("is sym");
                    }
                }
                else if(currentSocket.Contains("v"))
                {
                    //muss noch verallgemeinert werden
                    //is vertical
                    if (currentSocket == neighborSocket)
                    {
                        //both vertical
                    }
                }
                else
                {
                    //Sie sind ungleich und damit raus
                    Debug.Log("Delete");
                    neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }
        }
    }

    public void NegXNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Find this neighbor
        GameObject neighborNegX = null;
        for (int xP = 0; xP < gridList.Count; xP++)
        {
            if (gridList[xP].transform.position == neighborPos)
            {
                neighborNegX = gridList[xP];
            }
        }
        if (neighborNegX != null)
        {
            //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborNegX.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                Debug.Log("Current " + currentSocket);

                string neighborSocket = neighborNegX.gameObject.GetComponent<Cell>().validNeighbors[xA].pX;
                Debug.Log("Neighbor " + neighborSocket);

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    Debug.Log("is flipped");
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (isNoneFlipped)
                {
                    Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (currentSocket.Contains("v"))
                {
                    //muss noch verallgemeinert werden
                    //is vertical
                    if (currentSocket == neighborSocket)
                    {
                        //both vertical
                    }
                }
                else
                {
                    //Sie sind ungleich und damit raus
                    Debug.Log("Delete");
                    neighborNegX.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }
        }
    }

    public void PosZNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Find this neighbor
        GameObject neighborPosZ = null;
        for (int xP = 0; xP < gridList.Count; xP++)
        {
            if (gridList[xP].transform.position == neighborPos)
            {
                neighborPosZ = gridList[xP];
            }
        }
        if (neighborPosZ != null)
        {
            //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                Debug.Log("Current " + currentSocket);

                string neighborSocket = neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors[xA].nZ;
                Debug.Log("Neighbor " + neighborSocket);

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    Debug.Log("is flipped");
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (isNoneFlipped)
                {
                    Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (currentSocket.Contains("v"))
                {
                    //muss noch verallgemeinert werden
                    //is vertical
                    if (currentSocket == neighborSocket)
                    {
                        //both vertical
                    }
                }
                else
                {
                    //Sie sind ungleich und damit raus
                    Debug.Log("Delete");
                    neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }
        }
    }

    public void NegZNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Find this neighbor
        GameObject neighborNegZ = null;
        for (int xP = 0; xP < gridList.Count; xP++)
        {
            if (gridList[xP].transform.position == neighborPos)
            {
                neighborNegZ = gridList[xP];
            }
        }
        if (neighborNegZ != null)
        {
            //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                Debug.Log("Current " + currentSocket);

                string neighborSocket = neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors[xA].pZ;
                Debug.Log("Neighbor " + neighborSocket);

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    Debug.Log("is flipped");
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (isNoneFlipped)
                {
                    Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (currentSocket.Contains("v"))
                {
                    //muss noch verallgemeinert werden
                    //is vertical
                    if (currentSocket == neighborSocket)
                    {
                        //both vertical
                    }
                }
                else
                {
                    //Sie sind ungleich und damit raus
                    Debug.Log("Delete");
                    neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }
        }
    }

    public void PosYNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Find this neighbor
        GameObject neighborPosY = null;
        for (int xP = 0; xP < gridList.Count; xP++)
        {
            if (gridList[xP].transform.position == neighborPos)
            {
                neighborPosY = gridList[xP];
            }
        }
        if (neighborPosY != null)
        {
            //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborPosY.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                Debug.Log("Current " + currentSocket);

                string neighborSocket = neighborPosY.gameObject.GetComponent<Cell>().validNeighbors[xA].nY;
                Debug.Log("Neighbor " + neighborSocket);

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    Debug.Log("is flipped");
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (isNoneFlipped)
                {
                    Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (currentSocket.Contains("v"))
                {
                    //muss noch verallgemeinert werden
                    //is vertical
                    if (currentSocket == neighborSocket)
                    {
                        //both vertical
                    }
                }
                else
                {
                    //Sie sind ungleich und damit raus
                    Debug.Log("Delete");
                    neighborPosY.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }
        }
    }

    public void NegYNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Find this neighbor
        GameObject neighborNegY = null;
        for (int xP = 0; xP < gridList.Count; xP++)
        {
            if (gridList[xP].transform.position == neighborPos)
            {
                neighborNegY = gridList[xP];
            }
        }
        if (neighborNegY != null)
        {
            //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborNegY.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                Debug.Log("Current " + currentSocket);

                string neighborSocket = neighborNegY.gameObject.GetComponent<Cell>().validNeighbors[xA].pY;
                Debug.Log("Neighbor " + neighborSocket);

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    Debug.Log("is flipped");
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (isNoneFlipped)
                {
                    Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        Debug.Log("is sym");
                    }
                }
                else if (currentSocket.Contains("v"))
                {
                    //muss noch verallgemeinert werden
                    //is vertical
                    if (currentSocket == neighborSocket)
                    {
                        //both vertical
                    }
                }
                else
                {
                    //Sie sind ungleich und damit raus
                    Debug.Log("Delete");
                    neighborNegY.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }
        }
    }

    public bool ContainsOnlyDigitCheck(string s)
    {
        //reference https://learn.microsoft.com/de-de/dotnet/api/system.char.isdigit?view=net-8.0
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if(!char.IsDigit(c)) return false;
        }
        return true;
    }

    /*...*/
    /*
            //Find this neighbor
            GameObject neighborPosX = null;
            for (int xP = 0; xP < gridList.Count; xP++)
            {
                if (gridList[xP].transform.position == neighborFrontPos)
                {
                    neighborPosX = gridList[xP];
                }
            }
            if(neighborPosX != null)
            {
                WalkThroughNeighbor(currentCellStringX, neighborPosX);

                
                //Vergleiche das PosX Socket von current mit sllen möglichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

                for (int xA = 0; xA < neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
                {
                    Debug.Log("Current " + currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX);
                    Debug.Log("Neighbor " + neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX);
                    //check if its none flipped => only digits 
                    bool isNoneFlipped = ContainsOnlyDigitCheck(currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX);
                    //wenn Pos X == Neg X dann symetrisch => zahl+s
                    if (currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX == neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX)
                    {
                        //symmetrisch
                        Debug.Log("gleich" + currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX + " " + neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX);
                    }
                    else if (currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX.Contains("f"))
                    {
                        Debug.Log("is flipped");
                        //Check if neighbor is none flipped
                        if(currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX == neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX + "f")
                        {
                            Debug.Log("is sym");
                        }
                    }
                    else if(isNoneFlipped)
                    {
                        Debug.Log("is none flipped");
                        
                        //Check if Neighbor is flipped
                        if (currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX + "f" == neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX)
                        {
                            Debug.Log("is sym");
                        }
                    }
                    else if(currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX.Contains("v"))
                    {
                        //muss noch verallgemeinert werden
                        //is vertical
                        if (currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX == neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA].nX)
                        {
                            //both vertical
                        }

                    }
                    else
                    {
                        //Sie sind ungleich und damit raus
                        Debug.Log("Delete");
                       neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);

                    }
                
                }
            }*/



}
