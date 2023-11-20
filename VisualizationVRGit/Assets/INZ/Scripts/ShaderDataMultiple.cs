using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using static UnityEngine.Rendering.DebugUI;


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
            desireLength += graph.points.Length;
            desireLength += 3;
        }
        Texture2D input = new Texture2D(desireLength, 1, TextureFormat.RGBA64, false);
        input.filterMode = FilterMode.Point;
        input.wrapMode = TextureWrapMode.Clamp;
        int index = 0;
        for (int i = 0; i < graphs.Length; i++)
        {
            input.SetPixel(index, 0, graphs[i].pointColor);
            index++;
            input.SetPixel(index, 0, graphs[i].lineColor);
            index++;
            input.SetPixel(index, 0, new Color(normalizeValue(graphs[i].points.Length,minPointAmount,maxPointAmount),0f,0f,1f));
            index++;

            Vector2[] orderedPoints = graphs[i].points.OrderBy(v => v.x).ToArray<Vector2>();
            for (int j = 0; j < orderedPoints.Length; j++, index++)
            {
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
    /*
    public int GetClosestPiontIndex(float x, float y)
    {
        Vector2 point = new Vector2(x, y);
        List<Vector2> points = new List<Vector2>();
        foreach (Graph graph in graphs)
        {
            foreach(Vector2 pointinGraph in graph.points)
            {
                points.Add(pointinGraph);
            }
        }


        double distance = Vector2.Distance(point, points[0]);
        int index = 0;
        for (int i = 1; i < points.Count; i++)
        {
            if (Vector2.Distance(point, points[i]) < distance)
            {
                distance = Vector2.Distance(point, points[i]);
                index = i;
            }
        }

        //Debug.Log("Koordynaty zalapania:" + point);
        //Debug.Log("Koordynaty punktu:" + points[index]);
        //Debug.Log("Dystans:" + distance);

        return index;
    }
    */

    public void ChangeScaleY(float scale)
    {
        graphScaleOnY = math.remap(0, 1, graphScaleOnYMin, graphScaleOnYMax, scale);
    }

    public void ChangeScaleX(float scale)
    {
        graphScaleOnX = math.remap(0, 1, graphScaleOnXMin, graphScaleOnXMax, scale);
    }

    public void ChangeOffsetY(float scale)
    {
        graphOffsetOnY = math.remap(0, 1, graphOffsetOnYMin, graphOffsetOnYMax, scale);
    }

    public void ChangeOffsetX(float scale)
    {
        graphOffsetOnX = math.remap(0, 1, graphOffsetOnXMin, graphOffsetOnXMax, scale);
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
