using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Mathematics;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;


public class GraphControll : MonoBehaviour
{
    public XRKnob knobScaleX;
    //public XRKnob knobScaleY;
    //public XRKnob knobOffsetX;
    //public XRKnob knobOffsetY;
    public XRSimpleInteractable buttonBackOffset;
    public XRSimpleInteractable buttonNextOffset;
    MeshCollider meshCollider;
    IXRSelectInteractor xrInteractor;
    public XRSimpleInteractable simpleInteractable;
    ShaderDataMultiple shaderData;

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
        shaderData = GetComponent<ShaderDataMultiple>();

        simpleInteractable.selectEntered.AddListener(Attach);
        simpleInteractable.selectExited.AddListener(DisAttach);

        knobScaleX.onValueChange.AddListener(ScaleChangeX);
       // knobScaleY.onValueChange.AddListener(ScaleChangeY);
        //knobOffsetX.onValueChange.AddListener(OffsetChangeX);
        //knobOffsetY.onValueChange.AddListener(OffsetChangeY);

        //knobScaleX.value = math.remap(shaderData.graphScaleOnXMin, shaderData.graphScaleOnXMax, 0, 1, shaderData.graphScaleOnX);
       // knobScaleY.value = math.remap(shaderData.graphScaleOnYMin, shaderData.graphScaleOnYMax, 0, 1, shaderData.graphScaleOnY);
        //knobOffsetX.value = math.remap(shaderData.graphOffsetOnXMin, shaderData.graphOffsetOnXMax, 0, 1, shaderData.graphOffsetOnX);
       // knobOffsetY.value = math.remap(shaderData.graphOffsetOnYMin, shaderData.graphOffsetOnYMax, 0, 1, shaderData.graphOffsetOnY);

        nowMinX = shaderData.graphOffsetOnX;
        nowMaxX = shaderData.graphOffsetOnX + shaderData.graphScaleOnX;
        nowMinY = shaderData.graphOffsetOnY;
        nowMaxY = shaderData.graphOffsetOnY + shaderData.graphScaleOnY;

        buttonBackOffset.selectEntered.AddListener(OffsetBack);
        buttonNextOffset.selectEntered.AddListener(OffsetNext);



    }

    private void OffsetNext(SelectEnterEventArgs arg0)
    {
        int addition = 16;
        if (shaderData.graphScaleOnX == 12)
        {
            addition = 8;
        }
        else if(shaderData.graphScaleOnX == 8)
        {
            addition = 4;
        }
        else if(shaderData.graphScaleOnX == 6)
        {
            addition = 2;
        }
        float newOffset = shaderData.graphOffsetOnX + addition;
        shaderData.ChangeOffsetX(newOffset);
    }

    private void OffsetBack(SelectEnterEventArgs arg0)
    {
        int addition = 16;
        if (shaderData.graphScaleOnX == 12)
        {
            addition = 8;
        }
        else if (shaderData.graphScaleOnX == 8)
        {
            addition = 4;
        }
        else if (shaderData.graphScaleOnX == 6)
        {
            addition = 2;
        }
        float newOffset = shaderData.graphOffsetOnX - addition;
        if(newOffset >= -2)shaderData.ChangeOffsetX(newOffset);
    }

    private void ScaleChangeX(float arg0)
    {

        int exponent = (int)math.remap(0, 1, 1, 6, knobScaleX.value);
        float newScale = 4 + math.pow(2, exponent);

       // newScale = math.remap(0, 1, shaderData.graphScaleOnXMin, shaderData.graphScaleOnXMax, knobScaleX.value);
       if(shaderData.graphScaleOnX <=12)
        {
            float temp = (shaderData.graphOffsetOnX + 2);
            //Correction of offset
            if (temp % (newScale-4) != 0)
                shaderData.graphOffsetOnX = shaderData.graphOffsetOnX - temp;

        }

        shaderData.ChangeScaleX(newScale);
        //shaderData.unitPerGridOnX = newScale / 15;
    }
    //private void ScaleChangeY(float arg0)
    //{
    //    float newScale =  math.remap(0, 1, shaderData.graphScaleOnYMin, shaderData.graphScaleOnYMax, knobScaleY.value);
    //    shaderData.ChangeScaleY(newScale);
    //}
    //private void OffsetChangeX(float arg0)
    //{
    //    float newOffset = math.remap(0, 1, shaderData.graphOffsetOnXMin, shaderData.graphOffsetOnXMax, knobOffsetX.value);
    //    shaderData.ChangeOffsetX(newOffset);
    //}
    //private void OffsetChangeY(float arg0)
    //{
    //    float newOffset = math.remap(0, 1, shaderData.graphOffsetOnYMin, shaderData.graphOffsetOnYMax, knobOffsetY.value);
    //    shaderData.ChangeOffsetY(newOffset);
    //}

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

        (grapchIndex, pointIndex) = shaderData.GetClosestPiontIndex((float)interactorX, (float)interactorY);

        xrInteractor = arg0.interactorObject;    
        attached = true;
        

    }

    void Update()
    {
        nowMinX = shaderData.graphOffsetOnX;
        nowMaxX = shaderData.graphOffsetOnX + shaderData.graphScaleOnX;
        nowMinY = shaderData.graphOffsetOnY;
        nowMaxY = shaderData.graphOffsetOnY + shaderData.graphScaleOnY;

        if (attached)
        {
            Vector3 inteactionPoint = meshCollider.ClosestPointOnBounds(xrInteractor.transform.position);
            double interactorX = math.remap(-0.5f, 0.5f, nowMinX, nowMaxX, transform.InverseTransformPoint(inteactionPoint).x);
            double interactorY = math.remap(-0.5f, 0.5f, nowMinY, nowMaxY, transform.InverseTransformPoint(inteactionPoint).y);


            shaderData.graphs[grapchIndex].points[pointIndex].x = (float)interactorX;
            shaderData.graphs[grapchIndex].points[pointIndex].y = (float)interactorY;
        }
    }

}
