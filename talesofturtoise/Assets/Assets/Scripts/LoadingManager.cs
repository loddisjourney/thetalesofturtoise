using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{


    void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("GeneratedLevel", LoadSceneMode.Additive);
        }

        if(Input.GetKeyUp(KeyCode.U))
        {

        }
    }

}
