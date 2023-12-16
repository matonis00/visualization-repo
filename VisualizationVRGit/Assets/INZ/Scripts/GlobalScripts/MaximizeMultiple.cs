using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.XR.Interaction.Toolkit;
using static ShaderDataMultiple;
using static System.Collections.Specialized.BitVector32;

public class MaximizeMultiple : MonoBehaviour
{
    public GameObject multipleGraphSection;
    public GameObject gridSection;
    public ShaderDataMultiple shaderDataMultiple;
    public bool multipleGraphShown =false;
    List<int> indexesList;
    // Start is called before the first frame update
    void Start()
    {
        multipleGraphSection.GetNamedChild("ButtonMinMax").GetComponentInChildren<XRSimpleInteractable>().selectEntered.AddListener(x => MaximizeSection());
        AddListenersToGridElements();
    }

    public void AddListenersToGridElements()
    {
        List<GameObject> sectionList = new List<GameObject>();
        gridSection.GetChildGameObjects(sectionList);
        foreach(GameObject section in sectionList)
        {
            section.GetNamedChild("ButtonMinMax").GetComponentInChildren<XRSimpleInteractable>().selectEntered.AddListener(x => MaximizeSection()); 
        }
    }


    // Update is called once per frame
    void MaximizeSection()
    {
        if (multipleGraphShown)
        {
            //Powr�t do grida
            List<GameObject> list = new List<GameObject>();
            gridSection.GetChildGameObjects(list);
            ShaderDataMultiple multipleshaderData = multipleGraphSection.GetComponentInChildren<GraphControll>().GetShaderData();
            int graphIndex = 0;
            foreach(int i in indexesList)
            {

                ShaderDataMultiple shaderData = list[i].GetComponentInChildren<GraphControll>().GetShaderData();
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
                bool choosen = list[i].GetNamedChild("ButtonChoose").GetComponentInChildren<TwoStateButton>().value;
                if(choosen)
                {
                    ShaderDataMultiple shaderData = list[i].GetComponentInChildren<GraphControll>().GetShaderData();
                    Graph graph = new Graph();
                    graph.points = shaderData.graphs[0].points;
                    graph.lineColor = shaderData.graphs[0].lineColor;
                    graph.pointColor = shaderData.graphs[0].pointColor;
                    graphs.Add(graph);
                    indexesList.Add(i);
                }
            }
            shaderDataMultiple.graphs = graphs.ToArray();

            gridSection.SetActive(false);
            multipleGraphSection.SetActive(true);
            multipleGraphShown = true;

        }



        //if (!multipleGraphShown)
        //{
        //    multipleGraphSection.SetActive(true);
        //    gridSection.SetActive(false);
        //    List<GameObject> list = new List<GameObject>();
        //    gridSection.GetChildGameObjects(list);
        //    List<Graph> graphs = new List<Graph>();
        //    foreach (GameObject go in list)
        //    {
        //        Maximize child = go.GetComponent<Maximize>();
        //        if( child.pushButtonMultiple.value)
        //        {
        //            Graph graph = new Graph();
        //            graph.points = child.shaderData.points;
        //            graph.lineColor = child.shaderData.lineColor;
        //            graph.pointColor = child.shaderData.pointColor;
        //            graphs.Add(graph);
        //        }
        //    }
        //    shaderDataMultiple.graphs = graphs.ToArray();
        //    multipleGraphShown = true;
        //}
        //else
        //{
        //    multipleGraphSection.SetActive(false);
        //    gridSection.SetActive(true);
        //    multipleGraphShown = false;
        //}
        
    }
}