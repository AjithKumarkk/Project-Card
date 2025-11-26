using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GridLayoutManager grid;
    public MatchManager matchManager;
    public Button nextLevelButton;

    int currentLevel = 1;

    void Start()
    {
        currentLevel = Mathf.Max(1, GameSession.selectedLevel);

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextLevelPressed);
            nextLevelButton.gameObject.SetActive(false);
        }
        StartLevel(currentLevel);
    }

    public void StartLevel(int level)
    {
        currentLevel = level;

        grid.ClearGrid();
        matchManager.ResetAll();

        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);

        int rows = Random.Range(2, 5);
        int cols = Random.Range(2, 6);
        int total = rows * cols;
        if (total % 2 != 0)
        {
            if (cols < 6) cols++;
            else cols--;
        }

        grid.MakeGrid(rows, cols, seed: level);

        var gc = FindObjectOfType<GameController>();
        if (gc != null)
        {
            gc.rows = rows;
            gc.cols = cols;
            gc.seed = level;
            gc.OnGridReady();
        }
    }

    void CheckLevelComplete()
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
            HandleLevelComplete();
        }
    }

    public void HandleLevelComplete()
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int next = currentLevel + 1;
        int maxLevel = 10;
        if (next > unlocked && next <= maxLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", next);
            PlayerPrefs.Save();
        }

        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(true);
    }

    void OnNextLevelPressed()
    {
        GameSession.showLevelsOnMenuLoad = true;

        GameSession.selectedLevel = Mathf.Clamp(currentLevel + 1, 1, 10);

        SceneManager.LoadScene("MenuScene");
    }
}
