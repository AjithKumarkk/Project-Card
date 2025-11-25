using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    public GridLayoutManager grid;
    public MatchManager matchManager;
    public CardPool pool;

    public ScoreManager scoreManager;
    public AudioManager audioManager;

    public TMP_Text scoreText;
    public TMP_Text pairsLeftText;

    public int rows = 4;
    public int cols = 4;
    public int seed = 0;

    int totalPairs = 0;
    int matched = 0;

    public float previewSeconds = 2f;

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        matched = 0;
        totalPairs = (rows * cols) / 2;

        matchManager.ResetAll();
        scoreManager?.ResetScore();

        grid.MakeGrid(rows, cols, seed);

        matchManager.OnPairResolved = null;
        matchManager.OnPairResolved = OnPairResolved;

        foreach (var card in grid.ActiveCards)
        {
            card.OnRevealed = null;
            card.OnHidden = null;
            card.OnMatched = null;


            var et = card.GetComponent<EventTrigger>();
            if (et != null) et.enabled = false;

            card.OnRevealed += (c) =>
            {
                matchManager.EnqueueWhenRevealed(c);
                audioManager?.PlayFlip();
            };

        }

        StartCoroutine(PreviewFlipRoutine());

        UpdateScoreUI();
        UpdatePairsText();
    }

    private void OnPairResolved(Card a, Card b, bool isMatch)
    {
        if (isMatch)
        {
            scoreManager?.RegisterMatch();
            matched++;
            UpdatePairsText();
            audioManager?.PlayMatch();
        }
        else
        {
            audioManager?.PlayMismatch();
        }

        UpdateScoreUI();
        if (matched >= totalPairs)
        {
            audioManager?.PlayGameComplete();
        }
    }

    IEnumerator PreviewFlipRoutine()
    {
        // 1) Reveal all cards
        foreach (var card in grid.ActiveCards)
        {
            card.Reveal();
        }

        // 2) Wait for preview time
        yield return new WaitForSeconds(previewSeconds);

        // 3) Hide all cards
        foreach (var card in grid.ActiveCards)
        {
            card.Hide();
        }

        // 4) Wait a bit for hide animations to finish
        yield return new WaitForSeconds(0.25f);

        // 5) Re-enable EventTriggers 
        foreach (var card in grid.ActiveCards)
        {
            var et = card.GetComponent<EventTrigger>();
            if (et != null) et.enabled = true;
        }

        matchManager.ResetAll();

        Debug.Log("Preview finished. Game started!");
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + (scoreManager != null ? scoreManager.GetScore().ToString() : "0");
    }

    void UpdatePairsText()
    {
        if (pairsLeftText != null)
        {
            int remaining = Mathf.Max(0, totalPairs - matched);
            pairsLeftText.text = $"Pairs left: {remaining}";
        }
    }
}
