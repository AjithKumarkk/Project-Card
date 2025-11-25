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
        if (randomizeOnStart)
            StartNewRandomGame();
        else
            StartNewGame();
    }
    void PickRandomLayout(out int pickRows, out int pickCols)
    {
        System.Random rng = new System.Random();

        int r = rng.Next(minRows, maxRows + 1);
        int c = rng.Next(minCols, maxCols + 1);

        if (r * c > maxTotalCells)
        {
            while (r * c > maxTotalCells && c > minCols) c--;
            while (r * c > maxTotalCells && r > minRows) r--;
        }

        if ((r * c) % 2 != 0)
        {
            if (c > minCols) c--;
            else if (r > minRows) r--;
            else
            {
                if (c < maxCols) c++;
                else if (r < maxRows) r++;
            }
        }

        r = Mathf.Clamp(r, minRows, maxRows);
        c = Mathf.Clamp(c, minCols, maxCols);
        if ((r * c) % 2 != 0)
        {
            c = Mathf.Max(minCols, c - 1);
        }

        pickRows = r;
        pickCols = c;
    }

    public void StartNewRandomGame()
    {
        int r, c;
        PickRandomLayout(out r, out c);

        rows = r;
        cols = c;

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
