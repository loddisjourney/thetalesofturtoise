using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TileData : ScriptableObject
{

    /*
     * Basis fuer die Daten der Tiles als Schablone fuer alle Tiles
     * Scriptable Object Infos und grundlegende Verwendung, aufgerufen am 27.12.2023 unter: https://gamedevbeginner.com/scriptable-objects-in-unity/ 
     */

    [SerializeField] string tileName; //prototyp_...
    public GameObject meshObj;
    public Rotation rotation;
    public TileRotation tileRotation;
    public int weight;

    [Header("Sockets")]
    //Neighbor Sockets analog zu den festgelegten Regeln
    public string pX; //front
    public string nX; //back
    public string pZ; //right
    public string nZ; //left
    public string pY; //top
    public string nY; //bottom

    /*
     * Regelliste und Socket Definition
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
    * 25 mountain base
    * 26 mountain edge
    * v19_0 mountain vertical
    * 
    */

    public enum Rotation
    {
        r0,
        r90,
        r180,
        r270
    }

    //korrigiertes Enum, um Integer zu erhalten
    public enum TileRotation
    {
        r0 = 0,
        r90 = 90,
        r180 = 180,
        r270 =270
    }

}

