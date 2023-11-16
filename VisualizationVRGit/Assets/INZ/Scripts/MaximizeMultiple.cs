using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static ShaderDataMultiple;

public class MaximizeMultiple : MonoBehaviour
{
    public GameObject sectionToShow;
    public GameObject scetionToHide;
    public XRSimpleInteractable[] pushButtons;
    public ShaderDataMultiple shaderDataMultiple;
    public bool multipleGraphShown =false;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var pushButton in pushButtons)
        {
            pushButton.selectEntered.AddListener(x => MaximizeSection());
        }
        
    }


    // Update is called once per frame
    void MaximizeSection()
    {
        
        if (!multipleGraphShown)
        {
            sectionToShow.SetActive(true);
            scetionToHide.SetActive(false);
            List<GameObject> list = new List<GameObject>();
            scetionToHide.GetChildGameObjects(list);
            List<Graph> graphs = new List<Graph>();
            foreach (GameObject go in list)
            {
                Maximize child = go.GetComponent<Maximize>();
                if( child.pushButtonMultiple.value)
                {
                    Graph graph = new Graph();
                    graph.points = child.shaderData.points;
                    graph.lineColor = child.shaderData.lineColor;
                    graph.pointColor = child.shaderData.pointColor;
                    graphs.Add(graph);
                }
            }
            shaderDataMultiple.graphs = graphs.ToArray();
            multipleGraphShown = true;
        }
        else
        {
            sectionToShow.SetActive(false);
            scetionToHide.SetActive(true);
            multipleGraphShown = false;
        }
        
    }
}
