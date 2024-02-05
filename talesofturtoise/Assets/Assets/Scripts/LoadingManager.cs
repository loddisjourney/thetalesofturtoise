using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    /*
     * Loading Manager
     * Die Idee ist, dass eine andere art von LAdebalken visualisiert werden soll. Waehrend die Wave Function Collpase generiert, 
     * soll die Schildkroete den Weg dzum neuen Biom ablaufen. Die Umgebnung soll sich dabei stetig zum neuen Biom umwandeln in farbe 
     * und ggf. Assetplatzierung je nach prozentualen Fortschritt der Wave Function Collapse ueberblenden.
     * 
     * Im Hintergrund, da die Kamera das neue Level noch nicht "sieht", wird das neue Level addaptiv geladen.
     * 
     * Wahrend das neue LEvel laed lauefrt die Schildkroete entlng eines Pfads, der alle 3 Sekunden um ein loadingObj verlaengert wird und je nach prozentualen Stand eine neue Farbstufe blendet.
     * 
     * Problem: Wave Function Collapse laedt schneller als bedacht, daher wird der Timer nur einmal ausgefuehrt. Es muessten noch weitere Tests gemacht werden, wenn die Wave Function Collapse vollstaendig implementiert ist. 
     * Ansonsten koennte es einen Standardpfad geben der immer gleich lang ist. Dies wuerde die Komplexitaet der Farbanederung vermutlich reduzieren.
     * *
     */

    
    public Material skybox;

    public GameObject loadingObj;
    public GameObject endObj;

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
        //Schildkroete laeuft entlang des Pfads
        turtle.transform.Translate(Vector3.left * speed * Time.deltaTime);

        //Solange das Level nicht fertig geladen ist, soll immer nach 3 Sekunden bzw. einem timer der Weg verlaengert werden
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
            //Ist das Level geladen und das Endteil noch nicht platziert, wird es platziert.
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

    /*
     * Erster Ansatz fuer die Farblende, diese ist noch nicht korrekt und aucherstmal nur fuer die erste Anzahl von lineObjekten ausgelegt. Es wird bishe rnur getestet, ob die Lerp Funktion eine moegliche Loesung waere. Zeitlich konnte es nicht zu Ende getestet werden
     * Spaeter soll sie nur die Farbe von dem neuen Line Objekt aendern und die Groe�e des Lerp dynamisch aendern.
     * **/
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
