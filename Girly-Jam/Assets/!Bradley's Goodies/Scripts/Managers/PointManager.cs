using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Gig Timer UI")]
    [SerializeField] private Image gigTimerRadial;
    private float initialGigTime = 80f;

    private int clickCount = 0;
    private float gigTimer = 80f;
    private bool isGigActive = false;

    public GameObject gigCompletionCanvas;
    public TextMeshProUGUI fansEarned;
    public TextMeshProUGUI pointsEarned;
    public TextMeshProUGUI totalFans;
    public TextMeshProUGUI totalVibes;

    void Start()
    {
        StartCoroutine(startPassivePoints());
        StartCoroutine(calcAvgPointGain());
        StartCoroutine(decreasePracticePercent());
        StartCoroutine(UpdateVibes());
        isGigActive = true;
        UpdateGigTimerUI();
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

        if (isGigActive)
        {
            gigTimer -= Time.deltaTime;
            UpdateGigTimerUI();
            if (gigTimer <= 0)
            {
                CompleteGig();
            }
        }

        totalFans.text = "Total Fans " + fans.ToString();
        totalVibes.text = "Total Vibes: " + vibes.ToString();
    }

    private void UpdateGigTimerUI()
    {
        if (gigTimerRadial != null)
        {
            float fillAmount = gigTimer / initialGigTime;
            gigTimerRadial.fillAmount = fillAmount;
        }
    }

    public void OnGigClick()
    {
        if (isGigActive)
        {
            gigTimer -= 1f;
            UpdateGigTimerUI();
            if (gigTimer <= 0)
            {
                CompleteGig();
            }
        }
    }

    private void CompleteGig()
    {
        isGigActive = false;
        gigTimer = 80f;
        int previousFans = fans;
        if (fans == 0)
        {
            fans += 50;
        }
        fans *= 2;
        addPoints(300);
        ShowGigCompletionCanvas(300, fans - previousFans);
        UpdateGigTimerUI();
        SFXManager.Instance.PlayAudio("Win");
    }

    private void ShowGigCompletionCanvas(int points, int fansGained)
    {
        gigCompletionCanvas.SetActive(true);
        fansEarned.text = fansGained.ToString();
        pointsEarned.text = points.ToString();
        StartCoroutine(HideGigCompletionCanvas());
    }

    IEnumerator HideGigCompletionCanvas()
    {
        yield return new WaitForSeconds(5f);
        gigCompletionCanvas.SetActive(false);
        isGigActive = true;
    }

    private void IncrementFans()
    {
        if (fans == 0)
        {
            fans += 100;
        }
        else
        {
            fans = Mathf.RoundToInt(fans * 1.25f);
        }
    }

    private int CalculateFanBonus()
    {
        return fans / 10;
    }

    IEnumerator UpdateVibes()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            int objectCount = FindObjectsByType<ObjectController>(FindObjectsSortMode.None).Length;
            vibes = objectCount * 50;
            float multiplier = Mathf.Pow(1.10f, objectCount);
            passivePointsIn += Mathf.RoundToInt(vibes * multiplier);
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

    public float GetPassiveBonus() 
    { 
        return practicePercentageIncrease;
    }

    IEnumerator startPassivePoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToPassivePoints);
            
            addPoints(Mathf.RoundToInt(passivePointsIn*practicePercentageIncrease));
            IncrementFans();
            addPoints(CalculateFanBonus());
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
