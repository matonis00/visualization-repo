using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridFiller : MonoBehaviour
{
    public GameObject sectionPrefab;
    public TMP_Dropdown ionDropdown;
    public MaximizeController maximizeController;
    public InOutDataController dataController;

    // Start is called before the first frame update
    private void Start()
    {
        ionDropdown.onValueChanged.AddListener(Fill);
        Fill(ionDropdown.value);
    }

    private void Fill(int arg0)
    {
        int numberOfElements = dataController.ionData[arg0].laserAmount;
        string ionName = ionDropdown.options[arg0].text;
        SaveData saveData = null;
        if (dataController.CheckIfsaved(ionName))
        {
            saveData = dataController.LoadIonData(ionName);
            Debug.Log("jest");
        }
        else Debug.Log("Nie ma");


        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int i = 0;i< numberOfElements; i++) 
        {
            GameObject child = Instantiate(sectionPrefab,transform.position,Quaternion.identity,transform);
            child.GetComponentInChildren<GraphShaderData>().graphs[0].lineColor = new Color((float)i / numberOfElements, 0, (float)(numberOfElements- i) / numberOfElements, 1);
            if(saveData != null)
            {
                child.GetComponentInChildren<GraphShaderData>().graphs[0].points = saveData.graphs[i].points;
            }
            
        }
        maximizeController.AddListenersToGridElements();
    }

}
