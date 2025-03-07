using System.Collections;
using UnityEngine;
public class PointManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Point Variables")]

    [SerializeField] private int totalPoints;
    [SerializeField] private int clickPoints = 5;
    [SerializeField] private int passivePointsIn = 5;

    [Header("Passive Points Calc")]
    [SerializeField] private float timeToPassivePoints = 60;
    [SerializeField] private int vibes; //Furniture
    [SerializeField] private int fans; //outfits
    [SerializeField] private int gigs; //instruments

    [Header("Point Per Minute Variables")]
    [SerializeField] private int currentPointsPerMinute;

    private int clickCount = 0;

    void Start()
    {
        StartCoroutine(startPassivePoints());
        StartCoroutine(calcAvgPointGain());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SFXManager.Instance.PlayAudio("Click");
            addPoints(clickPoints);
            clickCount++;
        }
    }

    public void setTotalPoints(int points)
    {
        totalPoints = points;
    }

    public int getTotalPoints()
    {
        return totalPoints;
    }

    public void addPoints(int points)
    {
        totalPoints += points;
    }

    public void setPassivePoints(int points)
    {
        passivePointsIn = points;
    }

    IEnumerator startPassivePoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToPassivePoints);
            addPoints(passivePointsIn);
        }
    }

    IEnumerator calcAvgPointGain()
    {
        while (true)
        {
            clickCount = 0;

            int initialClickCount = clickCount;
            yield return new WaitForSeconds(1);

            int finalClickCount = clickCount;

            int clicksInSeconds = finalClickCount - initialClickCount;

            currentPointsPerMinute = ((clicksInSeconds * clickPoints) * 60) + (passivePointsIn);
            Debug.Log("Clicks in last 1 seconds: " + clicksInSeconds);
        }
    }
}
