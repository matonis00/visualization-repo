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
    public int elementId = 0;
    public bool get = false;
    public ShaderData shaderData;
   // public XRKnob timeKnob;
   // public TextAsset jsonFile;

    // Start is called before the first frame update
    void Start()
    {
        pushButton.selectEntered.AddListener(x => MaximizeSection());
       /* Configuration configuration = Configuration.CreateFromJSON(jsonFile.text);
        Debug.Log(configuration.mass);
        Debug.Log(configuration.charge);
        Debug.Log(configuration.nuclear_spin);
        Debug.Log(configuration.default_sublevel_selection[0]);*/
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
