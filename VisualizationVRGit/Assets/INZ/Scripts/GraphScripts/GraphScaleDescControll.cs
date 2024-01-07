using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>GraphScaleDescControll</c> controls graph scale descriptions
/// </summary>
public class GraphScaleDescControll : MonoBehaviour
{
    [SerializeField] private GraphShaderData shaderData;
    [SerializeField] private RectTransform scaleDesc1Transform;
    [SerializeField] private RectTransform scaleDesc3Transform;
    [SerializeField] private RectTransform scaleDescY0Transform;
    [SerializeField] private RectTransform scaleDescY1Transform;

    [SerializeField] private TextMeshProUGUI scaleDesc1;
    [SerializeField] private TextMeshProUGUI scaleDesc2;
    [SerializeField] private TextMeshProUGUI scaleDesc3;
    [SerializeField] private TextMeshProUGUI scaleDescAxiX;

    /// <summary>
    /// Method <c>Update</c> is called once per frame, refresh value and positio of scale desctiptions for graph
    /// </summary>
    void Update()
    {
        float scaleGraphElement = 19.5f;
        float sizeOfUnit = scaleGraphElement / shaderData.graphScaleOnX;
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
