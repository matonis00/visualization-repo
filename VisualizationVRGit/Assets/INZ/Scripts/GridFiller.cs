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
    public GameObject sectionToShow;
    public GameObject scetionToHide;
    // Start is called before the first frame update
    private void Start()
    {
        dropdown.onValueChanged.AddListener(Fill);
    }

    private void Fill(int arg0)
    {
        Debug.Log(arg0);
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int i = 0;i<arg0*3;i++) 
        {
            GameObject child = Instantiate(sectionPrefab,transform.position,Quaternion.identity,transform);
            child.transform.Rotate(0, 180, 0);
            Maximize childSetUp = child.GetComponent<Maximize>();
            childSetUp.sectionToShow = sectionToShow;
            childSetUp.scetionToHide = scetionToHide;
            childSetUp.elementId = i;
        }
    }

}
