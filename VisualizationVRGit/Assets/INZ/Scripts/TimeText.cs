using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class TimeText : MonoBehaviour
{
    public XRKnob knob;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    void FixedUpdate()
    {
        textMeshPro.text = "Time value:" + knob.value.ToString();
    }
}
