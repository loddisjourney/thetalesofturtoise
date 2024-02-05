using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    /*
     * Grid Zelle, welche es mit der Wave Function Collapse zu fuellen gilt
     * 
     * Entropy ist der Wert des errechneten Lowest Entropy
     * 
     * Liste der möglichen Tile besteht aus Array, welches automatisch alle Tiles aus einem Ordner liest, und einer Liste, welche dann vom Array gefuellt wird.
     * Sobald das Array oder die Liste vom Code angesprochen wird, wird es mit einem Getter initialisiert -> da es erst zur Runtime aus dem Ordner gefuellt wird.
     * Diese wird stetig von dem Wave Function Collapse Script aktualisert solange die Zelle ein Nachbar ist
     * 
     * Das Default Tile ist vorerst das, wenn auch nicht optimal, Error Handling es enthaelt eine leeres Tile 
     * 
     * Die Probability Tile Liste kommt erst beim Platzieren und Wahl eines konkreten Tiles. Sie fuellt sich mit jedem moeglichen Tile x dem Weight des Tiles (gewichteter Zufallswert)
     * In der Wave Function Collapse wird sie dann als Verweis auf das bestzte Tile verwendet (currentCell)
     * Das gewaehlte Tile wird durch den Index in der Liste in collapsedTile gespeichert
     * 
     * Es gibt zwei Boolean zur ueberpruefen, ob die Zelle schon besetzt wurde oder ein NAchbar einer besetzten Zelle ist.
     * 
     * mapParent dient lediglich der Ueberschaubarkeit in der Szene nach der Generierung, sodass dort die Zellen alle als Child platziert werden
     * 
     * *
     */
    
    public float entropy;

    //Sammle alle Valid Neighbors aus dem Ressource Ordner in ein Array und erstelle eine Liste daraus
    /*
     * Gefunden udn aufgerufen am 27.12.2023, unter: https://discussions.unity.com/t/add-prefabs-from-a-folder-to-array-as-gameobject/218233
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

    public List<TileData> tilesProbabilityList;

    public int collapsedTile;

    public bool collapsed = false;
    public bool isNeighbor = false;

    [SerializeField] private GameObject mapParent;



    private void Awake()
    {
        //Nicht optimal ggf. wenn es nicht gefunden wird eins erstellen unter LevelGenerator sammeln
        mapParent = GameObject.Find("LevelMap");
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (collapsed) isNeighbor = false;
        /* Debugging: Mache alle Nachbarn sichtbar indem ein Standard Cube Mesh eingeschaltet wird
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
        */
    }

    //Getter koennte ggf. nuetzlich werden, da fuer das befuellen der Liste, das Array nicht sichtbar sein soll
    public TileData[] GetTileData()
    { 
        return validTiles; 
    }


    /*
     * Berechnung der kleinsten Entropy nach der Shannon Entropy basierend aus der Formel von Boris Blog "Wave Function Collapse explained - Least Entropy" aufegerufen am 27.12.2023 unter https://www.boristhebrave.com/2020/04/13/wave-function-collapse-explained/
     * sowie dem YouTube Video von DV Gen, aufgerufen am 27.12.2023 unter https://youtu.be/zIRTOgfsjl0?si=J4xOnB-HwOMk4Caf&t=932
     * 
     * Berechnet die Entropy unter der Beruecksischtung der Anzahl der moeglichen Tiles sowie deren Wahrscheinlichkeit aus der gesammt Anzahl der Gewichtung
     * *
     */

    public float CalculateEntropy()
    {
        //validTilesList[i].weight ist die Wahrscheinlichkeit/der gewichtete Zufallswert, dass das Tile gewaehlt werden soll, je hoeher desto hoeher die Wahrscheinlichkeit
        //totalWeight die Summe aller gewichteter Zufallswerte der Tiles
        //Entropy ist das entgueltige Ergebnis
        entropy = 0;
        
        float totalWeight = 0;
        for(int i = 0; i < validTilesList.Count; i++)
        {
            totalWeight += validTilesList[i].weight;
        }

        //Die Entropy ist die Summe der Wahrscheinlichkeit der moeglichen Tiles multipliziert mit dem Logarithmus(2) der Wahrscheinlichkeit des Tiles
        //Die Wahrscheinlihckeit ist das Weight des aktuellen moeglichen Tiles durch die addierte totalWeight aller moeglichen Weights
        for (int i = 0; i < validTilesList.Count; i++)
        {
            float wahrscheinlichkeit = validTilesList[i].weight / totalWeight;
            entropy += wahrscheinlichkeit * Mathf.Log(wahrscheinlichkeit, 2);
        }

        //Debug.Log("Result of Entropy "+ -entropy + "Count of valid neighbors" + validTilesList.Count);
        //Das Ergebnis wird noch negiert
        return -entropy; 
    }

    /*
     * Wird aufgerufen sobald die Zelle ein konkretes Tiles zugeweisen bekommt, wenn es nicht die erste zu besetzene Zelle ist
     * Wenn die Liste der moeglichen Tiles leer ist, beduetet es kommt zu einem kritischen Zustand. Hier muesste ein konkretes Error Handling implementiert werden. Vorerst wird dies durch ein Default Luft Tile geloest.
     * *
     */
    public void ChooseRandomTile()
    {
        //Ist die Liste nicht leer soll ein Tile zufaellig gewaehlt werden
        if(validTilesList.Count != 0)
        {
            //Um die Weights zu beachten wird ein neues Array mit der jeweligen Tileanzahl nach dem Weight erstellt.
            for(int i = 0; i < validTilesList.Count; i++)
            {
                int probaility = validTilesList[i].weight;
                for(int j = 0; j < probaility; j++)
                {
                    tilesProbabilityList.Add(validTilesList[i]);
                }
            }
            
            //Waehle ein Tile aus der neuen Liste
            int randTile = UnityEngine.Random.Range(0, tilesProbabilityList.Count);
            
            //Platziere diese Tile auf die Position der akutellen Zelle und rotiere das Child Mesh nach der Definiton des Enum vom Tile
            GameObject chosenTile = tilesProbabilityList[randTile].meshObj;
            GameObject parentTile = Instantiate(chosenTile, this.transform.position, Quaternion.identity);
            GameObject child = parentTile.transform.GetChild(0).gameObject;
            child.transform.rotation = Quaternion.Euler(0, (int)tilesProbabilityList[randTile].tileRotation, 0);
            //Platziere das neue Tile unter die Level Map
            parentTile.transform.parent = mapParent.transform;
            //Speichere den gewaehlten Index des gewaehlten Tile
            collapsedTile = randTile;
        }
        else
        {
            //Problem: Tritt dieser Fall auf, so koennen die Nachbarn keine passenden Tiles mehr finden
            Instantiate(defaultTile.meshObj, this.transform.position, Quaternion.identity);

            //Liste muesste ein neues Tile erhalten um in der Nachbarpruefung ein Tile zu finden, wobei hier eine  Loesung implementiert werden muesste, dass -1 fuer alle horizontalen NAchbarn akzeptiert wird
            //collapsedTile = 0;
            //validTilesList.Add(defaultTile);
        }
        
        //Setze die Zelle auf besetzt
        collapsed = true;
        
    }

    /*
     * Wird aufgerufen sobald die erste Zelle ein konkretes Tiles zugeweisen bekommt
     * *
     */
    public void FirstTile()
    {

        //Um die Weights zu beachten wird ein neues Array mit der jeweligen Tileanzahl nach dem Weight erstellt.
        for (int i = 0; i < validTilesList.Count; i++)
        {
            int probaility = validTilesList[i].weight;
            for (int j = 0; j < probaility; j++)
            {
                tilesProbabilityList.Add(validTilesList[i]);
            }
        }

        //Voerst: 
        //Setze das erste Tile zu einem Basis GrasTile -> kann auch random sein
        int grasTile = 0;
        for(int i = 0; i < tilesProbabilityList.Count; i++)
        {
            if(tilesProbabilityList[i].meshObj.name =="grass_base")
            {
                grasTile = i;
                collapsedTile = i;
            }
        }


        //Platziere diese Tile auf die Position der akutellen Zelle und rotiere das Child Mesh nach der Definiton des Enum vom Tile
        GameObject chosenTile = tilesProbabilityList[grasTile].meshObj;
        GameObject parentTile = Instantiate(chosenTile, this.transform.position, Quaternion.identity);
        GameObject child = parentTile.transform.GetChild(0).gameObject;
        child.transform.rotation = Quaternion.Euler(0, (int)tilesProbabilityList[grasTile].rotation, 0);
        //Platziere das neue Tile unter die Level Map
        parentTile.transform.parent = mapParent.transform;
        //Setze die Zelle auf besetzt
        collapsed = true;
       
    }


}
