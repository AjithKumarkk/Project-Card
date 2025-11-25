using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float comboWindow = 2f; // time allowed between matches

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

        // Check if this is a combo
        if (now - lastMatchTime <= comboWindow)
            combo++;    // combo increases
        else
            combo = 1; // first match in chain

        lastMatchTime = now;

        // Scoring rules:
        // combo == 1 → normal → +2
        // combo >= 2 → combo → +4
        int gained = (combo == 1) ? 2 : 4;

        score += gained;

        OnScoreChanged?.Invoke(score);
    }

    public int GetScore() => score;
    public int GetCombo() => combo;
}
