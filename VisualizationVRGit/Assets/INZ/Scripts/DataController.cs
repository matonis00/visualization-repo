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
using static ShaderDataMultiple;

public class DataController : MonoBehaviour
{
    public GameObject gridSection;
    public XRSimpleInteractable buttonSave;
    public XRSimpleInteractable buttonExport;
    public TMP_Dropdown dropdownIon;
    public TMP_InputField inputPitime;
    public TMP_Dropdown dropdownPitimeUnit;
    public TMP_InputField inputIonName;
    [SerializeField] private string pathToSave;
    public string pathToExport;


    // Start is called before the first frame update
    [Serializable]
    public class saveData
    {
        public string IonName;
        public int numberOfLasers;
        public float piTime;
        public Graph[] graphs;
    }



    void Start()
    {
        dropdownIon.value = 1;
        buttonSave.selectEntered.AddListener(x => SaveIon());
        buttonExport.selectEntered.AddListener(x => ExportIon());
        InitializeDropdownIon();
        SaveIon();
    }

    private void ExportIon()
    {
        throw new NotImplementedException();
    }

    private void InitializeDropdownIon()
    {
        dropdownIon.options.Clear();
        dropdownIon.value = 0;
        string[] ions = Directory.GetDirectories(pathToSave);
        foreach (string ion in ions) {
            dropdownIon.options.Add(new TMP_Dropdown.OptionData(ion.Split('\\').Last()));
        }
    }

    private void ReloadDropdownIon(string newOption)
    {
        dropdownIon.options.Add(new TMP_Dropdown.OptionData(newOption));
        dropdownIon.value = dropdownIon.options.Count - 1;
    }


    private void SaveIon()
    {
        //Pobraæ nazwê
        string nameOfIon = dropdownIon.options[dropdownIon.value].text;
        string pathToIonDirectory = pathToSave + "/" + nameOfIon;
        float piTimeValue = float.Parse(inputPitime.text) * (float)Math.Pow(10, -3 * (dropdownPitimeUnit.value + 1));

        
        List<ShaderDataMultiple> data = gridSection.GetComponentsInChildren<ShaderDataMultiple>().ToList();
        List<Graph> list = new List<Graph>();
        foreach (var graph in data)
        {
            list.Add(graph.graphs[0]);
        }


        saveData saveData = new saveData();
        saveData.IonName = nameOfIon;
        saveData.numberOfLasers = list.Count;
        saveData.piTime = piTimeValue;
        saveData.graphs = list.ToArray();

        string outputJSON = JsonConvert.SerializeObject(saveData,Formatting.Indented);



        if (!Directory.Exists(pathToIonDirectory))
        {
            //Jak nie to stworzyæ folder i dodac plik
            Directory.CreateDirectory(pathToIonDirectory);
            //Add Option to Dropdown
            ReloadDropdownIon(nameOfIon);
        }

        string pathToIonFile = pathToIonDirectory + "/" + "Ion.json";
        
        File.WriteAllText(pathToIonFile, outputJSON);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
