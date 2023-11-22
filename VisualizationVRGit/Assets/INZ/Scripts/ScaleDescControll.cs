using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ScaleDescControll : MonoBehaviour
{

    public XRKnob knobScaleX;
    public TextMeshProUGUI scaleDesc1;
    public TextMeshProUGUI scaleDesc2;
    public TextMeshProUGUI scaleDesc3;
    public TextMeshProUGUI scaleDescAxiX;
    public String[] scaleDesc1Values = { "1ns", "10ns", "100ns", "1μs", "10μs", "100μs", "1ms", "10ms", "100ms"};
    public String[] scaleDesc2Values = { "5ns", "50ns", "500ns", "5μs", "50μs", "500μs", "5ms", "50ms", "500ms"};
    public String[] scaleDesc3Values = { "10ns", "100ns", "1μs", "10μs", "100μs", "1ms","10ms", "100ms", "1s"};
    // Start is called before the first frame update
    void Start()
    {
        knobScaleX.onValueChange.AddListener(ScaleDescAdjust);
    }

    private void ScaleDescAdjust(float arg0)
    {
        int exponent = (int)math.remap(0, 1, 0, 8, knobScaleX.value);
        scaleDesc1.text = scaleDesc1Values[exponent];
        scaleDesc2.text = scaleDesc2Values[exponent];
        scaleDesc3.text = scaleDesc3Values[exponent];
    }

}
