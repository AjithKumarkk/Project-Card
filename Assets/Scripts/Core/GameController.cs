using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GridLayoutManager grid;
    public MatchManager matchManager;
    public CardPool pool;

    public int rows = 4;
    public int cols = 4;
    public int seed = 0;

    int totalPairs = 0;
    int matched = 0;

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        matched = 0;
        totalPairs = (rows * cols) / 2;
        matchManager.ResetAll();
        grid.MakeGrid(rows, cols, seed);

        // wire card events
        foreach (var card in grid.ActiveCards)
        {
            // clear old callbacks
            card.OnRevealed = null;
            card.OnHidden = null;
            card.OnMatched = null;

            card.OnRevealed += (c) => {
                matchManager.EnqueueWhenRevealed(c);
            };

            card.OnMatched += (c) => {
                matched++;
                Debug.Log("Matched: " + matched + "/" + totalPairs);
                if (matched >= totalPairs)
                {
                    Debug.Log("Game finished!");
                }
            };
        }
    }
}
