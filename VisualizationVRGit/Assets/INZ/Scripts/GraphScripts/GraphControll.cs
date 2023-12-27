using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Mathematics;
using UnityEngine.XR.Content.Interaction;


public class GraphControll : MonoBehaviour
{
    public XRKnob knobScaleX;
    public XRSimpleInteractable buttonBackOffset;
    public XRSimpleInteractable buttonNextOffset;
    public XRSimpleInteractable buttonModeChange;
    public GameObject buttonModeChangeGO;
    public GameObject buttonModeChangeDescGO;
    public XRSimpleInteractable simpleInteractable;

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

    // Start is called before the first frame update
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
        buttonModeChange.GetComponent<TwoStateButton>().value = createMode;

    }

    private void ChangeMode(SelectEnterEventArgs arg0)
    {
        createMode = !createMode;
    }

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
        graphShaderData.ChangeOffsetX(newOffset);
    }

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
        if(newOffset >= -2)graphShaderData.ChangeOffsetX(newOffset);
    }

    private void ScaleChangeX(float arg0)
    {

        int exponent = (int)math.remap(0, 1, 1, 6, knobScaleX.value);
        float newScale = 4 + math.pow(2, exponent);

       if(graphShaderData.graphScaleOnX <=12)
        {
            float temp = (graphShaderData.graphOffsetOnX + 2);
            //Correction of offset
            if (temp % (newScale-4) != 0)
                graphShaderData.graphOffsetOnX = graphShaderData.graphOffsetOnX - temp;

        }

        graphShaderData.ChangeScaleX(newScale);
    }
 
    private void DisAttach(SelectExitEventArgs arg0)
    {
        attached = false;
        xrInteractor = null;
    }

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
            buttonModeChange.GetComponent<TwoStateButton>().value = false;
            buttonModeChangeGO.SetActive(false);
            buttonModeChangeDescGO.SetActive(false);
        }
        else
        {
            buttonModeChangeGO.SetActive(true);
            buttonModeChangeDescGO.SetActive(true);
        }
    }

    public GraphShaderData GetShaderData()
    {
        return graphShaderData;
    }


}
