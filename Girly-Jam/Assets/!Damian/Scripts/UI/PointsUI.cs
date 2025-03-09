using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsUI : MonoBehaviour
{
    public TextMeshProUGUI totalPointsText;
    public TextMeshProUGUI pointsPerMinuteText;

    void Update()
    {
        if (PointManager.Instance != null && totalPointsText != null && pointsPerMinuteText != null)
        {
            totalPointsText.text = "Total Points: " + PointManager.Instance.getTotalPoints().ToString();
            pointsPerMinuteText.text = "Points P/M: " + PointManager.Instance.GetCurrentPointsPerMinute().ToString();
        }
    }
}