using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DataController : MonoBehaviour
{
    public GameObject gridSection;
    public XRSimpleInteractable buttonSave;
    public XRSimpleInteractable buttonExport;
    public TMP_Dropdown dropdownIon;
    public TMP_InputField inputPitime;
    public TMP_Dropdown dropdownPitimeUnit;
    public TMP_InputField inputIonName;
    public TwoStateButton buttonNewIon;
    public GameObject newMenu;
    [SerializeField] private string pathToSave;
    public string pathToExport;
    [SerializeField] private string pathToIonsFile;
    public List<IonData> ionData;

    
    void Start()
    {

        dropdownIon.value = 0;
        buttonSave.selectEntered.AddListener(x => SaveIon());
        buttonExport.selectEntered.AddListener(x => ExportIon());
        InitializeDropdownIon();
    }
    public bool CheckIfsaved(string nameOfIon)
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


    private void ExportIon()
    {
        string nameOfIon = (buttonNewIon.value) ? inputIonName.text : dropdownIon.options[dropdownIon.value].text;

        string pathToIonDirectory = pathToExport + "/" + nameOfIon;
        string pathToIonFile = pathToIonDirectory + "/" + "Ion.json";
        float piTimeValue = float.Parse(inputPitime.text) * (float)Math.Pow(10, -3 * (dropdownPitimeUnit.value + 1));
        List<ShaderDataMultiple> data = gridSection.GetComponentsInChildren<ShaderDataMultiple>().ToList();
        
        ExportData exportData = new ExportData();
        exportData.IonName = nameOfIon;
        exportData.piTime = piTimeValue;
        exportData.lasers = new List<Laser>();

        int juliaIndex = 1;
        string julliaScript = "include(\"D:/GIT/INZ/VisualizationRepo/VisualizationVRGit/Assets/INZ/Scripts/laserValueBaseScript.jl\")\r\n" + "ionFile = \"" + pathToIonFile.Replace('\\', '/')+ "\"\r\n";
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
            //Jak nie to stworzyæ folder i dodac plik
            Directory.CreateDirectory(pathToIonDirectory);
            File.WriteAllText(pathToJuliaScript, julliaScript);
            File.WriteAllText(pathToIonFile, outputJSON);
            //Add Option to Dropdown
            //if (buttonNewIon.value) AddDropdownIon(nameOfIon, exportData.lasers.Count);
            //else ReloadDropdownIon(nameOfIon);
        }
        else
        {
            File.WriteAllText(pathToJuliaScript, julliaScript);
            File.WriteAllText(pathToIonFile, outputJSON);
        }
    }

    private void InitializeDropdownIon()
    {
        ionData = JsonConvert.DeserializeObject<IonData[]>(File.ReadAllText(pathToIonsFile)).ToList();
        dropdownIon.options.Clear();
        dropdownIon.value = 0;
        //string[] ions = Directory.GetDirectories(pathToSave);
        foreach (var ion in ionData) {
            //ion.Split('\\').Last()
            dropdownIon.options.Add(new TMP_Dropdown.OptionData(ion.name));
        }
    }

    private void ReloadDropdownIon(string newOption)
    {
  
    }

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


    private void SaveIon()
    {
        //Pobraæ nazwê lub wybraæ obecn¹ w zale¿noœci od decyzji
        string nameOfIon = (buttonNewIon.value) ? inputIonName.text : dropdownIon.options[dropdownIon.value].text;
        string pathToIonDirectory = pathToSave + "/" + nameOfIon;
        float piTimeValue = float.Parse(inputPitime.text) * (float)Math.Pow(10, -3 * (dropdownPitimeUnit.value + 1));

        
        List<ShaderDataMultiple> data = gridSection.GetComponentsInChildren<ShaderDataMultiple>().ToList();
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
            //Jak nie to stworzyæ folder i dodac plik
            Directory.CreateDirectory(pathToIonDirectory);
            File.WriteAllText(pathToIonFile, outputJSON);
            //Add Option to Dropdown
            if (buttonNewIon.value) AddDropdownIon(nameOfIon, saveData.graphs.Length);
            else ReloadDropdownIon(nameOfIon);
        }
        else
        {
            File.WriteAllText(pathToIonFile, outputJSON);
        }

        
        
       

    }

    // Update is called once per frame
    void Update()
    {
        if (buttonNewIon.value)
        {
            newMenu.SetActive(true);
        }
        else
        {
            newMenu.SetActive(false);
        }
        
    }
}
