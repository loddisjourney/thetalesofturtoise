using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Material skybox;

    public GameObject loadingObj;
    int loadingInt = 12;
    //ggf wieder -7
    int positionOfZ = -8;
    GameObject currentPart;
    public GameObject loaderParent;
    int i = 0;

    public GameObject turtle;
    float speed = 0.3f;
    public int placeTime = 3;
    bool spawning;
    float timer;

    bool levelLoaded = false;

    void Start()
    {
        SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        RenderSettings.skybox = skybox;
        timer = 3;
        Debug.Log("timer " + timer);
    }

    private void Update()
    {
        //if(Input.GetKeyUp(KeyCode.Space))
        //{
        //    SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        //}

        turtle.transform.Translate(Vector3.left * speed * Time.deltaTime);

        // while ( i < loadingInt)
        // {
        //     currentPart = Instantiate(loadingObj, new Vector3(-27, 0, positionOfZ), Quaternion.Euler(0,-90,0));
        //    currentPart.transform.parent = loaderParent.transform;
        //     positionOfZ++;
        //     i++;
        // }
       // Debug.Log("timer " + timer);
        timer -= Time.deltaTime;

           
            if (timer <= 0.0f)
            {
                PlaceLine();
            }


            //Add end part and ask fpor progress => float progress =  current cound of cells / count of cells .. while progress != 1 do function -> is called from other script or is asked if some cells is collapsing
            // over time until the scene is done

            // next change color

            //implement cell list with weight count and probaility

        
    }

    public void PlaceLine()
    {
        currentPart = Instantiate(loadingObj, new Vector3(-27, 0, positionOfZ), Quaternion.Euler(0, -90, 0));
        currentPart.transform.parent = loaderParent.transform;
        positionOfZ++;
        timer = 3;
    }

}
