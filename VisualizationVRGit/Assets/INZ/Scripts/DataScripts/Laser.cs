using System;
using UnityEngine;

/// <summary>
/// Class <c>Laser</c> is a data class used to describe one laser
/// </summary>
[Serializable]
public struct Laser
{
    /// <summary>
    /// Variable <c>points</c> contains points of laser
    /// </summary>
    public Vector2[] points;
}
