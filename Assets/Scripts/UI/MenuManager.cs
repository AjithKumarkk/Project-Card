using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject menuCanvas;
    public GameObject mainCanvas;
    public CanvasGroup menuGroup;
    public CanvasGroup gameGroup;

    [Header("Menu UI")]
    public Button startButton;
    public Button[] levelButtons;

    const string UNLOCK_KEY = "UnlockedLevel";
    const int DEFAULT_UNLOCKED = 1;

    private void Awake()
    {
        // Ensure there's always at least level 1 unlocked
        if (!PlayerPrefs.HasKey(UNLOCK_KEY))
            PlayerPrefs.SetInt(UNLOCK_KEY, DEFAULT_UNLOCKED);
    }

    void Start()
    {
        ShowMenu();
        // Start button opens level 1
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() =>
        {
            OnLevelSelected(1);
        });
    }
    public void ShowMenu()
    {
        menuGroup.alpha = 1; menuGroup.interactable = true; menuGroup.blocksRaycasts = true;
        gameGroup.alpha = 0; gameGroup.interactable = false; gameGroup.blocksRaycasts = false;
    }

    public void ShowGame()
    {
        menuGroup.alpha = 0; menuGroup.interactable = false; menuGroup.blocksRaycasts = false;
        gameGroup.alpha = 1; gameGroup.interactable = true; gameGroup.blocksRaycasts = true;
    }
    public void RefreshLevelButtons()
    {
        int unlocked = PlayerPrefs.GetInt(UNLOCK_KEY, DEFAULT_UNLOCKED);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = (levelNumber <= unlocked);

                levelButtons[i].onClick.RemoveAllListeners();
                int captured = levelNumber;
                levelButtons[i].onClick.AddListener(() => OnLevelSelected(captured));
            }
        }
    }

    public void OnLevelSelected(int level)
    {
        // hide menu, show game
        ShowGame();

        // tell LevelManager to start the level
        var lm = FindObjectOfType<LevelManager>();
        if (lm != null) lm.StartLevel(level);
    }

    public void UnlockNextLevel(int completedLevel)
    {
        int unlocked = PlayerPrefs.GetInt(UNLOCK_KEY, DEFAULT_UNLOCKED);
        int next = completedLevel + 1;
        if (next > unlocked)
        {
            if (next <= levelButtons.Length)
            {
                PlayerPrefs.SetInt(UNLOCK_KEY, next);
                PlayerPrefs.Save();
            }
        }
        RefreshLevelButtons();
    }

    public void ReturnToMenu()
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
        if (menuCanvas != null) menuCanvas.SetActive(true);
        RefreshLevelButtons();
    }
}
