using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class <c>ButtonFollowVisual</c> controls visual of interactable button
/// </summary>
public class ButtonFollowVisual : MonoBehaviour
{
    [SerializeField] private Transform visualTarget;
    [SerializeField] private Vector3 localAxis;
    [SerializeField] private float resetSpeed =5f;
    [SerializeField] private float followAngleTreshold=45f;



    private bool freeze = false;

    private Vector3 initialLocalPosition;

    private Vector3 offset;
    private Transform pokeAttachTransform;

    private XRBaseInteractable interactable;
    private bool isFollowing = false;


    /// <summary>
    /// Method <c>Start</c> is called before the first frame update, reposonsible for set up of class and controlled visual
    /// </summary>
    void Start()
    {
        initialLocalPosition = visualTarget.localPosition;
        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(ResetButton);
        interactable.selectEntered.AddListener(Freeze);
    }

    /// <summary>
    /// Method <c>Follow</c> is used to start process of folowing interactor by visual
    /// </summary>
    /// <param name="hover">Information on BaseInteraction event</param>
    public void Follow(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;

            float pokeAngle = Vector3.Angle(offset, visualTarget.TransformDirection(localAxis));
            if (pokeAngle < followAngleTreshold)
            {
                isFollowing = true;
                freeze = false;
            }
        }
    }

    /// <summary>
    /// Method <c>ResetButton</c>  is used to end process of folowing interactor by visual and resets visual
    /// </summary>
    /// <param name="hover">Information on BaseInteraction event</param>
    public void ResetButton(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {         
            isFollowing = false;
            freeze = false;
        }
    }

    /// <summary>
    /// Method <c>Freeze</c> is used to freez visual
    /// </summary>
    /// <param name="hover">Information on BaseInteraction event</param>
    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;
        }
    }


    /// <summary>
    /// Method <c>Update</c> called once per frame is reposonsible moving visual in right direction based on set variables
    /// </summary>
    void Update()
    {
        if(freeze) 
        {
            return;
        }
        if(isFollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localAxis);
            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPosition,Time.deltaTime * resetSpeed);
        }
    }
}
