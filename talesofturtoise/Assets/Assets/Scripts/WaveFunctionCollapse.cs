using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
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
    public List<GameObject> neighborsList; //Liste der gefunden Nachbarn
    public List<GameObject> lowestGridCellList; //Liste mit dem lowest Entropy
    public List<GameObject> collapsedGridCellList; //Liste der besetzten Cells

    [Header ("Ground Generation")]
    //This will be replaced by the worlds atmosphere
    [SerializeField] private GameObject waterTile;
    [SerializeField] private GameObject waterParent;
    private GameObject currentWaterElement;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
           FindLowestEntropy();
        }
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

       FindLowestEntropy();
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
        Debug.Log("Count of not collapsed " +validGridCellList.Count);

        //check if its the first placment
        if(gridList.Count == validGridCellList.Count)
        {
            //First Tile, vorerst setze Gras Tile
            //Dies kann ersetzt werden durch Rahmen oder �hnliche vor definierte Level, ggf. auch nur die inneren position oder eine konkrete position als start
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
            //F�ge Cell zur Liste hinzu
            collapsedGridCellList.Add(gridList[firstGridCell]);

            CheckNeighbors();
        }
        else if(validGridCellList.Count == 0)
        {
            //Completed Placement
            //setze level laden auf true aus dem loadings screen level -> async level loading in einer anderen szene beenden, genshin impact intro
            Debug.Log("done");
        }
        else
        {
            if (validGridCellList != null) validGridCellList.Clear();
            for (int g = 0; g < gridList.Count; g++)
            {
                if (gridList[g].gameObject.GetComponent<Cell>().collapsed == false && gridList[g].gameObject.GetComponent<Cell>().isNeighbor == true)
                {
                    validGridCellList.Add(gridList[g]);
                }
            }
            Debug.Log("M�gliche NAchbarn Liste -------------------------" + validGridCellList.Count);
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
            //Debug.Log("1. Lowest Entropy " + lowestEntropy);

            //Then generate List of all lowest Entropy
            if (lowestGridCellList != null) lowestGridCellList.Clear();
            for (int g = 0; g < validGridCellList.Count; g++)
            {
                if (lowestEntropy == validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy()) //ggf entropy variable nutzen
                {
                    lowestGridCellList.Add(validGridCellList[g]);
                   // Debug.Log(" 2. M�gliche Cell " + validGridCellList[g].gameObject.name);
                }
            }
            //Debug.Log("3. Auswahlanzahl " + lowestGridCellList.Count);
            //Choose a Random of the lowest Entropy Grid Cells
            int randGridCell = UnityEngine.Random.Range(0, lowestGridCellList.Count);
            //Debug.Log("4. Indexwahl " + randGridCell + " ist " + lowestGridCellList[randGridCell].name);
            
            //Place a Random Tile add this GridCell
            PlaceTile(randGridCell);
        }

    }

    public void PlaceTile(int randIndex)
    {
        //Choose a random Tile of the chosen Grid Cell and place it
        lowestGridCellList[randIndex].gameObject.GetComponent<Cell>().ChooseRandomTile();
        //f�ge Grid Cell zur Liste hinzu -> ggf von cells steuern
        collapsedGridCellList.Add(lowestGridCellList[randIndex]);

        CheckNeighbors();
    }

    public void CheckNeighbors()
    {
        //Durchlaufe alle collapsed Cells
        for (int i = 0; i < collapsedGridCellList.Count; i++)
        {
            GameObject currentGridCell = collapsedGridCellList[i];
            string currentSocket;
            //vielleicht hat er wegen der rotation probleme -> test die rotation zu setzen
            //Nachbar Pos X----------------------------------------------------------------------------------------------- Pos X == Neg X?
            try { 
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX;
                Vector3 neighborFrontPos = currentGridCell.transform.position + new Vector3(1, 0, 0);

                PosXNeighbor(currentSocket, neighborFrontPos);
            }
            catch
            {
                //kein nachbar
                // Debug.Log("kein nachbar");
            }

            //Nachbar Neg X ----------------------------------------------------------------------------------------------- Neg X == Pos X?
            try 
            {
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nX;
                Vector3 neighborBackPos = currentGridCell.transform.position + new Vector3(-1, 0, 0);


                NegXNeighbor(currentSocket, neighborBackPos);
            }
            catch
            {
                //kein nachbar
                //Debug.Log("kein nachbar");
            }

            //Nachbar Pos Z ----------------------------------------------------------------------------------------------- Pos Z == Neg Z?
            try
            {
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pZ;
                Vector3 neighborRightPos = currentGridCell.transform.position + new Vector3(0, 0, 1);

                PosZNeighbor(currentSocket, neighborRightPos);
            }
            catch
            {
                //kein nachbar
                // Debug.Log("kein nachbar");
            }


            //Nachbar Neg Z ----------------------------------------------------------------------------------------------- Neg Z == Pos Z?
            try
            {
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nZ;
                Vector3 neighborLeftPos = currentGridCell.transform.position + new Vector3(0, 0, -1);

                NegZNeighbor(currentSocket, neighborLeftPos);
            }
            catch
            {
                //kein nachbar
                //Debug.Log("kein nachbar");
            }


            //Nachbar Pos Y ----------------------------------------------------------------------------------------------- Pos Y == Neg Y?
            try
            {
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pY;
                Vector3 neighborTopPos = currentGridCell.transform.position + new Vector3(0, 1, 0);

                PosYNeighbor(currentSocket, neighborTopPos);
            }
            catch
            {
                //kein nachbar
                // Debug.Log("kein nachbar");
            }

            //Nachbar Neg Y  ----------------------------------------------------------------------------------------------- Neg Y == Pos Y?
            try
            {
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().validNeighbors[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nY;
                Vector3 neighborBottomPos = currentGridCell.transform.position + new Vector3(0, -1, 0);

                NegYNeighbor(currentSocket, neighborBottomPos);
            }
            catch
            {
                //kein nachbar
                //Debug.Log("kein nachbar");
            }



            
        }
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
       // FindLowestEntropy();
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
        if (neighborPosX != null && neighborPosX.gameObject.GetComponent<Cell>().collapsed == false)
        {
           // Debug.Log("Check +X");

            //f�ge nachbar zur liste zu
            neighborPosX.gameObject.GetComponent<Cell>().isNeighbor = true;
            
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von current mit sllen m�glichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets         
            for (int xA = 0; xA < neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                TileData currentNeighborTile = neighborPosX.gameObject.GetComponent<Cell>().validNeighbors[xA];
                string neighborSocket = currentNeighborTile.nX; //Tile Data Object aus der Liste und von dieser Tile Data der Negativ X Nachbar als gegen Socket zum Current von Positiv

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch zb 0s
                }
                else if (currentSocket.Contains("f")) 
                {
                    //is flipped
                    //Check if neighbor is none flipped
                    if(currentSocket == neighborSocket + "f")
                    {
                        //symmetrisch zb 1f und 1
                    }
                }
                else if(isNoneFlipped) 
                {
                    //Debug.Log("is none flipped");
                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        //symmetrisch 1 und 1f
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
                    //ungleich
                    invalidNeighborList.Add(currentNeighborTile);
                    //neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                }

            }

            foreach(TileData currentNeighborTile in invalidNeighborList)
            {
                neighborPosX.gameObject.GetComponent<Cell>().validNeighbors.Remove(currentNeighborTile);
                Debug.Log("+X " + currentSocket + " " + currentNeighborTile.nX);
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
        if (neighborNegX != null && neighborNegX.gameObject.GetComponent<Cell>().collapsed == false)
        {
            //Debug.Log("Check -X");
            //f�ge nachbar zur liste zu
            neighborNegX.gameObject.GetComponent<Cell>().isNeighbor = true;

            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von current mit sllen m�glichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets
            for (int xA = 0; xA < neighborNegX.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                TileData currentNeighborTile = neighborNegX.gameObject.GetComponent<Cell>().validNeighbors[xA];
                string neighborSocket = currentNeighborTile.pX;

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    //is flipped
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        //symmetrisch
                    }
                }
                else if (isNoneFlipped)
                {
                    //Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        //symmetrisch
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
                    //ungleich
                    //neighborNegX.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                    invalidNeighborList.Add(currentNeighborTile);
                }

            }

            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborNegX.gameObject.GetComponent<Cell>().validNeighbors.Remove(currentNeighborTile);
                Debug.Log("-X " + currentSocket + " " + currentNeighborTile.pX);
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
        if (neighborPosZ != null && neighborPosZ.gameObject.GetComponent<Cell>().collapsed == false)
        {
            //Debug.Log("Check +Z");
            //f�ge nachbar zur liste zu
            neighborPosZ.gameObject.GetComponent<Cell>().isNeighbor = true;

            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von current mit sllen m�glichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets
            for (int xA = 0; xA < neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                TileData currentNeighborTile = neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors[xA];
                string neighborSocket = currentNeighborTile.nZ;

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    //is flipped
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        //symmetrisch
                    }
                }
                else if (isNoneFlipped)
                {
                    //Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        //symmetrisch
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
                    //ungleich
                    //neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                    invalidNeighborList.Add(currentNeighborTile);
                }

            }
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborPosZ.gameObject.GetComponent<Cell>().validNeighbors.Remove(currentNeighborTile);
                Debug.Log("+Z " + currentSocket + " " + currentNeighborTile.nZ);
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
        if (neighborNegZ != null && neighborNegZ.gameObject.GetComponent<Cell>().collapsed == false)
        {
            //Debug.Log("Check -Z");
            //f�ge nachbar zur liste zu
            neighborNegZ.gameObject.GetComponent<Cell>().isNeighbor = true;

            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von current mit sllen m�glichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets
            for (int xA = 0; xA < neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                TileData currentNeighborTile = neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors[xA];
                string neighborSocket = currentNeighborTile.pZ;

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    //is flipped
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        //symmetrisch
                    }
                }
                else if (isNoneFlipped)
                {
                    //Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        //symmetrisch
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
                    //ungleich
                   // neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                    invalidNeighborList.Add(currentNeighborTile);
                }

            }
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborNegZ.gameObject.GetComponent<Cell>().validNeighbors.Remove(currentNeighborTile);
                Debug.Log("-Z " + currentSocket + " " + currentNeighborTile.pZ);
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
        if (neighborPosY != null && neighborPosY.gameObject.GetComponent<Cell>().collapsed == false )
        {
            // Debug.Log("Check +Y");
            //f�ge nachbar zur liste zu
            neighborPosY.gameObject.GetComponent<Cell>().isNeighbor = true;

            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von current mit sllen m�glichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets
            for (int xA = 0; xA < neighborPosY.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                TileData currentNeighborTile = neighborPosY.gameObject.GetComponent<Cell>().validNeighbors[xA];
                string neighborSocket = currentNeighborTile.nY;

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    //is flipped
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        //symmetrisch
                    }
                }
                else if (isNoneFlipped)
                {
                    //Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        //symmetrisch
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
                    //ungleich
                    //neighborPosY.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                    invalidNeighborList.Add(currentNeighborTile);
                }
            }
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborPosY.gameObject.GetComponent<Cell>().validNeighbors.Remove(currentNeighborTile);
                Debug.Log("+Y " + currentSocket + " " + currentNeighborTile.nY);
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
        if (neighborNegY != null && neighborNegY.gameObject.GetComponent<Cell>().collapsed == false)
        {
            //Debug.Log("Check -Y");
            neighborNegY.gameObject.GetComponent<Cell>().isNeighbor = true;

            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von current mit sllen m�glichen  NegX Socket der NAchbarliste und streiche ungleiche Sockets

            for (int xA = 0; xA < neighborNegY.gameObject.GetComponent<Cell>().validNeighbors.Count; xA++)
            {
                TileData currentNeighborTile = neighborNegY.gameObject.GetComponent<Cell>().validNeighbors[xA];
                string neighborSocket = currentNeighborTile.pY;

                //check if its none flipped => only digits 
                bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);
                
                //wenn Pos X == Neg X dann symetrisch => zahl+s
                if (currentSocket == neighborSocket)
                {
                    //symmetrisch
                }
                else if (currentSocket.Contains("f"))
                {
                    //is flipped
                    //Check if neighbor is none flipped
                    if (currentSocket == neighborSocket + "f")
                    {
                        //symmetrisch
                    }
                }
                else if (isNoneFlipped)
                {
                    // Debug.Log("is none flipped");

                    //Check if Neighbor is flipped
                    if (currentSocket + "f" == neighborSocket)
                    {
                        //symmetrisch
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
                    //ungleich
                    //neighborNegY.gameObject.GetComponent<Cell>().validNeighbors.RemoveAt(xA);
                    invalidNeighborList.Add(currentNeighborTile);
                }

            }
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborNegY.gameObject.GetComponent<Cell>().validNeighbors.Remove(currentNeighborTile);
                Debug.Log("-Y " + currentSocket + " " + currentNeighborTile.pY);
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

}
