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

    public GameObject[] lineObjs;
    private Material lineMaterial;

    public Color startColor;
    public Color endColor;



    void Start()
    {
        SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        RenderSettings.skybox = skybox;
        timer = 3;
    }

    private void Update()
    {

        turtle.transform.Translate(Vector3.left * speed * Time.deltaTime);

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

    public void ChnageMaterialStep()
    {
        lineObjs = GameObject.FindGameObjectsWithTag("LoadingPath");
        for (int i = 0; i < lineObjs.Length; i++)
        {
            foreach (Transform child in lineObjs[i].transform)
            {
                foreach (Transform child2 in child.transform)
                {
                    if (child2.gameObject.name.Contains("obj"))
                    {
                        //list.Add(child.gameObject);
                        Debug.Log(child.gameObject.name + i);
                        Renderer renderer = child2.GetComponent<Renderer>();
                        renderer.material.SetColor("_Base_Color_1", Color.Lerp(startColor, endColor, i / lineObjs.Length));
                    }
                }
            }
        }
    }


  
}
