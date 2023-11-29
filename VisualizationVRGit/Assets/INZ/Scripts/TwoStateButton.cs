using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoStateButton : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material materialOn;
    public Material materialOff;
    public bool value =false;
    public XRSimpleInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {
        interactable.selectEntered.AddListener(x => ChangeValue());

    }
    public void ChangeValue()
    {
        value =!value;
        
    }
    private void Update()
    {
        if (value)
        {
            meshRenderer.sharedMaterial = materialOn;
        }
        else
        {
            meshRenderer.sharedMaterial = materialOff;
        }
    }
}
