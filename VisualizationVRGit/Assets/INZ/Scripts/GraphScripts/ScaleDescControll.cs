using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class ScaleDescControll : MonoBehaviour
{
    public ShaderDataMultiple shaderData;
    public RectTransform scaleDesc1Transform;
    public RectTransform scaleDesc3Transform;
    public RectTransform scaleDescY0Transform;
    public RectTransform scaleDescY1Transform;

    public TextMeshProUGUI scaleDesc1;
    public TextMeshProUGUI scaleDesc2;
    public TextMeshProUGUI scaleDesc3;
    public TextMeshProUGUI scaleDescAxiX;

    float scaleGraphElement = 19.5f;
    //ON SCALE OR OFFSETCHANGE
    void Update()
    {
        float sizeOfUnit = 19.5f / shaderData.graphScaleOnX;
        float scale = shaderData.graphScaleOnX - 4;
        
        float offset = shaderData.graphOffsetOnX + 2f; //π
        float startingText = offset / 8f;
        float startingTextPosition = -(scaleGraphElement/2) + sizeOfUnit * 2;

        float middleText = (offset + (scale / 2f)) / 8f;

        float lastText = (offset + scale) / 8f;
        float lastTextPosition = (scaleGraphElement / 2) - sizeOfUnit * 2;
        
        scaleDesc1.text = startingText + "π";
        scaleDesc1Transform.localPosition = new Vector3(startingTextPosition, scaleDesc1Transform.localPosition.y, scaleDesc1Transform.localPosition.z);
        scaleDesc2.text = middleText + "π";
        scaleDesc3.text = lastText + "π";
        scaleDesc3Transform.localPosition = new Vector3(lastTextPosition, scaleDesc3Transform.localPosition.y, scaleDesc3Transform.localPosition.z);

        float yTextPosition ;
        if (offset == 0  ) {
            yTextPosition = -(scaleGraphElement / 2) + sizeOfUnit * 1.5f;
        }
        else if( offset ==1)
        {
            yTextPosition = -(scaleGraphElement / 2) + sizeOfUnit * 0.5f;
        }
        else
        {
            yTextPosition = -(scaleGraphElement / 2) + sizeOfUnit * 0.25f; ;
        }

        scaleDescY0Transform.localPosition = new Vector3(yTextPosition, scaleDescY0Transform.localPosition.y, scaleDescY0Transform.localPosition.z);
        scaleDescY1Transform.localPosition = new Vector3(yTextPosition, scaleDescY1Transform.localPosition.y, scaleDescY1Transform.localPosition.z);

    }

}
