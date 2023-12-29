using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class <c>GraphMaximize</c> hepl with process of maximization and minimization of graph section
/// </summary>
public class GraphMaximize : MonoBehaviour
{
    [SerializeField] private TwoStateButton chooseButton;
    [SerializeField] private XRSimpleInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {
        interactable.selectEntered.AddListener(x => StartMaximize());
    }

    void StartMaximize()
    {
        chooseButton.SetValue(true);
        MaximizeController.instance.MaximizeSection();
    }
}
