using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public GridLayoutManager grid;      
    public MatchManager matchManager;   
    public MenuManager menuManager;     
    public Button returnButton;         
    public TMP_Text levelTitleText;    

    int currentLevel = 1;

    private void Start()
    {
        if (menuManager == null) menuManager = FindObjectOfType<MenuManager>();

        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() =>
            {
                menuManager?.ReturnToMenu();
            });
        }
    }

    public void StartLevel(int level)
    {
        currentLevel = level;
        // if (levelTitleText != null) levelTitleText.text = $"Level {level}";

        int rows = 2;
        int cols = 2;

        switch (level)
        {
            case 1: rows = 2; cols = 2; break;
            case 2: rows = 2; cols = 3; break;
            case 3: rows = 2; cols = 4; break;
            case 4: rows = 3; cols = 4; break;
            case 5: rows = 3; cols = 4; break;
            case 6: rows = 4; cols = 4; break;
            case 7: rows = 4; cols = 5; break;
            case 8: rows = 4; cols = 5; break;
            case 9: rows = 5; cols = 6; break;
            case 10: rows = 5; cols = 6; break;
            default:
                rows = Mathf.Clamp(2 + level/3, 2, 5);
                cols = Mathf.Clamp(2 + level/2, 2, 6);
                break;
        }

        grid.MakeGrid(rows, cols, seed: level); 
        matchManager.ResetAll();

        matchManager.OnPairResolved = (a, b, isMatch) =>
        {
            CheckLevelComplete();
        };
    }

    private void CheckLevelComplete()
    {
        bool allMatched = true;
        foreach (var c in grid.ActiveCards)
        {
            if (c != null && !c.IsMatched)
            {
                allMatched = false;
                break;
            }
        }

        if (allMatched)
        {
            OnCompleteLevel();
        }
    }

    public void OnCompleteLevel()
    {
        menuManager?.UnlockNextLevel(currentLevel);
    }

    public void ForceCompleteLevel()
    {
        OnCompleteLevel();
    }
}
