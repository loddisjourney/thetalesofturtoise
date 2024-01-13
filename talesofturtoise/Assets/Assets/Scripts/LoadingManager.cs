using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Material skybox;

    void Start()
    {
        SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        RenderSettings.skybox = skybox;
    }

    private void Update()
    {
        //if(Input.GetKeyUp(KeyCode.Space))
       //{
        //    SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        //}
    }

}
