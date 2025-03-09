using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject shopCanvas;

    public void OpenShop()
    {
        shopCanvas.SetActive(true);
    }
}
