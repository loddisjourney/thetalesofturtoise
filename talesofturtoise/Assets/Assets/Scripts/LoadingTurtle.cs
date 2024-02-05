using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadingTurtle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //hit.collison.tag == "End"

        if(LoadingManager.levelLoaded)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.transform.CompareTag("EndPart"))
                {
                    
                    //LoadingManager.endOfLoading = true;
                    WaveFunctionCollapse.levelGenerated = true;
                }

                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 20, Color.red);
            }
        }

       
           
    }
}
