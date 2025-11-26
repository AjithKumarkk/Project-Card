using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Canvas groups")]
    public CanvasGroup menuGroup;
    public CanvasGroup gameGroup;
    [Header("Menu UI")]
    public GameObject startButtonGO;
    public Button startButton;
    public GameObject levelsParent;
    public Button[] levelButtons;

    const string UNLOCK_KEY = "UnlockedLevel";
    const int DEFAULT_UNLOCKED = 1;

    void Awake()
    {
        if (!PlayerPrefs.HasKey(UNLOCK_KEY))
            PlayerPrefs.SetInt(UNLOCK_KEY, DEFAULT_UNLOCKED);

        if (menuGroup != null)
        {
            menuGroup.alpha = 1f;
            menuGroup.interactable = true;
            menuGroup.blocksRaycasts = true;
        }

        if (GameSession.showLevelsOnMenuLoad)
        {
            ShowMenuWithLevels();
            GameSession.showLevelsOnMenuLoad = false; 
        }
        else
        {
            ShowMenuInitialState();
        }
    }

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartPressed);
        }

        RefreshLevelButtons();
    }

    void ShowMenuInitialState()
    {
        if (menuGroup != null)
        {
            menuGroup.alpha = 1f;
            menuGroup.interactable = true;
            menuGroup.blocksRaycasts = true;
        }

        if (startButtonGO != null) startButtonGO.SetActive(true);
        if (levelsParent != null) levelsParent.SetActive(false);
    }

    public void ShowMenuWithLevels()
    {
        if (menuGroup != null)
        {
            menuGroup.alpha = 1f;
            menuGroup.interactable = true;
            menuGroup.blocksRaycasts = true;
        }

        if (startButtonGO != null) startButtonGO.SetActive(false);
        if (levelsParent != null) levelsParent.SetActive(true);

        RefreshLevelButtons();
    }

    public void OnStartPressed()
    {
        if (startButtonGO != null) startButtonGO.SetActive(false);
        if (levelsParent != null) levelsParent.SetActive(true);
        RefreshLevelButtons();
    }

    public void RefreshLevelButtons()
    {
        int unlocked = PlayerPrefs.GetInt(UNLOCK_KEY, DEFAULT_UNLOCKED);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            var btn = levelButtons[i];
            if (btn == null) continue;
            btn.interactable = (levelNumber <= unlocked);

            btn.onClick.RemoveAllListeners();
            int captured = levelNumber;
            btn.onClick.AddListener(() => OnLevelChosen(captured));
        }
    }

    void OnLevelChosen(int level)
    {
        GameSession.selectedLevel = level;

        GameSession.showLevelsOnMenuLoad = false;

        SceneManager.LoadScene("GameScene");
    }

    public void UnlockNextLevel(int completedLevel)
    {
        int unlocked = PlayerPrefs.GetInt(UNLOCK_KEY, DEFAULT_UNLOCKED);
        int next = completedLevel + 1;
        if (next > unlocked && next <= levelButtons.Length)
        {
            PlayerPrefs.SetInt(UNLOCK_KEY, next);
            PlayerPrefs.Save();
        }
    }
}
