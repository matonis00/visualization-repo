using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridFiller : MonoBehaviour
{
    public GameObject sectionPrefab;
    public TMP_Dropdown dropdown;
    public MaximizeMultiple multipleGraphController;
    //public GameObject sectionToShow;
    //public GameObject scetionToHide;
    // Start is called before the first frame update
    private void Start()
    {
        dropdown.onValueChanged.AddListener(Fill);
        

    }

    private void Fill(int arg0)
    {
        int numberOfElements = arg0 * 3 + 1;
        Debug.Log(arg0);
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int i = 0;i< numberOfElements; i++) 
        {
            GameObject child = Instantiate(sectionPrefab,transform.position,Quaternion.identity,transform);
            child.transform.Rotate(0, 180, 0);
            child.GetComponentInChildren<ShaderDataMultiple>().graphs[0].lineColor = new Color((float)i / numberOfElements, 0, (float)(numberOfElements- i) / numberOfElements, 1);
            //Maximize childSetUp = child.GetComponent<Maximize>();
            //childSetUp.sectionToShow = sectionToShow;
            //childSetUp.scetionToHide = scetionToHide;
            //childSetUp.elementId = i;
        }
        multipleGraphController.AddListenersToGridElements();
    }

}
