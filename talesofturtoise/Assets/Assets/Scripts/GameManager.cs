using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * Control the States of the Game and manage all functions
     */

    public WaveFunctionCollapse wfc;
    private bool gridStarted;
    private bool entropyStarted;
    private bool checkNeighborStarted;

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
                StartGeneration();
                break;
            case GameState.PlaceTile:
                //if there is a free neighbor choose the lowest entropy or a random from the lowest entropy
                entropyStarted = false;
                StartFindLowestEntropy();
                break;
            case GameState.CheckNeighbors:
                //check all neighbors and their entropy update ans update the list with all possible tiles and without the setted tiles
                checkNeighborStarted = false;
                StartCheckNeighbors();
                break;
            default:
                //incase their is no specific state set it to free game
                gameState = GameState.PlaceTile;
                break;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            entropyStarted = false;
            StartFindLowestEntropy();
        }
    }

    public void StartGeneration()
    {
        if(!gridStarted)
        {
            gridStarted = true;
            wfc.GenerateGrid();
        }
        
    }

    public void StartFindLowestEntropy()
    {
        if (!entropyStarted)
        {
            entropyStarted = true;
            wfc.FindLowestEntropy();
        }
    }

    public void StartCheckNeighbors()
    {
        Debug.Log("GameManager");
        if (!checkNeighborStarted)
        {
            checkNeighborStarted = true;
            Debug.Log("starting");
            wfc.CheckNeighbors();
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
