using System.Collections;
using UnityEngine;
public class PointManager : MonoBehaviour
{
    public static PointManager Instance;
    [Header("Point Variables")]

    [SerializeField] private int totalPoints;
    [SerializeField] private int clickPoints = 5;
    [SerializeField] private int passivePointsIn = 5;
    [SerializeField] private float practicePercentageIncrease = 1;

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
        StartCoroutine(decreasePracticePercent());
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SFXManager.Instance.PlayAudio("Click");
            addPoints(clickPoints);
            if (practicePercentageIncrease < 2.18f)
            {
                practicePercentageIncrease += 0.02f;
                practicePercentageIncrease = Mathf.Round(practicePercentageIncrease * 100f) / 100f;
            }
            clickCount++;
        }

        if (practicePercentageIncrease < 1)
        {
            practicePercentageIncrease = 1;
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

    public void decreasePoints(int points)
    {
        totalPoints -= points;
    }

    public void addPoints(int points)
    {
        totalPoints += points;
    }

    public void setPassivePoints(int points)
    {
        passivePointsIn = points;
    }

    public int GetCurrentPointsPerMinute()
    {
        return currentPointsPerMinute;
    }

    IEnumerator startPassivePoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToPassivePoints);
            
            addPoints(Mathf.RoundToInt(passivePointsIn*practicePercentageIncrease));
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
        }
    }

    IEnumerator decreasePracticePercent()
    {
        while (true)
        {
            if (practicePercentageIncrease <= 2 && practicePercentageIncrease > 1)
            {
                yield return new WaitForSeconds(2);
                practicePercentageIncrease -= 0.01f;
                practicePercentageIncrease = Mathf.Round(practicePercentageIncrease * 100f) / 100f;
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                practicePercentageIncrease -= 0.01f;
                practicePercentageIncrease = Mathf.Round(practicePercentageIncrease * 100f) / 100f;
            }
        }

    }
}
