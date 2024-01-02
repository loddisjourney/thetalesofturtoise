using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * Control the States of the Game and manage all functions
     */

    public WaveFunctionCollapse wfc;

    public GameState gameState;
    void Start()
    {
        gameState = GameState.GenerateGrid;
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
                //if player triggers an event it may limit his controls
                break;
            case GameState.LoadingLevel:
                //loading the generation of the level (while the level is generating and path is made like in GenshinImpact)
                break;
            case GameState.GenerateGrid:
                //genrates the basic grid of the level, it may gets different size propertys
                wfc.GenerateGrid();
                break;
            case GameState.PlaceTile:
                //if there is a free neighbor choose the lowest entropy or a random from the lowest entropy
                wfc.FindLowestEntropy();
                break;
            case GameState.CheckNeighbors:
                //check all neighbors and their entropy update ans update the list with all possible tiles and without the setted tiles
                break;
            default:
                //incase their is no specific state set it to free game
                gameState = GameState.FreeGame;
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
