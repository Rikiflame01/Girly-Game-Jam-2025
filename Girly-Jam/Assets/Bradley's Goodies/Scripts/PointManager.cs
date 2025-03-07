using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class PointManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Point Variables")]

    [SerializeField] private int totalPoints;
    [SerializeField] private int clickPoints = 5;
    [SerializeField] private int passivePointsIn = 5;
    [SerializeField] private float timeToPassivePoints = 2;

    [Header("Point Per Minute Variables")]
    [SerializeField] private int currentPointsPerMinute;
    
    


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
            addPoints(clickPoints);
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
            yield return new WaitForSeconds(1);
            addPoints(passivePointsIn);
        }
    }

    IEnumerator calcAvgPointGain()
    {
        while (true)
        {
            int pointMark1 = totalPoints;
            yield return new WaitForSeconds(2);
            int pointMark2 = totalPoints;
            currentPointsPerMinute = (pointMark2 - pointMark1) * 30;

        }
    }


}
