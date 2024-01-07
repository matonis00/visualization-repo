using UnityEngine;

/// <summary>
/// Class <c>QuitButton</c> used to close application
/// </summary>
public class QuitButton : MonoBehaviour
{
    /// <summary>
    /// Method <c>QuitApplication</c> quits application
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
