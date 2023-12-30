using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class <c>InOutDataController</c> responsible for importing, saving and exporting data
/// </summary>
public class InOutDataController : MonoBehaviour
{
    /// <summary>
    /// Instance of <c>InOutDataController</c> class
    /// </summary>
    public static InOutDataController instance { get; private set; }

    [SerializeField] private GameObject gridSection;
    [SerializeField] private XRSimpleInteractable buttonSave;
    [SerializeField] private XRSimpleInteractable buttonExport;
    [SerializeField] private TMP_Dropdown dropdownIon;
    [SerializeField] private TMP_InputField inputPitime;
    [SerializeField] private TMP_Dropdown dropdownPitimeUnit;
    [SerializeField] private TMP_InputField inputIonName;
    [SerializeField] private TwoStateButton buttonNewIon;
    [SerializeField] private GameObject newMenu;
    [SerializeField] private string pathToSave;
    [SerializeField] private string pathToExport;
    [SerializeField] private string pathToIonsFile;
    public List<IonData> ionData;

    
    /// <summary>
    /// Method <c>Awake</c> reposonsible for set instance of class and run import of data process
    /// </summary>
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        dropdownIon.value = 0;
        buttonSave.selectEntered.AddListener(x => SaveIon());
        buttonExport.selectEntered.AddListener(x => ExportIon());
        InitializeConfigData();
        InitializeDropdownIon();
    }
    /// <summary>
    /// Method <c>CheckIfSaved</c> provide option to check if there is a save file for given ion name
    /// </summary>
    /// <param name="nameOfIon">Name of ion to check save for</param>
    /// <returns>
    /// A bool with value of "true" if there is a save or "false" if there is not
    /// </returns>
    public bool CheckIfSaved(string nameOfIon)
    {
        string pathToIonDirectory = pathToSave + "/" + nameOfIon;
        if(Directory.Exists(pathToIonDirectory))
        {
            if(File.Exists(pathToIonDirectory +"/Ion.json"))
            {
                return true;
            }
        }      
        return false;
    }

    /// <summary>
    /// Method <c>LoadIonData</c> provide option to load data from save file of given ion
    /// </summary>
    /// <param name="nameOfIon">Name of ion to load save data</param>
    /// <returns>
    /// <c>SaveData</c> object that contain save data for given ion name or returns <c>null</c> if there is no save file for this ion name
    /// </returns>
    public SaveData LoadIonData(string nameOfIon)
    {
        string pathToIonDirectory = pathToSave + "/" + nameOfIon;
        if (Directory.Exists(pathToIonDirectory))
        {
            string filePath = pathToIonDirectory + "/Ion.json";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<SaveData>(json);
            }
        }
        return null;
    }

    /// <summary>
    /// Method <c>ExportIon</c> exports the data of the currently processed ion
    /// </summary>
    private void ExportIon()
    {
        string nameOfIon = (buttonNewIon.GetValue()) ? inputIonName.text : dropdownIon.options[dropdownIon.value].text;

        string pathToIonDirectory = pathToExport + "/" + nameOfIon;
        string pathToIonFile = pathToIonDirectory + "/" + "Ion.json";
        float piTimeValue = float.Parse(inputPitime.text) * (float)Math.Pow(10, -3 * (dropdownPitimeUnit.value + 1));
        List<GraphShaderData> data = gridSection.GetComponentsInChildren<GraphShaderData>().ToList();
        
        ExportData exportData = new ExportData();
        exportData.IonName = nameOfIon;
        exportData.piTime = piTimeValue;
        exportData.lasers = new List<Laser>();

        int juliaIndex = 1;
        string julliaScript = "include(\""+ Directory.GetCurrentDirectory().Replace('\\','/') + "/scripts/laserValueBaseScript.jl\")\r\n" + "ionFile = \"" + pathToIonFile.Replace('\\', '/')+ "\"\r\n";
        string pathToJuliaScript = pathToIonDirectory + "/" + nameOfIon + "Lasers.jl";
        foreach (var graph in data)
        {
            List<Vector2> tmpPoints = new List<Vector2>{ new Vector2(0, 0) };
            tmpPoints = tmpPoints.Concat(graph.graphs[0].points.ToList().Distinct().OrderBy(v => v.x)).ToList();
            tmpPoints.Add(new Vector2(tmpPoints.Last().x+1.0f,tmpPoints.Last().y));

            Vector2[] points = tmpPoints.ToArray();
            for (int i = 0; i < points.Length; i++)
            {
                points[i].x = points[i].x / 16.0f * piTimeValue;
                points[i].y = points[i].y / 8.0f;
            }
            Laser laser = new Laser();
            laser.points = points;
            exportData.lasers.Add(laser);
            julliaScript += "function Laser" + juliaIndex + "(timeValue::Float64)\r\n    laserIndex = "+ juliaIndex + "\r\n    return laserValue(laserIndex,timeValue,ionFile)\r\nend\r\n";
            juliaIndex++;

        }

        string outputJSON = JsonConvert.SerializeObject(exportData, Formatting.Indented);

        if (!Directory.Exists(pathToIonDirectory))
        {
            Directory.CreateDirectory(pathToIonDirectory);
            File.WriteAllText(pathToJuliaScript, julliaScript);
            File.WriteAllText(pathToIonFile, outputJSON);
        }
        else
        {
            File.WriteAllText(pathToJuliaScript, julliaScript);
            File.WriteAllText(pathToIonFile, outputJSON);
        }
    }

    /// <summary>
    /// Method <c>InitializeDropdownIon</c> initializes <c>dropdownIon</c> controll with imported data
    /// </summary>
    private void InitializeDropdownIon()
    {
        ionData = JsonConvert.DeserializeObject<IonData[]>(File.ReadAllText(pathToIonsFile)).ToList();
        dropdownIon.options.Clear();
        dropdownIon.value = 0;
        foreach (var ion in ionData) {
            dropdownIon.options.Add(new TMP_Dropdown.OptionData(ion.name));
        }
    }

    /// <summary>
    /// Method <c>InitializeConfigData</c> initializes variables: <c>pathToSave</c>, <c>pathToExport</c> and <c>pathToIonsFile</c> with data form config file
    /// </summary>
    private void InitializeConfigData()
    {
        ConfigData confData =  JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(Directory.GetCurrentDirectory() + "/config/config.json"));
        Debug.Log(Directory.GetCurrentDirectory() + "/config/config.json");
        Debug.Log(confData.pathToIonsFile);

        pathToExport = confData.pathToExport;
        pathToSave = confData.pathToSave;
        pathToIonsFile = confData.pathToIonsFile;
    }

    /// <summary>
    /// Method <c>AddDropdownIon</c> provide option to add new option to <c>dropdownIon</c> controll, with given option name and number of lasers
    /// </summary>
    /// <param name="newOption">Name of new option</param>
    /// <param name="numberofLasers">Numer of lasers for new option</param>
    private void AddDropdownIon(string newOption, int numberofLasers)
    {
        IonData newIon = new IonData();
        newIon.name = newOption;
        newIon.laserAmount = numberofLasers;
        ionData.Add(newIon);

        string outputJSON = JsonConvert.SerializeObject(ionData, Formatting.Indented);
        File.WriteAllText(pathToIonsFile, outputJSON);
        dropdownIon.options.Add(new TMP_Dropdown.OptionData(newOption));
        dropdownIon.value = dropdownIon.options.Count - 1;

    }

    /// <summary>
    /// Method <c>SaveIon</c> save the data of the currently processed ion
    /// </summary>
    private void SaveIon()
    {
        string nameOfIon = (buttonNewIon.GetValue()) ? inputIonName.text : dropdownIon.options[dropdownIon.value].text;
        string pathToIonDirectory = pathToSave + "/" + nameOfIon;
        float piTimeValue = float.Parse(inputPitime.text) * (float)Math.Pow(10, -3 * (dropdownPitimeUnit.value + 1));

        
        List<GraphShaderData> data = gridSection.GetComponentsInChildren<GraphShaderData>().ToList();
        List<Graph> list = new List<Graph>();
        foreach (var graph in data)
        {
            list.Add(graph.graphs[0]);
        }

        SaveData saveData = new SaveData();
        saveData.IonName = nameOfIon;
        saveData.numberOfLasers = list.Count;
        saveData.piTime = piTimeValue;
        saveData.graphs = list.ToArray();

        string outputJSON = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        string pathToIonFile = pathToIonDirectory + "/" + "Ion.json";

        if (!Directory.Exists(pathToIonDirectory))
        {
            Directory.CreateDirectory(pathToIonDirectory);
            File.WriteAllText(pathToIonFile, outputJSON);
            if (buttonNewIon.GetValue()) AddDropdownIon(nameOfIon, saveData.graphs.Length);
        }
        else
        {
            File.WriteAllText(pathToIonFile, outputJSON);
        }

    }

    /// <summary>
    /// Method <c>Update</c> called once per frame is reposonsible for hide or show <c>newMenu</c> controll based on <c>buttonNewIon</c>
    /// </summary>
    void Update()
    {
        if (buttonNewIon.GetValue())
        {
            newMenu.SetActive(true);
        }
        else
        {
            newMenu.SetActive(false);
        }
        
    }
}
