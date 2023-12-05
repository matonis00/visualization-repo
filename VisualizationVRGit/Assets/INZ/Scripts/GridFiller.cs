using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static DataController;

public class GridFiller : MonoBehaviour
{
    public GameObject sectionPrefab;
    public TMP_Dropdown dropdown;
    public MaximizeMultiple multipleGraphController;
    public DataController controller;
    //public GameObject sectionToShow;
    //public GameObject scetionToHide;
    // Start is called before the first frame update
    private void Start()
    {
        dropdown.onValueChanged.AddListener(Fill);
        Fill(dropdown.value);
    }

    private void Fill(int arg0)
    {
        int numberOfElements = controller.ionData[arg0].laserAmount;
        string ionName = dropdown.options[arg0].text;
        SaveData saveData = null;
        if (controller.CheckIfsaved(ionName))
        {
            saveData = controller.LoadIonData(ionName);
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
            child.transform.Rotate(0, 180, 0);
            child.GetComponentInChildren<ShaderDataMultiple>().graphs[0].lineColor = new Color((float)i / numberOfElements, 0, (float)(numberOfElements- i) / numberOfElements, 1);
            if(saveData != null)
            {
                child.GetComponentInChildren<ShaderDataMultiple>().graphs[0].points = saveData.graphs[i].points;
            }
            
        }
        multipleGraphController.AddListenersToGridElements();
    }

}
