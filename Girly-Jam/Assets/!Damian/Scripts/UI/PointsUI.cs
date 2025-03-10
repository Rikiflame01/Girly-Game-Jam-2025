using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsUI : MonoBehaviour
{
    public TextMeshProUGUI totalPointsText;
    public TextMeshProUGUI pointsPerMinuteText;
    public TextMeshProUGUI bonusPointsText;

    void Update()
    {
        if (PointManager.Instance != null && totalPointsText != null && pointsPerMinuteText != null)
        {
            totalPointsText.text = "Total Points: " + PointManager.Instance.getTotalPoints().ToString();
            pointsPerMinuteText.text = "Points P/M: " + PointManager.Instance.GetCurrentPointsPerMinute().ToString();
            bonusPointsText.text = "Passive Point Bonus: " + PointManager.Instance.GetPassiveBonus().ToString() + "X";
        }
    }
}