using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TileData : ScriptableObject
{

    /*
     * BaseTile Data as a preset for all Tile Assets to simply create mutliple Tiles fast
     * Scriptable Object Information found on https://gamedevbeginner.com/scriptable-objects-in-unity/ 27.12.2023
     */

    [SerializeField] string tileName; //prototype
    [SerializeField] GameObject meshObj;
    public Rotation rotation; //orientation of the socket
    public int weight; //5,4,3,2,1

    [Header("Sockets")]
    //Neighbor Sockets
    public string pX; //front
    public string nX; //back
    public string pZ; //right
    public string nZ; //left
    public string pY; //top
    public string nY; //bottom

    /*
     * ^Neighbor List
     * -1 invalid = nothing
     * 0s socket type + symmetric
     * 1 or 1f socket type + form in flipped or not flipped ( 1f with valid neighbor of 1 and 1 with valid neighbor of 1f)
     * v0_0 vertical socket with type 0 and rotation 0 to 3
     * f for left side none for right side
    * ----------------Definitions------------------------
    * 0 Grass step zero
    * 1 Dirt Path Grass step zero
    * 2 Dirt Path Grass Edge step zero
    * 3 Grass Edge step zero
    * 4 grass ramp step zero ----- not yet integrated
    * 5 grass stair top edge step zero
    * 6 grass stair middle edge step 0
    * 7 grass step 1
    * 8 Dirt Path Grass step 1
    * 9 Dirt Path Grass Edge step 1
    * 10 Grass Edge step 1 
    * 11 grass ramp step 1
    * 12 grass stair top edge step 1
    * 13 grass step 2
    * 14 Dirt Path Grass step 2
    * 15 Dirt Path Grass Edge step 2
    * 16 Grass Edge step 2 
    * 17 grass ramp step 2
    * 18 grass middle deep edge
    * 19 grass step 3
    * 20 Dirt Path Grass step 3 ----- not yet integrated
    * 21 Dirt Path Grass Edge step 3 ----- not yet integrated
    * 22 Grass Edge step 3 
    * 23 grass ramp step 3 ----- not yet integrated
    * 24 grass top deep edge
    * 25 mountain edge
    * 
    * KONTROLLE
    */


    // THIS WILL MAYBE BE REMOVED
    [Header("Neighbors")]
    //Maybe a List to specifiy valid neighbors 
    public List<string[]> valid_neighbors; //[...] 

    public string[] pX_Neighbor;
    public string[] nX_Neighbor;
    public string[] pZ_Neighbor;
    public string[] nZ_Neighbor;
    public string[] pY_Neighbor;
    public string[] nY_Neighbor;


    private void Awake()
    {
        valid_neighbors = new List<string[]>
        {
            pX_Neighbor,
            nX_Neighbor,
            pZ_Neighbor,
            nZ_Neighbor,
            pY_Neighbor,
            nY_Neighbor
        };
    }



    //Maybe usefull for error handeling to reset a default = air tile
    public void ResetData()
    {
        name = "-1";
        meshObj = null;
        rotation = Rotation.r0;
        weight = 0;
        pX = "-1";
        nX = "-1";
        pZ = "-1";
        nZ = "-1";
        pY = "-1";
        nY = "-1";

    }

    public enum Rotation
    {
        r0,
        r90,
        r180,
        r270
    }

}

