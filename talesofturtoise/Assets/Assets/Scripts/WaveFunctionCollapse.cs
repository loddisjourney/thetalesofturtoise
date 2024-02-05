using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveFunctionCollapse : MonoBehaviour
{
    /*
     * Level Generator
     * Hier wird eine eigene Version der Wave Function Collapse implementiert. 
     * *
     */
    
    public GameManager gameManager;

    [Header("Level Info")]
    private int width = 10; //z
    private int height = 1; //y
    private int length = 10; //x

    private GameObject[,,] gridArray;

    [Header("Grid Info")]
    public Cell cell;
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

    [SerializeField] private GameObject cellParent;

    [SerializeField] private Material skybox1;

    public static bool levelGenerated = false;

    public GameObject _player;
    public GameObject _camera;

    //Nur fuer den Protoyp
    public GameObject restartButton;
    public void RestartGame()
    {
        SceneManager.LoadScene("PlayerHome");
    }

    // Start is called before the first frame update
    void Start()
    {
        gridArray = new GameObject[length, height, width];
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        /*Debug Schritt fuer Schritt platzierung eines Tiles
        if (Input.GetKeyDown(KeyCode.K))
        {       
            ManageCollapse();
        }
        */

        // Unload Loading Scene
        if (levelGenerated)
        {
            Debug.Log("unloading...");
            SceneManager.UnloadSceneAsync("LoadingLevel");
            //levelGenerated = true;
            _camera.SetActive(true);
            _player.SetActive(true);
            restartButton.SetActive(true);
            levelGenerated = false;
            RenderSettings.skybox = skybox1;
        }
    }

    /*
     * Basis fuer die Wave Function Collapse:
     * Ein Grid wird zur Runtime nach height, length und width erstellt -> dies soll spaeter beim Generieren nicht immer gleich sein, sondern nach bestimmten Situtation anpassen.
     * Fuer jedes Gridelement wird eine Zelle (Cell) instanziiert und in einer Liste festgehalten.
     * Fuer eine bessere Uebersicht wird jede Zelle unter einem Parent gesetzt.
     * 
     * (noch nicht vollstaendig implementiert) in einem Array werden alle Posiitonen der Zellen gespeichert, welche dann fuer die Verebreitung der Regeln nach neuer Platzeirung statt aktuellen Pruefung vernwedet werden soll.
     * 
     * **/
    public void GenerateGrid()
    {    
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < length; x++)
            {
                for(int z = 0; z < width; z++)
                {
                    //Create a Grid Cell at x y z
                    currentCell = Instantiate(gridCell, new Vector3(x, y, z), Quaternion.identity);
                    currentCell.name = $"Cell {x} {y} {z}";
                    currentCell.transform.parent = cellParent.transform;
                    gridList.Add(currentCell);
                    gridArray[x,y,z] = currentCell;
                    //collapsedGridCellList[i].x = x;
                    //collapsedGridCellList[i].y = y;
                    //collapsedGridCellList[i].z = z;
                }
            }
        }

        //Voerst: Geneeriere unter dem Grid eine Wasserebene
        //Dies soll spaeter durch ganze Levelrahmen ausgetasucht werden
        for(int x2 = 0; x2 < length; x2++)
        {
            for(int z2 = 0;z2 < width; z2++)
            {
                currentWaterElement = Instantiate(waterTile, new Vector3(x2, length - (length+1), z2), Quaternion.identity);
                currentWaterElement.transform.parent = waterParent.transform;
            }
        }
        
        //Beginne die Wave Function Collapse
        ManageCollapse();
    }

    /*
     * In dieser Funktion wird gepreuft wie viele Zellen noch nicht gefuellt wurden. Abhanegig davon wird die erste Zelle ausgeawehlt, eine naechste geawehlt oder die Generierung beendet.
     * 
     * *
     */
    public void ManageCollapse()
    {
        /*
         * Zuerst wird eine neue Liste erstellt inder alle nicht gefuellten Zellen rein kommen.
         * *
         */
        if (validGridCellList != null) validGridCellList.Clear();
        for (int g = 0; g < gridList.Count; g++)
        {
            if(gridList[g].gameObject.GetComponent<Cell>().collapsed == false)
            {
                validGridCellList.Add(gridList[g]);
            }
        }
        //Debug.Log("Count of not collapsed " +validGridCellList.Count);

        /*
         * Die groeße der nicht besetzten Zellen wird geprueft.
         * Ist sie genauso groß wie die Anzahl der Gridelemente, so wurde enoch keine Zelle gefuellt. Es wird die erste Zelle zum Fuellen ausgeweaehlt.
         * Ist sie leer, so wurden alle Zellen gefuellt. Das Generieren ist beendet.
         * Andernfalls wird eine neue Zelle gewaehlt.
         * **/
        if(gridList.Count == validGridCellList.Count)
        {
            //Waehle die erste Zelle. Diese soll vorerst  immer in der untersten Ebene des Grid sein. Die For-Schleife koennte durch das fertige GridArray optimiert werden.
            //Dies kann spaeter durch Rahmen oder aehnliche vor definierte Level ersetzt werden. Diese wuerden dann aehnlich zur Model Synthesis nach dem Prinzip des Modifiying in Blocks einen Startrahmen geben.
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
            //Rufe die Cell Methode zum Platzieren des ersten Tiles auf
            gridList[firstGridCell].gameObject.GetComponent<Cell>().FirstTile();
            //Danach wird diese Zelle zur besetzte Zellen Liste hinzugefuegt
            collapsedGridCellList.Add(gridList[firstGridCell]);
            //Nachdem Platzieren muss die Info an die Nachbarn weiter gegeben bzw. die Regeln angewendet werden
            CheckNeighbors();
        }
        else if(validGridCellList.Count == 0)
        {
            //Wave Function Collapse ist beendet
            //Gebe die Info an den LevelLoader weiter, um dort das Endstueck zu platzieren (momentane Loesung)
            LoadingManager.levelLoaded = true;
        }
        else
        {
            //Waehele die naechste Zelle aus der Liste der moeglichen Zellen zum Fuellen aus, diese soll allerdings nur aus den Nachbarn der platzierten Zellen bestehen, da nazunhemen ist, dass diese wneiger Moeglichkeiten haben und daher zuerst besetzt werden sollen
            if (validGridCellList != null) validGridCellList.Clear();
            for (int g = 0; g < gridList.Count; g++)
            {
                if (gridList[g].gameObject.GetComponent<Cell>().collapsed == false && gridList[g].gameObject.GetComponent<Cell>().isNeighbor == true)
                {
                    validGridCellList.Add(gridList[g]);
                }
            }

            //Berechne fuer jeden dieser Nachbarn die Entropy aus und finde die kleinste Entropy
            float lowestEntropy = float.PositiveInfinity; //Da die heochste Entropy nicht bekannt ist, wird sie zeurst auf unedlich gestzt, 28.12.2023 gefunden unter: https://forum.unity.com/threads/finding-the-index-of-the-lowest-valued-element-in-an-array.295195/
            for (int g = 0; g < validGridCellList.Count; g++)
            {
                float currentEntropy;
                currentEntropy = validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy();

                if (currentEntropy < lowestEntropy)
                {
                    lowestEntropy = currentEntropy;
                }
            }
            //Debug.Log("Lowest Entropy " + lowestEntropy);

            //Da es moeglicherweise mehrere Zellen mit der kleisnten Entropy gibt, sollen diese zuerst in eine neue Liste gefuellt werden.
            if (lowestGridCellList != null) lowestGridCellList.Clear();
            for (int g = 0; g < validGridCellList.Count; g++)
            {
                if (lowestEntropy == validGridCellList[g].gameObject.GetComponent<Cell>().CalculateEntropy()) //ggf entropy variable nutzen
                {
                    lowestGridCellList.Add(validGridCellList[g]);
                    //Debug.Log("Moegliche Cell " + validGridCellList[g].gameObject.name + " mit Entropy von + validGridCellList[g].gameObject.GetComponent<Cell>().entropy);
                }
            }
            //Debug.Log("Anzahl der Cells mit der kleinsten Entropy " + lowestGridCellList.Count);
            
            //Wahle zufaellig einer dieser Zellen aus
            int randGridCell = UnityEngine.Random.Range(0, lowestGridCellList.Count);
            //Debug.Log("Neue Zelle " + randGridCell + " ist " + lowestGridCellList[randGridCell].name);
            
            //Platziere ein Tile in die gewaehlte Zelle
            PlaceTile(randGridCell);
        }

    }

    public void PlaceTile(int randIndex)
    {
        //Rufe die Cell Methode zum Platzieren des naechsten Tiles auf
        lowestGridCellList[randIndex].gameObject.GetComponent<Cell>().ChooseRandomTile();
        //Danach wird diese Zelle zur besetzte Zellen Liste hinzugefuegt
        collapsedGridCellList.Add(lowestGridCellList[randIndex]);
        //Nachdem Platzieren muss die Info an die Nachbarn weiter gegeben bzw. die Regeln angewendet werden
        CheckNeighbors();
    }

   /*
    * Jetzt wird die Info des platzierten neuen Tile an die Nachbarn weiter gegeben und die Regeln angewendet, um nicht passende Tiles zu entfernen -> dies koennte optimiert werden nach der AC-3, aktuell ist es eine optimierte AC-1
    * *
    */

    public void CheckNeighbors()
    {
        //Durchlaufe alle besetzten Tiles und pruefe ihre Nachbarn in alle Richtungen
        for (int i = 0; i < collapsedGridCellList.Count; i++)
        {
            GameObject currentGridCell = collapsedGridCellList[i];
            //Fuer eine spaetere Implementierung des Grid Arrays -> so muss nicht direkt die position verwendet werden
            //GameObject neighborObject;
            //int x = collapsedGridCellList[i].x;
            //int y = collapsedGridCellList[i].y;
            //int z = collapsedGridCellList[i].z;

            string currentSocket;

            //Wenn der jeweilige Nachbar existiert, dann pruefe ihn, sonst fange den Error ab bzw. ueberspringe ihn

            //Nachbar Pos X----------------------------------------------------------------------------------------------- Pos X == Neg X?
            try
            { 
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().tilesProbabilityList[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pX;
                Vector3 neighborFrontPos = currentGridCell.transform.position + new Vector3(1, 0, 0);
                //neighborObject = gridArray[x + 1, y, z];
                PosXNeighbor(currentSocket, neighborFrontPos);
            }
            catch
            {
                //kein nachbar
                //Debug.Log("kein nachbar");
            }

            //Nachbar Neg X ----------------------------------------------------------------------------------------------- Neg X == Pos X?
            try 
            {
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().tilesProbabilityList[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nX;
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
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().tilesProbabilityList[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pZ;
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
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().tilesProbabilityList[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nZ;
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
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().tilesProbabilityList[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].pY;
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
                currentSocket = currentGridCell.gameObject.GetComponent<Cell>().tilesProbabilityList[currentGridCell.gameObject.GetComponent<Cell>().collapsedTile].nY;
                Vector3 neighborBottomPos = currentGridCell.transform.position + new Vector3(0, -1, 0);

                NegYNeighbor(currentSocket, neighborBottomPos);
            }
            catch
            {
                //kein nachbar
                //Debug.Log("kein nachbar");
            }
            
        }
        //Fuehre die Wave Function Collpase fort
        ManageCollapse();
    }

    /*
     * Da der Nachbar der aktuellen besetzten Zelle exisitiert, wird geprueft, ob dieser bereits besetzt ist, wenn ja, wird dieser uebersprungen
     * Aktuell wird dies noch separat fuer jede Richtung gemacht, bessere waere es, eine Loesung zu kreiren, die das verallgemeinert. Einzelne Parts konnten bisher schon in zwei Methoden verallgemeinert werden: CompareSockets() und ContainsOnlyDigitCheck()
     * *
     */
    public void PosXNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Wird ggf. durch das GridArray umgeschrieben
        //Zuerst wird geperueft, ob es einen Nachbar in die Richtung in der GridListe gibt
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
            //Wenn dieser Nachbar existiert und noch nicht besetzt ist, wird er als Nachbar markiert
            neighborPosX.gameObject.GetComponent<Cell>().isNeighbor = true;
            
            //In dieser Liste werden alle nicht passenden Tiles gesammelt, um sie dann zu Löschen, ohne dabei die Reihenfolge der moegliche Tile Liste der Cell waehrend der Pruefung zu veraendern
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosX Socket von der aktuellen besetzten Zelle mit allen moeglichen NegX Socket der Tile Liste vom Nachbar        
            for (int xA = 0; xA < neighborPosX.gameObject.GetComponent<Cell>().validTilesList.Count; xA++)
            {
                TileData currentNeighborTile = neighborPosX.gameObject.GetComponent<Cell>().validTilesList[xA];
                string neighborSocket = currentNeighborTile.nX; //Tile Object aus der Liste und von diesem Tile der Negativ X Nachbar

                //pruefe, ob vom Nachbar Tile das Socket passt
                bool sym = CompareSockets(currentSocket, neighborSocket);

                //Wenn nicht fuege es zu den nicht passenden Nachbarn zu
                if(!sym)
                {
                    invalidNeighborList.Add(currentNeighborTile);
                   // Debug.Log("ungleich");
                } 

            }
            //Nach der Pruefung aller Tiles vom Nachbarn, loesche alle, die nicht passen
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborPosX.gameObject.GetComponent<Cell>().validTilesList.Remove(currentNeighborTile);
               // Debug.Log("+X " + currentSocket + " " + currentNeighborTile.nX + " ");
            }
        }
    }

    public void NegXNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Wird ggf. durch das GridArray umgeschrieben
        //Zuerst wird geperueft, ob es einen Nachbar in die Richtung in der GridListe gibt
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
            //Wenn dieser Nachbar existiert und noch nicht besetzt ist, wird er als Nachbar markiert
            neighborNegX.gameObject.GetComponent<Cell>().isNeighbor = true;

            //In dieser Liste werden alle nicht passenden Tiles gesammelt, um sie dann zu Löschen, ohne dabei die Reihenfolge der moegliche Tile Liste der Cell waehrend der Pruefung zu veraendern
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das NegX Socket von der aktuellen besetzten Zelle mit allen moeglichen PosX Socket der Tile Liste vom Nachbar 
            for (int xA = 0; xA < neighborNegX.gameObject.GetComponent<Cell>().validTilesList.Count; xA++)
            {
                TileData currentNeighborTile = neighborNegX.gameObject.GetComponent<Cell>().validTilesList[xA];
                string neighborSocket = currentNeighborTile.pX;//Tile Object aus der Liste und von diesem Tile der Positv X Nachbar

                //pruefe, ob vom Nachbar Tile das Socket passt
                bool sym = CompareSockets(currentSocket, neighborSocket);

                //Wenn nicht fuege es zu den nicht passenden Nachbarn zu
                if (!sym)
                {
                    invalidNeighborList.Add(currentNeighborTile);
                   // Debug.Log("ungleich");
                }

            }
            //Nach der Pruefung aller Tiles vom Nachbarn, loesche alle, die nicht passen
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborNegX.gameObject.GetComponent<Cell>().validTilesList.Remove(currentNeighborTile);
               // Debug.Log("-X " + currentSocket + " " + currentNeighborTile.pX);
            }
        }
    }

    public void PosZNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Wird ggf. durch das GridArray umgeschrieben
        //Zuerst wird geperueft, ob es einen Nachbar in die Richtung in der GridListe gibt
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
            //Wenn dieser Nachbar existiert und noch nicht besetzt ist, wird er als Nachbar markiert
            neighborPosZ.gameObject.GetComponent<Cell>().isNeighbor = true;

            //In dieser Liste werden alle nicht passenden Tiles gesammelt, um sie dann zu Löschen, ohne dabei die Reihenfolge der moegliche Tile Liste der Cell waehrend der Pruefung zu veraendern
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosZ Socket von der aktuellen besetzten Zelle mit allen moeglichen NegZ Socket der Tile Liste vom Nachbar 
            for (int xA = 0; xA < neighborPosZ.gameObject.GetComponent<Cell>().validTilesList.Count; xA++)
            {
                TileData currentNeighborTile = neighborPosZ.gameObject.GetComponent<Cell>().validTilesList[xA];
                string neighborSocket = currentNeighborTile.nZ;//Tile Object aus der Liste und von diesem Tile der Negativ Z Nachbar

                //pruefe, ob vom Nachbar Tile das Socket passt
                bool sym = CompareSockets(currentSocket, neighborSocket);

                //Wenn nicht fuege es zu den nicht passenden Nachbarn zu
                if (!sym)
                {
                    invalidNeighborList.Add(currentNeighborTile);
                   // Debug.Log("ungleich");
                }

            }
            //Nach der Pruefung aller Tiles vom Nachbarn, loesche alle, die nicht passen
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborPosZ.gameObject.GetComponent<Cell>().validTilesList.Remove(currentNeighborTile);
               // Debug.Log("+Z " + currentSocket + " " + currentNeighborTile.nZ);
            }
        }
    }

    public void NegZNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Wird ggf. durch das GridArray umgeschrieben
        //Zuerst wird geperueft, ob es einen Nachbar in die Richtung in der GridListe gibt
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
            //Wenn dieser Nachbar existiert und noch nicht besetzt ist, wird er als Nachbar markiert
            neighborNegZ.gameObject.GetComponent<Cell>().isNeighbor = true;

            //In dieser Liste werden alle nicht passenden Tiles gesammelt, um sie dann zu Löschen, ohne dabei die Reihenfolge der moegliche Tile Liste der Cell waehrend der Pruefung zu veraendern
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das NegZ Socket von der aktuellen besetzten Zelle mit allen moeglichen PosZ Socket der Tile Liste vom Nachbar 
            for (int xA = 0; xA < neighborNegZ.gameObject.GetComponent<Cell>().validTilesList.Count; xA++)
            {
                TileData currentNeighborTile = neighborNegZ.gameObject.GetComponent<Cell>().validTilesList[xA];
                string neighborSocket = currentNeighborTile.pZ;//Tile Object aus der Liste und von diesem Tile der Positiv Z Nachbar

                //pruefe, ob vom Nachbar Tile das Socket passt
                bool sym = CompareSockets(currentSocket, neighborSocket);

                //Wenn nicht fuege es zu den nicht passenden Nachbarn zu
                if (!sym)
                {
                    invalidNeighborList.Add(currentNeighborTile);
                  //  Debug.Log("ungleich");
                }

            }
            //Nach der Pruefung aller Tiles vom Nachbarn, loesche alle, die nicht passen
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborNegZ.gameObject.GetComponent<Cell>().validTilesList.Remove(currentNeighborTile);
               // Debug.Log("-Z " + currentSocket + " " + currentNeighborTile.pZ);
            }
        }
    }

    public void PosYNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Wird ggf. durch das GridArray umgeschrieben
        //Zuerst wird geperueft, ob es einen Nachbar in die Richtung in der GridListe gibt
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
            //Wenn dieser Nachbar existiert und noch nicht besetzt ist, wird er als Nachbar markiert
            neighborPosY.gameObject.GetComponent<Cell>().isNeighbor = true;

            //In dieser Liste werden alle nicht passenden Tiles gesammelt, um sie dann zu Löschen, ohne dabei die Reihenfolge der moegliche Tile Liste der Cell waehrend der Pruefung zu veraendern
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das PosY Socket von der aktuellen besetzten Zelle mit allen moeglichen NegY Socket der Tile Liste vom Nachbar 
            for (int xA = 0; xA < neighborPosY.gameObject.GetComponent<Cell>().validTilesList.Count; xA++)
            {
                TileData currentNeighborTile = neighborPosY.gameObject.GetComponent<Cell>().validTilesList[xA];
                string neighborSocket = currentNeighborTile.nY;//Tile Object aus der Liste und von diesem Tile der Negativ Y Nachbar

                //pruefe, ob vom Nachbar Tile das Socket passt
                bool sym = CompareSockets(currentSocket, neighborSocket);

                //Wenn nicht fuege es zu den nicht passenden Nachbarn zu
                if (!sym)
                {
                    invalidNeighborList.Add(currentNeighborTile);
                  //  Debug.Log("ungleich");
                }

            }
            //Nach der Pruefung aller Tiles vom Nachbarn, loesche alle, die nicht passen
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborPosY.gameObject.GetComponent<Cell>().validTilesList.Remove(currentNeighborTile);
               // Debug.Log("+Y " + currentSocket + " " + currentNeighborTile.nY);
            }
        }
    }

    public void NegYNeighbor(string currentSocket, Vector3 neighborPos)
    {
        //Wird ggf. durch das GridArray umgeschrieben
        //Zuerst wird geperueft, ob es einen Nachbar in die Richtung in der GridListe gibt
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
            //Wenn dieser Nachbar existiert und noch nicht besetzt ist, wird er als Nachbar markiert
            neighborNegY.gameObject.GetComponent<Cell>().isNeighbor = true;

            //In dieser Liste werden alle nicht passenden Tiles gesammelt, um sie dann zu Löschen, ohne dabei die Reihenfolge der moegliche Tile Liste der Cell waehrend der Pruefung zu veraendern
            List<TileData> invalidNeighborList = new List<TileData>();

            //Vergleiche das NegY Socket von der aktuellen besetzten Zelle mit allen moeglichen PosY Socket der Tile Liste vom Nachbar 
            for (int xA = 0; xA < neighborNegY.gameObject.GetComponent<Cell>().validTilesList.Count; xA++)
            {
                TileData currentNeighborTile = neighborNegY.gameObject.GetComponent<Cell>().validTilesList[xA];
                string neighborSocket = currentNeighborTile.pY;//Tile Object aus der Liste und von diesem Tile der Positiv Y Nachbar

                //pruefe, ob vom Nachbar Tile das Socket passt
                bool sym = CompareSockets(currentSocket, neighborSocket);

                //Wenn nicht fuege es zu den nicht passenden Nachbarn zu
                if (!sym)
                {
                    invalidNeighborList.Add(currentNeighborTile);
                   // Debug.Log("ungleich");
                }

            }
            //Nach der Pruefung aller Tiles vom Nachbarn, loesche alle, die nicht passen
            foreach (TileData currentNeighborTile in invalidNeighborList)
            {
                neighborNegY.gameObject.GetComponent<Cell>().validTilesList.Remove(currentNeighborTile);
              //  Debug.Log("-Y " + currentSocket + " " + currentNeighborTile.pY);
            }
        }
    }

    /*
     * Vergleicht die Sockets auf den festgelegten regeln und gibt einen boolean zurueck
     * *
     */
    public bool CompareSockets(string currentSocket, string neighborSocket)
    {
        //pruefe auf none flipped => also ob nur eine ID im Socket ist 
        bool isNoneFlipped = ContainsOnlyDigitCheck(currentSocket);

        //pruefe, ob Pos X == Neg X und ein s enthalten, dann sind es gleiche symetrischee Socket
        if (currentSocket == neighborSocket && currentSocket.Contains("s"))
        {
            //passen zb 0s 0s
            return true;
        }
        else if (currentSocket.Contains("f"))
        {
            //is flipped
            //pruefe, ob Nachbar ein nicht gespeigeltes Socket hat und passen wuerde
            if (currentSocket == neighborSocket + "f")
            {
                //passen zb 1f und 1
                return true;

            }
        }
        else if (isNoneFlipped)
        {
            //pruefe, ob Nachbar ein gespeigeltes Socket hat und passen wuerde

            if (currentSocket + "f" == neighborSocket)
            {
                //passen zb 1 und 1f
                return true;
            }
        }
        else if (currentSocket.Contains("v"))
        {
            //muss noch verallgemeinert werden
            //ist vertical
            if (currentSocket == neighborSocket)
            {
                //beide vertikal
                return true;
            }
        }
        else
        {
            //ungleich
            return false;
        }
        return false;
    }

    /*
     * 
     * Die Funktion prueft, ob der String nur aus 0-9 besteht
     * am 30.12.2024 gefunden unter https://learn.microsoft.com/de-de/dotnet/api/system.char.isdigit?view=net-8.0
     * *
     */
    public bool ContainsOnlyDigitCheck(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if(!char.IsDigit(c)) return false;
        }
        return true;
    }

}
