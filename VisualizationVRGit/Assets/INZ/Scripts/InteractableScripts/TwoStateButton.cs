using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class <c>TwoStateButton</c> is responsible to controll two state button interaction
/// </summary>
public class TwoStateButton : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material materialOn;
    public Material materialOff;
    public bool value =false;
    public XRSimpleInteractable interactable;

    /// <summary>
    /// Method <c>Start</c> is called before the first frame update, reposonsible for set up of class
    /// </summary>
    void Start()
    {
        interactable.selectEntered.AddListener(x => ChangeValue());

    }
    /// <summary>
    /// Method <c>ChangeValue</c> changes value of button to opposite
    /// </summary>
    public void ChangeValue()
    {
        value =!value;
        
    }

    /// <summary>
    /// Method <c>Update</c> called once per frame is reposonsible change color of visual part of button based on its value
    /// </summary>
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
