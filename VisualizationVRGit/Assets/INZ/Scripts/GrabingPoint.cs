using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Mathematics;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.UIElements;

public class GrabingPoint : MonoBehaviour
{
    public XRSimpleInteractable pushButton;
    public XRKnob knobScaleX;
    public XRKnob knobScaleY;
    public XRKnob knobOffsetX;
    public XRKnob knobOffsetY;
    public MeshCollider meshCollider;
    IXRSelectInteractor xrInteractor;
    XRSimpleInteractable simpleInteractable;
    ShaderData shaderData;
    bool create = false;
    bool attached = false;
    int index = 0;
    float nowMinX;
    float nowMaxX;
    float nowMinY;
    float nowMaxY;


    // Start is called before the first frame update
    void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        shaderData = GetComponent<ShaderData>();
        pushButton.selectEntered.AddListener(ChangeMode);
        simpleInteractable.selectEntered.AddListener(Attach);
        simpleInteractable.selectExited.AddListener(DisAttach);

        knobScaleX.onValueChange.AddListener(ScaleChangeX);
        knobScaleY.onValueChange.AddListener(ScaleChangeY);
        knobOffsetX.onValueChange.AddListener(OffsetChangeX);
        knobOffsetY.onValueChange.AddListener(OffsetChangeY);

        knobScaleX.value = math.remap( shaderData.graphScaleOnXMin, shaderData.graphScaleOnXMax, 0, 1, shaderData.graphScaleOnX);
        knobScaleY.value = math.remap( shaderData.graphScaleOnYMin, shaderData.graphScaleOnYMax, 0, 1, shaderData.graphScaleOnY);
        knobOffsetX.value = math.remap(shaderData.graphOffsetOnXMin, shaderData.graphOffsetOnXMax, 0, 1, shaderData.graphOffsetOnX);
        knobOffsetY.value = math.remap(shaderData.graphOffsetOnYMin, shaderData.graphOffsetOnYMax, 0, 1, shaderData.graphOffsetOnY);

        
        nowMinX = shaderData.graphOffsetOnX;
        nowMaxX = shaderData.graphOffsetOnX + shaderData.graphScaleOnX;
        nowMinY = shaderData.graphOffsetOnY;
        nowMaxY = shaderData.graphOffsetOnY + shaderData.graphScaleOnY;

    }

    private void ScaleChangeX(float arg0)
    {
        shaderData.ChangeScaleX(knobScaleX.value);
    }
    private void ScaleChangeY(float arg0)
    {
        shaderData.ChangeScaleY(knobScaleY.value);
    }
    private void OffsetChangeX(float arg0)
    {
        shaderData.ChangeOffsetX(knobOffsetX.value);
    }
    private void OffsetChangeY(float arg0)
    {
        shaderData.ChangeOffsetY(knobOffsetY.value);
    }

    private void ChangeMode(SelectEnterEventArgs arg0)
    {
        create =!create;
    }

    private void DisAttach(SelectExitEventArgs arg0)
    {
        attached = false;
        xrInteractor = null;
    }

    private void Attach(SelectEnterEventArgs arg0)
    {
        Vector3 inteactionPoint = meshCollider.ClosestPointOnBounds(arg0.interactorObject.transform.position);
        double interactorX = math.remap(-0.5f,0.5f,  nowMinX, nowMaxX, transform.InverseTransformPoint(inteactionPoint).x);
        double interactorY = math.remap(-0.5f,0.5f, nowMinY, nowMaxY, transform.InverseTransformPoint(inteactionPoint).y);
        

        index = shaderData.GetClosestPiontIndex((float)interactorX, (float)interactorY);
        xrInteractor = arg0.interactorObject;
        if(create)
        {
            shaderData.AddPointToGraph(new Vector2((float)interactorX, (float)interactorY));
        }
        else
        {
            attached = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        nowMinX = shaderData.graphOffsetOnX;
        nowMaxX = shaderData.graphOffsetOnX + shaderData.graphScaleOnX;
        nowMinY = shaderData.graphOffsetOnY;
        nowMaxY = shaderData.graphOffsetOnY + shaderData.graphScaleOnY;

        if (attached)
        {
            Vector3 inteactionPoint = meshCollider.ClosestPointOnBounds(xrInteractor.transform.position);
            double interactorX = math.remap(-0.5f,0.5f,  nowMinX, nowMaxX, transform.InverseTransformPoint(inteactionPoint).x);
            double interactorY = math.remap(-0.5f, 0.5f, nowMinY, nowMaxY,  transform.InverseTransformPoint(inteactionPoint).y);


            shaderData.points[index].x = (float)interactorX;
            shaderData.points[index].y = (float)interactorY;
        }
    }





}
