using UnityEngine.UI;
using UnityEngine;

public class ClosePanel : MonoBehaviour {
    public Canvas helpCanvas;

    public void Start()
    {
        helpCanvas.enabled = false;
    }

    public void openCanvas()
    {
        helpCanvas.enabled = true;
    }

    public void closeCanvas()
    {
        helpCanvas.enabled = false;
    }
}
