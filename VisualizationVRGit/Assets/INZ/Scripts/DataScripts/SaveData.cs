using System;

/// <summary>
/// Class <c>SaveData</c> is a data class used in process of serialization of save data
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>
    /// Variable <c>IonName</c> contains name of saved ion
    /// </summary>
    public string IonName;
    /// <summary>
    /// Variable <c>piTime</c> contains number of lasers in saved ion
    /// </summary>
    public int numberOfLasers;
    /// <summary>
    /// Variable <c>piTime</c> contains value of pitime parameter
    /// </summary>
    public float piTime;
    /// <summary>
    /// Variable <c>graphs</c> contains array of graphs in given saved ion
    /// </summary>
    public Graph[] graphs;
}
