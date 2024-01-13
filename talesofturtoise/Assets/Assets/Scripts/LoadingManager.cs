using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Material skybox;

    public GameObject loadingObj;
    public GameObject endObj;
    //ggf wieder -7
    int positionOfZ = -8;
    GameObject currentPart;
    public GameObject loaderParent;

    public GameObject turtle;
    float speed = 0.3f;
    public int placeTime = 3;
    float timer;
    public static bool levelLoaded = false;

    public static bool endOfLoading = false;

    bool placedEnd = false;

    //Color

    private GameObject[] lineObjs;
    private Material lineMaterial;

    public Color startColor;

    void Start()
    {
        SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        RenderSettings.skybox = skybox;
        timer = 3;
        //  Debug.Log("timer " + timer);
        lineObjs = GameObject.FindGameObjectsWithTag("Respawn");
    }

    private void Update()
    {
        
        for (int i = 0; i < lineObjs.Length; i++)
        {
            lineMaterial= lineObjs[i].GetComponent<MeshRenderer>().material;
            Color currentColor = Color.Lerp(startColor, endColor, t);
        }
        
        
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

        if(!levelLoaded )
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                PlaceLine();
            }

        }
        else
        {
            if(placedEnd== false)
            {
                PlaceLoadingEnd();
            }
            
        }
        
       // if(endOfLoading== true)
       // {
            //Scene scene = SceneManager.GetActiveScene();
            //Debug.Log("Active Scene is '" + scene.name + "'.");
       // }


            //Add end part and ask fpor progress => float progress =  current cound of cells / count of cells .. while progress != 1 do function -> is called from other script or is asked if some cells is collapsing
            // over time until the scene is done

            // next change color



        
    }

    public void PlaceLoadingEnd()
    {
        currentPart = Instantiate(endObj, new Vector3(-27, 0, positionOfZ), Quaternion.Euler(0, -90, 0));
        currentPart.transform.parent = loaderParent.transform;
        placedEnd = true;
    }

    public void PlaceLine()
    {
        currentPart = Instantiate(loadingObj, new Vector3(-27, 0, positionOfZ), Quaternion.Euler(0, -90, 0));
        currentPart.transform.parent = loaderParent.transform;
        positionOfZ++;
        timer = 3;
    }

}
