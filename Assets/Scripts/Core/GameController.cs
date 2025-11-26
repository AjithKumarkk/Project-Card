using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    public GridLayoutManager grid;
    public MatchManager matchManager;
    public CardPool pool;
    public LevelManager levelManager;

    public ScoreManager scoreManager;
    public AudioManager audioManager;

    public TMP_Text scoreText;
    public TMP_Text pairsLeftText;

    [Header("Random layout settings")]
    public bool randomizeOnStart = true;
    public int minRows = 2;
    public int maxRows = 5;
    public int minCols = 2;
    public int maxCols = 6;
    public int rows = 4;
    public int cols = 4;
    public int seed = 0;
    public int maxTotalCells = 30;

    int totalPairs = 0;
    int matched = 0;

    public float previewSeconds = 2f;

    void Start()
    {
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();
    }

    public void StartNewGame()
    {
        matched = 0;
        totalPairs = (rows * cols) / 2;

        matchManager.ResetAll();
        scoreManager?.ResetScore();

        matchManager.OnPairResolved = null;
        matchManager.OnPairResolved = OnPairResolved;

        UpdateScoreUI();
        UpdatePairsText();
    }

    public void OnGridReady()
    {
        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        totalPairs = (grid.ActiveCards.Count) / 2;
        matched = 0;
        scoreManager?.ResetScore();

        matchManager.OnPairResolved = null;
        matchManager.OnPairResolved = OnPairResolved;

        foreach (var card in grid.ActiveCards)
        {
            card.OnRevealed = null;
            card.OnHidden = null;
            card.OnMatched = null;

            var et = card.GetComponent<EventTrigger>();
            if (et != null) et.enabled = false;

            card.OnMatched += (c) => { };
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
            if (levelManager == null)
                levelManager = FindObjectOfType<LevelManager>();
            levelManager?.HandleLevelComplete();
        }
    }

    IEnumerator PreviewFlipRoutine()
    {
        foreach (var card in grid.ActiveCards) card.Reveal();
        yield return new WaitForSeconds(previewSeconds);
        foreach (var card in grid.ActiveCards) card.Hide();
        yield return new WaitForSeconds(0.25f);

        foreach (var card in grid.ActiveCards)
        {
            var c = card;
            c.OnRevealed += (revealedCard) =>
            {
                matchManager.EnqueueWhenRevealed(revealedCard);
                audioManager?.PlayFlip();
            };

            var et = c.GetComponent<EventTrigger>();
            if (et != null) et.enabled = true;
        }
        matchManager.ResetAll();
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
