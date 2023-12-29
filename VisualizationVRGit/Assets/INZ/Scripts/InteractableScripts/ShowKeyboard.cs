using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>ShowKeyboard</c> shows and hides interactable keyboard
/// </summary>
public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;
    public float distance = 0.5f;
    public float verticaloffset = -0.5f;
    public Transform positionSource;


    /// <summary>
    /// Method <c>Start</c> is called before the first frame update, reposonsible for set up of class
    /// </summary>
    void Start()
    {
        inputField =GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(x => OpenKeyboard());
    }

    /// <summary>
    /// Method <c>OpenKeyboard</c> shows interactable keyboard
    /// </summary>
    public void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);

        Vector3 direction = positionSource.forward;
        direction.y = 0f;
        direction.Normalize();

        Vector3 targetPosition = positionSource.position + direction * distance + Vector3.up * verticaloffset;

        NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition);
        SetCaretColorAlpha(1);

        NonNativeKeyboard.Instance.OnClosed += Instance_OnClosed;
    }

    /// <summary>
    /// Method <c>Instance_OnClosed</c> clear up variables when keyboard hide
    /// </summary>
    /// <param name="sender">Object that send event</param>
    /// <param name="e">Event arguments</param>
    private void Instance_OnClosed(object sender, System.EventArgs e)
    {
        SetCaretColorAlpha(0);
        NonNativeKeyboard.Instance.OnClosed -= Instance_OnClosed;
    }

    /// <summary>
    /// Method <c>SetCaretColorAlpha</c> setting caret color alpha value to given value
    /// </summary>
    /// <param name="alpha">Value of alpha in color to set</param>
    public void SetCaretColorAlpha(float alpha)
    {
        inputField.customCaretColor = true;
        Color caretColor = inputField.caretColor;
        caretColor.a = alpha;
        inputField.caretColor = caretColor;
    }
}
