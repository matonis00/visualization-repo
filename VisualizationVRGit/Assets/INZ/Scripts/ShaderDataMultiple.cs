using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UIElements;


public class ShaderDataMultiple : MonoBehaviour
{


    private MaterialPropertyBlock materialPB;

    public MeshRenderer meshRenderer;



    public float graphOffsetOnX = -2f;
    public float graphOffsetOnY = -2f;

    public float graphOffsetOnXMin = -6f;
    public float graphOffsetOnYMin = -6f;

    public float graphOffsetOnXMax = 6f;
    public float graphOffsetOnYMax = 6f;


    public float graphScaleOnX = 16f;
    public float graphScaleOnY = 16f;

    public float graphScaleOnXMin = 4f;
    public float graphScaleOnYMin = 4f;

    public float graphScaleOnXMax = 16f;
    public float graphScaleOnYMax = 16f;



    public float unitPerGridOnX = 1f;
    public float unitPerGridOnY = 1f;

    public enum pointShapeEnum
    {
        Square = 1,
        Circle = 2
    }

    public pointShapeEnum pointShape = pointShapeEnum.Square;
    public Color pointColor = Color.black;
    public float pointSize = 0.2f;
    public float minXPointValue = -1f;
    public float maxXPointValue = 9f;
    public float minYPointValue = -1f;
    public float maxYPointValue = 9f;

    public int minPointAmount = 0;
    public int maxPointAmount = 255;


    public enum lineVariantEnum
    {
        Normal = 1,
        AllwaysConnected = 2,
        Catmull = 3
    }


    public lineVariantEnum lineVariant = lineVariantEnum.Normal;
    public Color lineColor = Color.black;
    public float lineSize = 0.2f;


    [Serializable]
    public struct Graph {
        public Color pointColor;
        public Color lineColor;
        public Vector2[] points; 
    }
    public Graph[] graphs;
    
    private void Start()
    {


        materialPB = new MaterialPropertyBlock();
    }
    void Update()
    {
        int desireLength = 0;
        foreach (Graph graph in graphs)
        {

            desireLength += graph.points.Distinct().ToArray().Length;
            desireLength += 3;
        }
        Texture2D input = new Texture2D(desireLength, 1, TextureFormat.RGBAFloat, false);
        input.filterMode = FilterMode.Point;
        input.wrapMode = TextureWrapMode.Clamp;
        int index = 0;
        for (int i = 0; i < graphs.Length; i++)
        {
            Vector2[] orderedPoints = graphs[i].points.Distinct().ToArray().OrderBy(v => v.x).ToArray<Vector2>();
            input.SetPixel(index, 0, graphs[i].pointColor);
            index++;
            input.SetPixel(index, 0, graphs[i].lineColor);
            index++;
            input.SetPixel(index, 0, new Color(normalizeValue(orderedPoints.Length,minPointAmount,maxPointAmount),0f,0f,1f));
            index++;

           
             
            for (int j = 0; j < orderedPoints.Length; j++, index++)
            {
                //Debug.Log(normalizeValue(orderedPoints[j].x, minXPointValue, maxXPointValue));
                input.SetPixel(index, 0, new Color(normalizeValue(orderedPoints[j].x,minXPointValue,maxXPointValue), normalizeValue(orderedPoints[j].y,minYPointValue,maxYPointValue), 1.0f,1.0f));
            }
        }
        input.Apply();
        meshRenderer.GetPropertyBlock(materialPB);
        materialPB.SetFloat("_ScaleX", graphScaleOnX);
        materialPB.SetFloat("_ScaleY", graphScaleOnY);
        materialPB.SetFloat("_OffsetX", graphOffsetOnX);
        materialPB.SetFloat("_OffsetY", graphOffsetOnY);
        materialPB.SetFloat("_UnitPerGridX", unitPerGridOnX);
        materialPB.SetFloat("_UnitPerGridY", unitPerGridOnY);
        materialPB.SetInteger("_LineVariant", (int)lineVariant);
        materialPB.SetColor("_LineColor", lineColor);
        materialPB.SetFloat("_LineSize", lineSize);
        materialPB.SetInteger("_PointShape", (int)pointShape);
        materialPB.SetColor("_PointColor", pointColor);
        materialPB.SetFloat("_PointSize", pointSize);
        materialPB.SetTexture("_GraphsTex", input);
        materialPB.SetInt("_GraphsAmount", graphs.Length);
        materialPB.SetInt("_TexAmount", desireLength);
        materialPB.SetInt("_MinPointsAmount", minPointAmount);
        materialPB.SetInt("_MaxPointsAmount", maxPointAmount);
        materialPB.SetFloat("_MinXValue", minXPointValue);
        materialPB.SetFloat("_MaxXValue", maxXPointValue);
        materialPB.SetFloat("_MinYValue", minYPointValue);
        materialPB.SetFloat("_MaxYValue", maxYPointValue);
        meshRenderer.SetPropertyBlock(materialPB);
    }


    //TODO REPAIR FOR MULTI
    
    public (int,int) GetClosestPiontIndex(float x, float y)
    {
        Vector2 point = new Vector2(x, y);
        
        double distance = double.MaxValue;
        int graphIndex = 0;
        int pointIndex = 0;
        for (int i = 0; i < graphs.Length; i++)
        {
            for (int j = 0;j < graphs[i].points.Length; j++)
            {
                if (Vector2.Distance(point, graphs[i].points[j]) < distance)
                {
                    distance = Vector2.Distance(point, graphs[i].points[j]);
                    graphIndex = i;
                    pointIndex = j;
                }
            }

        }
        Vector2 closestPoint = new Vector2(graphIndex, pointIndex);

        return (graphIndex, pointIndex);
    }
    

    public void ChangeScaleY(float scale)
    {
        graphScaleOnY = scale;
    }

    public void ChangeScaleX(float scale)
    {
        graphScaleOnX = scale;
    }

    public void ChangeOffsetY(float offset)
    {
        graphOffsetOnY = offset;
    }

    public void ChangeOffsetX(float offset)
    {
        graphOffsetOnX = offset;
    }

    public void CopyValuesFrom(ShaderDataMultiple input)
    {
        graphOffsetOnX = input.graphOffsetOnX;
        graphOffsetOnXMin = input.graphOffsetOnXMin;
        graphOffsetOnXMax = input.graphOffsetOnXMax;
        graphOffsetOnY = input.graphOffsetOnY;
        graphOffsetOnYMin = input.graphOffsetOnYMin;
        graphOffsetOnYMax = input.graphOffsetOnYMax;

        graphScaleOnX = input.graphScaleOnX;
        graphScaleOnXMin = input.graphScaleOnXMin;
        graphScaleOnXMax = input.graphScaleOnXMax;
        graphScaleOnY = input.graphScaleOnY;
        graphScaleOnYMin = input.graphScaleOnYMin;
        graphScaleOnYMax = input.graphScaleOnYMax;

        unitPerGridOnX = input.unitPerGridOnX;
        unitPerGridOnY = input.unitPerGridOnY;

        pointShape = input.pointShape;
        pointColor = input.pointColor;
        pointSize = input.pointSize;
        minXPointValue = input.minXPointValue;
        maxXPointValue = input.maxXPointValue;
        minYPointValue = input.minYPointValue;
        maxYPointValue = input.maxYPointValue;

        lineColor = input.lineColor;
        lineSize = input.lineSize;
        lineVariant = input.lineVariant;

    }

    private float normalizeValue(float value, float minValue, float maxValue)
    {
        return (value - minValue) / (maxValue - minValue);
    }

}
