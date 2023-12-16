using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class Maximize : MonoBehaviour
{
    public GameObject sectionToShow;
    public GameObject scetionToHide;
    public XRSimpleInteractable pushButton;
    public TwoStateButton pushButtonMultiple;
    public int elementId = 0;
    public bool get = false;
    public ShaderData shaderData;

    // Start is called before the first frame update
    void Start()
    {
        pushButton.selectEntered.AddListener(x => MaximizeSection());

    }


    // Update is called once per frame
    void MaximizeSection()
    {
        sectionToShow.SetActive(true);
        scetionToHide.SetActive(false);
        if (get)
        {
            List<GameObject> list = new List<GameObject>();
            sectionToShow.GetChildGameObjects(list);
            foreach (GameObject go in list)
            {
                Maximize child = go.GetComponent<Maximize>();
                if (elementId == child.elementId)
                {
                    // child.timeKnob.value = timeKnob.value;
                    child.shaderData.CopyValuesFrom(shaderData);
                }
            }
        }
        else
        { 
            Maximize toShow = sectionToShow.GetComponent<Maximize>();
            toShow.elementId = elementId;
            //toShow.timeKnob.value = timeKnob.value;
            toShow.shaderData.CopyValuesFrom(shaderData);
        }
    }
}
