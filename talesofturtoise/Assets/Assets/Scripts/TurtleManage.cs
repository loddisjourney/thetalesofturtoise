using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurtleManage : MonoBehaviour
{
    /*
     *  Prototyp fuer das Management der Schildkroete zum Oeffnen der Ladeszene.
     * 
     */
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("LoadingLevel");
        }
    }

}
