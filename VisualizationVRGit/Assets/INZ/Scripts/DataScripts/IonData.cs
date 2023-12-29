using System;

/// <summary>
/// Class <c>IonData</c> is a data class used in process of deserialization of ions data
/// </summary>
[Serializable]
public class IonData
{
    /// <summary>
    /// Variable <c>name</c> contains name of ion
    /// </summary>
    public string name;
    /// <summary>
    /// Variable <c>laserAmount</c> contains number of lasers in configuration for this ion
    /// </summary>
    public int laserAmount;
}
