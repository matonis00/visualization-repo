using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Mathematics;
using UnityEngine.XR.Content.Interaction;

/// <summary>
/// Class <c>GraphControll</c> controls and manages interactions and actions of user with graph
/// </summary>
public class GraphControll : MonoBehaviour
{
    [SerializeField] private XRKnob knobScaleX;
    [SerializeField] private XRSimpleInteractable buttonBackOffset;
    [SerializeField] private XRSimpleInteractable buttonNextOffset;
    [SerializeField] private XRSimpleInteractable buttonModeChange;
    [SerializeField] private GameObject buttonModeChangeGO;
    [SerializeField] private GameObject buttonModeChangeDescGO;
    [SerializeField] private XRSimpleInteractable simpleInteractable;


    MeshCollider meshCollider;
    IXRSelectInteractor xrInteractor;
    GraphShaderData graphShaderData;
    bool createMode = false;
    bool attached = false;
    int grapchIndex;
    int pointIndex;
    float nowMinX;
    float nowMaxX;
    float nowMinY;
    float nowMaxY;

    /// <summary>
    /// Method <c>Start</c> is called before the first frame update, reposonsible for set up of class and controlled interactables 
    /// </summary>
    void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        meshCollider = GetComponent<MeshCollider>();
        graphShaderData = GetComponent<GraphShaderData>();

        simpleInteractable.selectEntered.AddListener(Attach);
        simpleInteractable.selectExited.AddListener(DisAttach);

        knobScaleX.onValueChange.AddListener(ScaleChangeX);
     
        nowMinX = graphShaderData.graphOffsetOnX;
        nowMaxX = graphShaderData.graphOffsetOnX + graphShaderData.graphScaleOnX;
        nowMinY = graphShaderData.graphOffsetOnY;
        nowMaxY = graphShaderData.graphOffsetOnY + graphShaderData.graphScaleOnY;

        buttonBackOffset.selectEntered.AddListener(OffsetBack);
        buttonNextOffset.selectEntered.AddListener(OffsetNext);

        buttonModeChange.selectEntered.AddListener(ChangeMode);
        buttonModeChange.GetComponent<TwoStateButton>().SetValue(createMode);

    }

    /// <summary>
    /// Method <c>ChangeMode</c> changes value of createMode variable that indicates what the current operating mode is
    /// </summary>
    /// <param name="arg0">Inforamtion about SelectEnter event</param>
    private void ChangeMode(SelectEnterEventArgs arg0)
    {
        createMode = !createMode;
    }

    /// <summary>
    /// Method <c>OffsetNext</c> changes offset of graph axis to next interval that depends on current scale of graph
    /// </summary>
    /// <param name="arg0">Inforamtion about SelectEnter event</param>
    private void OffsetNext(SelectEnterEventArgs arg0)
    {
        int addition = 16;
        if (graphShaderData.graphScaleOnX == 12)
        {
            addition = 8;
        }
        else if(graphShaderData.graphScaleOnX == 8)
        {
            addition = 4;
        }
        else if(graphShaderData.graphScaleOnX == 6)
        {
            addition = 2;
        }
        float newOffset = graphShaderData.graphOffsetOnX + addition;
        graphShaderData.SetOffsetOnX(newOffset);
    }
    /// <summary>
    /// Method <c>OffsetBack</c> changes offset of graph axis to previous interval that depends on current scale of graph
    /// </summary>
    /// <param name="arg0">Inforamtion about SelectEnter event</param>
    private void OffsetBack(SelectEnterEventArgs arg0)
    {
        int addition = 16;
        if (graphShaderData.graphScaleOnX == 12)
        {
            addition = 8;
        }
        else if (graphShaderData.graphScaleOnX == 8)
        {
            addition = 4;
        }
        else if (graphShaderData.graphScaleOnX == 6)
        {
            addition = 2;
        }
        float newOffset = graphShaderData.graphOffsetOnX - addition;
        if(newOffset >= -2)graphShaderData.SetOffsetOnX(newOffset);
    }

    /// <summary>
    /// Method <c>ScaleChangeX</c> change scale of graph based on given value
    /// </summary>
    /// <param name="arg0">Value of knob to change skale (normlaized)</param>
    private void ScaleChangeX(float arg0)
    {

        int exponent = (int)math.remap(0, 1, 1, 6, knobScaleX.value);
        float newScale = 4 + math.pow(2, exponent);
        Debug.Log(math.remap(1, 6, 0, 1, 1));
        Debug.Log(math.remap(1, 6, 0, 1, 2));
        Debug.Log(math.remap(1, 6, 0, 1, 3));
        Debug.Log(math.remap(1, 6, 0, 1, 4));
        Debug.Log(math.remap(1, 6, 0, 1, 5));
        Debug.Log(math.remap(1, 6, 0, 1, 6));
        Debug.Log(math.remap(0, 1, -90, -270, math.remap(1, 6, 0, 1, 4)));
       if(graphShaderData.graphScaleOnX <=12)
        {
            float temp = (graphShaderData.graphOffsetOnX + 2);
            //Correction of offset
            if (temp % (newScale-4) != 0)
                graphShaderData.graphOffsetOnX = graphShaderData.graphOffsetOnX - temp;

        }

        graphShaderData.SetScaleOnX(newScale);
    }

    /// <summary>
    /// Method <c>DisAttach</c> reset variables used to attach point to interactor
    /// </summary>
    /// <param name="arg0">Inforamtion about SelectExit event</param>
    private void DisAttach(SelectExitEventArgs arg0)
    {
        attached = false;
        xrInteractor = null;
    }

    /// <summary>
    /// Method <c>Attach</c> attach point to interactor or create new point if <c>createMode</c> is set to <c>true</c>
    /// </summary>
    /// <param name="arg0"></param>
    private void Attach(SelectEnterEventArgs arg0)
    {
        Vector3 inteactionPoint = meshCollider.ClosestPointOnBounds(arg0.interactorObject.transform.position);
        double interactorX = math.remap(-0.5f, 0.5f, nowMinX, nowMaxX, transform.InverseTransformPoint(inteactionPoint).x);
        double interactorY = math.remap(-0.5f, 0.5f, nowMinY, nowMaxY, transform.InverseTransformPoint(inteactionPoint).y);

        (grapchIndex, pointIndex) = graphShaderData.GetClosestPiontIndex((float)interactorX, (float)interactorY);

        xrInteractor = arg0.interactorObject;

        if (createMode)
        {
            graphShaderData.AddPointToGraph(0,new Vector2((float)interactorX, (float)interactorY));
        }
        else
        {
            attached = true;
        }

       
        

    }


    /// <summary>
    /// Method <c>Update</c> called once per frame updates value of processed variables and if variable <c>attached</c> is set true moves the point to the correct coordinates
    /// </summary>
    void Update()
    {
        nowMinX = graphShaderData.graphOffsetOnX;
        nowMaxX = graphShaderData.graphOffsetOnX + graphShaderData.graphScaleOnX;
        nowMinY = graphShaderData.graphOffsetOnY;
        nowMaxY = graphShaderData.graphOffsetOnY + graphShaderData.graphScaleOnY;

        if (attached)
        {
            Vector3 inteactionPoint = meshCollider.ClosestPointOnBounds(xrInteractor.transform.position);
            double interactorX = math.remap(-0.5f, 0.5f, nowMinX, nowMaxX, transform.InverseTransformPoint(inteactionPoint).x);
            double interactorY = math.remap(-0.5f, 0.5f, nowMinY, nowMaxY, transform.InverseTransformPoint(inteactionPoint).y);


            graphShaderData.graphs[grapchIndex].points[pointIndex].x = (float)interactorX;
            graphShaderData.graphs[grapchIndex].points[pointIndex].y = (float)interactorY;
        }

        if(graphShaderData.graphs.Length != 1 ) 
        {
            createMode = false;
            buttonModeChange.GetComponent<TwoStateButton>().SetValue(false);
            buttonModeChangeGO.SetActive(false);
            buttonModeChangeDescGO.SetActive(false);
        }
        else
        {
            buttonModeChangeGO.SetActive(true);
            buttonModeChangeDescGO.SetActive(true);
        }
    }

    /// <summary>
    /// Method <c>GetShaderData</c> returns <c>GraphShaderData</c> of graph
    /// </summary>
    /// <returns>Value of <c>graphShaderData</c> variable</returns>
    public GraphShaderData GetShaderData()
    {
        return graphShaderData;
    }


}
