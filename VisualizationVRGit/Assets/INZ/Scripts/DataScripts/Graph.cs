using System;
using UnityEngine;

/// <summary>
/// Class <c>Graph</c> is a data class used to describe one graph
/// </summary>
[Serializable]
public struct Graph
{
    /// <summary>
    /// Variable <c>pointColor</c> contains color of points of graph
    /// </summary>
    public Color pointColor;
    /// <summary>
    /// Variable <c>lineColor</c> contains color of graph
    /// </summary>
    public Color lineColor;
    /// <summary>
    /// Variable <c>points</c> contains points of graph
    /// </summary>
    public Vector2[] points;
}
