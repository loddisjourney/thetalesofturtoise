using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadingTurtle : MonoBehaviour
{
    /*
     * Kollisionsmanager fuer die Schildkroete in der Ladeszene mit einem Raycast
     * Setzt das neue Level erst auf generiert, wenn das Enstueck des Ladewegs beruehrt wird, da die Wave Function Collapse schneller laedt als zurvor angenommen
     * 
     */
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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
