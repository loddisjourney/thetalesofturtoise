using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * Control the States of the Game and manage all functions
     */

    public GameState gameState;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.FreeGame:
                //State while player plays by default and moves freely through the world
                break;
            case GameState.CutScene:
                //if player triggers an event it may 
                break;
            case GameState.LoadingLevel:
                break;
            case GameState.GenerateGrid:
                break;
            case GameState.PlaceTile:
                break;
            case GameState.CheckNeighbors:
                break;
            default:
                break;
        }
    }

    public enum GameState
    {
        FreeGame,
        CutScene,
        LoadingLevel,
        GenerateGrid,
        PlaceTile,
        CheckNeighbors
    }
}
