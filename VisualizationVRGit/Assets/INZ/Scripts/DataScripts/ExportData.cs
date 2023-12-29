using System;
using System.Collections.Generic;

/// <summary>
/// Class <c>ExportData</c> is a data class used in process of serialization of export data
/// </summary>
[Serializable]
public class ExportData
{
    /// <summary>
    /// Variable <c>IonName</c> contains name of exported ion
    /// </summary>
    public string IonName;
    /// <summary>
    /// Variable <c>piTime</c> contains value of pitime parameter
    /// </summary>
    public float piTime;
    /// <summary>
    /// Variable <c>lasers</c> contains list of configured lasers for exported ion
    /// </summary>
    public List<Laser> lasers;
}