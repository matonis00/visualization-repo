using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class <c>MaximizeController</c> control process of maximization and minimization of graph sections
/// </summary>
public class MaximizeController : MonoBehaviour
{
    /// <summary>
    /// Instance of <c>MaximizeController</c> class
    /// </summary>
    public static MaximizeController instance { get; private set; }


    [SerializeField] private GameObject multipleGraphSection;
    [SerializeField] private GameObject gridSection;
    [SerializeField] private GraphShaderData graphShaderData;
    [SerializeField] private bool multipleGraphShown = false;
    List<int> indexesList;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    /// <summary>
    /// Method <c>MaximizeSection</c> maximize choseen sections or minimize bigger section if its shown
    /// </summary>
    public void MaximizeSection()
    {
        if (multipleGraphShown)
        {
            List<GameObject> list = new List<GameObject>();
            gridSection.GetChildGameObjects(list);
            GraphShaderData multipleshaderData = multipleGraphSection.GetComponentInChildren<GraphControll>().GetShaderData();
            int graphIndex = 0;
            foreach(int i in indexesList)
            {

                GraphShaderData shaderData = list[i].GetComponentInChildren<GraphControll>().GetShaderData();
                Graph graph = multipleshaderData.graphs[graphIndex];
                shaderData.graphs[0].points = graph.points;
                shaderData.lineColor = graph.lineColor;
                shaderData.pointColor = graph.pointColor;
                graphIndex++;
            }
            gridSection.SetActive(true);
            multipleGraphSection.SetActive(false);
            multipleGraphShown = false;
        }
        else
        {
            List<GameObject> list = new List<GameObject>();
            gridSection.GetChildGameObjects(list);
            List<Graph> graphs = new List<Graph>();
            indexesList = new List<int>();
            for (int i = 0;i<list.Count;i++)
            {
                bool choosen = list[i].GetNamedChild("ButtonChoose").GetComponentInChildren<TwoStateButton>().GetValue();
                if(choosen)
                {
                    GraphShaderData shaderData = list[i].GetComponentInChildren<GraphControll>().GetShaderData();
                    Graph graph = new Graph();
                    graph.points = shaderData.graphs[0].points;
                    graph.lineColor = shaderData.graphs[0].lineColor;
                    graph.pointColor = shaderData.graphs[0].pointColor;
                    graphs.Add(graph);
                    indexesList.Add(i);
                }
            }
            graphShaderData.graphs = graphs.ToArray();

            gridSection.SetActive(false);
            multipleGraphSection.SetActive(true);
            multipleGraphShown = true;
        } 
    }
}
