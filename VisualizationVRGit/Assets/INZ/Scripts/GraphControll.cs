using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Mathematics;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.UIElements;


public class GraphControll : MonoBehaviour
{
    public XRKnob knobScaleX;
    public XRKnob knobScaleY;
    public XRKnob knobOffsetX;
    public XRKnob knobOffsetY;
    ShaderDataMultiple shaderData;
    // Start is called before the first frame update
    void Start()
    {
        shaderData = GetComponent<ShaderDataMultiple>();
        knobScaleX.onValueChange.AddListener(ScaleChangeX);
        knobScaleY.onValueChange.AddListener(ScaleChangeY);
        knobOffsetX.onValueChange.AddListener(OffsetChangeX);
        knobOffsetY.onValueChange.AddListener(OffsetChangeY);

        knobScaleX.value = math.remap(shaderData.graphScaleOnXMin, shaderData.graphScaleOnXMax, 0, 1, shaderData.graphScaleOnX);
        knobScaleY.value = math.remap(shaderData.graphScaleOnYMin, shaderData.graphScaleOnYMax, 0, 1, shaderData.graphScaleOnY);
        knobOffsetX.value = math.remap(shaderData.graphOffsetOnXMin, shaderData.graphOffsetOnXMax, 0, 1, shaderData.graphOffsetOnX);
        knobOffsetY.value = math.remap(shaderData.graphOffsetOnYMin, shaderData.graphOffsetOnYMax, 0, 1, shaderData.graphOffsetOnY);


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


    
}
