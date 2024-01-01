using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{

    //[SerializeField] GameObject gridElement;
    public Cell cell;
    int i;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        i=cell.gameObject.GetComponent<Cell>().GetTileData().Length;
        Debug.Log(i);
    }

    public void GenerateGrid()
    {
        //and generate floor -1 with water
    }
}
