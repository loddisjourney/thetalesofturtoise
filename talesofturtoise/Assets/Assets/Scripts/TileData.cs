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
    public int weight; //

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
     * 0s type + symmetric
     * 1 or 1f type + form in flipped or not flipped ( 1f with valid neighbor of 1 and 1 with valid neighbor of 1f)
     * v0_0 vertical socket with type 0 and rotation 0 to 3
    * 
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

