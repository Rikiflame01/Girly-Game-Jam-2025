using UnityEngine;
using System.IO;

public class ScreenshotTaker : MonoBehaviour
{
    private string desktopPath;

    private void Start()
    {
        desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
    }

    public void TakeScreenshot()
    {
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(desktopPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log("Screenshot saved to: " + fullPath);
    }
}
