using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using WanzyeeStudio.Json;

[Serializable]
public struct energy_level
{
    public int n;
    public int l;
    public float j; 
    public float f;
    public float E; 
};
[Serializable]
public struct transition
{
    public string multipole { get; set; }
    public float einsteinA { get; set; }
}
[Serializable]
public class Configuration 
{
    public double mass;
    public float charge;
    public float nuclear_spin;
    public Dictionary<string, energy_level> full_level_structure;
   // public Dictionary<Tuple<string, string>, transition> full_transitions { get; set; }
   // public string[] default_sublevel_selection;
   // public Dictionary<string, float> g_factors { get; set; }

    public static Configuration CreateFromJSON(string jsonString)
    {
        
        return JsonConvert.DeserializeObject<Configuration>(jsonString);
        //return JsonUtility.FromJson<Configuration>(jsonString);
    }
}




