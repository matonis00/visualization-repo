using System;
/// <summary>
/// Class <c>ConfigData</c> is a data class used in process of deserialization of config data
/// </summary>
[Serializable]
public class ConfigData 
{
    /// <summary>
    /// Variable <c>pathToSave</c> contains path to save files folder
    /// </summary>
    public string pathToSave;
    /// <summary>
    /// Variable <c>pathToExport</c> contains path to export files folder
    /// </summary>
    public string pathToExport;
    /// <summary>
    /// Variable <c>pathToIonsFile</c> contains path to file with ion configurations
    /// </summary>
    public string pathToIonsFile;
}

