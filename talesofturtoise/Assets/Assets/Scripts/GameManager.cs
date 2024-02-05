using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * GameManager soll im spaeteren Verlauf der Entwicklung verwendet werden und wird momentan noch nicht eingesetzt.
     * Er soll spaeter dazu dienen die Controls des Player zu steurn, je nach Spiel Status
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
                //if player triggers an event it may limit his controls
                break;
            case GameState.LoadingLevel:
                //loading the generation of the level (while the level is generating and path is made like in GenshinImpact)
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
        LoadingLevel
    }
}
