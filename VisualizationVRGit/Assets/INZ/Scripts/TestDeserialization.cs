using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WanzyeeStudio.Json;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class D32
{
    public int n;
    public int l;
    public double j;
    public double f;
    public double E;
}

public class D32P12
{
    public string multipole;
    public double einsteinA;
}

public class D32P32
{
    public string multipole;
    public double einsteinA;
}

public class D52
{
    public int n;
    public int l;
    public double j;
    public double f;
    public double E;
}

public class D52P32
{
    public string multipole;
    public double einsteinA;
}

public class FullLevelStructure
{
    [JsonProperty("S1/2")]
    public S12 S12;

    [JsonProperty("D3/2")]
    public D32 D32;

    [JsonProperty("D5/2")]
    public D52 D52;

    [JsonProperty("P1/2")]
    public P12 P12;

    [JsonProperty("P3/2")]
    public P32 P32;
}

public class FullTransitions
{
    [JsonProperty("(\"D3/2\", \"P3/2\")")]
    public D32P32 D32P32;

    [JsonProperty("(\"S1/2\", \"P1/2\")")]
    public S12P12 S12P12;

    [JsonProperty("(\"D3 / 2\", \"P1 / 2\")")]
    public D32P12 D32P12;

    [JsonProperty("(\"D5 / 2\", \"P3 / 2\")")]
    public D52P32 D52P32;

    [JsonProperty("(\"S1 / 2\", \"D3 / 2\")")]
    public S12D32 S12D32;

    [JsonProperty("(\"S1 / 2\", \"D5 / 2\")")]
    public S12D52 S12D52;

    [JsonProperty("(\"S1 / 2\", \"P3 / 2\")")]
    public S12P32 S12P32;
}

public class GFactors
{
    [JsonProperty("D5/2")]
    public double D52;

    [JsonProperty("S1/2")]
    public double S12;
}

public class P12
{
    public int n;
    public int l;
    public double j;
    public double f;
    public double E;
}

public class P32
{
    public int n;
    public int l;
    public double j;
    public double f;
    public double E;
}

public class Root
{
    public double mass;
    public int charge;
    public int nuclear_spin;
    public FullLevelStructure full_level_structure;
    public FullTransitions full_transitions;
    public List<List<string>> default_sublevel_selection;
    public GFactors g_factors;
}

public class S12
{
    public int n;
    public int l;
    public double j;
    public double f;
    public int E;
}

public class S12D32
{
    public string multipole;
    public double einsteinA;
}

public class S12D52
{
    public string multipole;
    public double einsteinA;
}

public class S12P12
{
    public string multipole;
    public double einsteinA;
}

public class S12P32
{
    public string multipole;
    public double einsteinA;
}








public class TestDeserialization : MonoBehaviour
{
    // Start is called before the first frame update
    public TextAsset jsonFile;
    void Start()
    {
        DictionaryConverter dictionaryConverter = new DictionaryConverter();
        //Configuration cofig= Configuration.CreateFromJSON(jsonFile.text);
        //Root cofig = dictionaryConverter.ReadJson(jsonFile.text);
        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonFile.text);

        Debug.Log(myDeserializedClass);
        Debug.Log(myDeserializedClass.mass);
        Debug.Log(myDeserializedClass.charge);
        Debug.Log(myDeserializedClass.nuclear_spin);
        Debug.Log(myDeserializedClass.full_level_structure.P12);
        Debug.Log(myDeserializedClass.full_transitions.S12P12.einsteinA);
        Debug.Log(myDeserializedClass.default_sublevel_selection[0][1]);
        Debug.Log(myDeserializedClass.g_factors.S12);
    }

}
