using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>GridFiller</c> responsible for filling of <c>GraphGrid</c> with right number of <c>GraphSection</c> prefabs
/// </summary>
public class GridFiller : MonoBehaviour
{
    /// <summary>
    /// Variable that contains <c>GraphSection</c> prefab
    /// </summary>
    [SerializeField] private GameObject sectionPrefab;
    [SerializeField] private TMP_Dropdown ionDropdown;
    [SerializeField] private MaximizeController maximizeController;

    /// <summary>
    /// Method <c>Start</c> is called before the first frame update, reposonsible for set up of class 
    /// </summary>
    private void Start()
    {
        ionDropdown.onValueChanged.AddListener(Fill);
        Fill(ionDropdown.value);
    }

    /// <summary>
    /// Method <c>Fill</c> is responsible of filling <c>GraphGrid</c> with given number of <c>GraphSection</c> prefabs
    /// </summary>
    /// <param name="arg0">Number of section to create</param>
    private void Fill(int arg0)
    {
        int numberOfElements = InOutDataController.instance.ionData[arg0].laserAmount;
        string ionName = ionDropdown.options[arg0].text;
        SaveData saveData = null;
        if (InOutDataController.instance.CheckIfSaved(ionName))
        {
            saveData = InOutDataController.instance.LoadIonData(ionName);
        }

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int i = 0;i< numberOfElements; i++) 
        {
            GameObject child = Instantiate(sectionPrefab,transform.position,Quaternion.identity,transform);
            child.GetComponentInChildren<GraphShaderData>().graphs[0].lineColor = new Color((float)i / numberOfElements, 0, (float)(numberOfElements- i) / numberOfElements, 1);
            if(saveData != null)
            {
                child.GetComponentInChildren<GraphShaderData>().graphs[0].points = saveData.graphs[i].points;
            }
            
        }
        maximizeController.AddListenersToGridElements();
    }

}
