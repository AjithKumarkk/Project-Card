using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float comboWindow = 2f;

    int score = 0;
    int combo = 0;
    float lastMatchTime = -999f;

    public Action<int> OnScoreChanged;

    public void ResetScore()
    {
        score = 0;
        combo = 0;
        lastMatchTime = -999f;
        OnScoreChanged?.Invoke(score);
    }

    public void RegisterMatch()
    {
        float now = Time.time;

       
        if (now - lastMatchTime <= comboWindow)
            combo++;   
        else
            combo = 1; 

        lastMatchTime = now;

        int gained = (combo == 1) ? 2 : 4;

        score += gained;

        OnScoreChanged?.Invoke(score);
    }

    public int GetScore() => score;
    public int GetCombo() => combo;
}
