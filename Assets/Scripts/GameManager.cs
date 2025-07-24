using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private GameObject screwPrefab;
    [SerializeField] private GameObject nutPrefab;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InputManager inputManager;

    private int currentLevel = 1;
    private List<Screw> screws = new List<Screw>();
    private LevelConfig.LevelData currentLevelData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        uiManager.ShowMainScreen();
    }

    public void LoadLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 20);
        currentLevelData = levelConfig.GetLevelData(currentLevel);
        ClearScrews();
        LevelGenerator.GenerateLevel(currentLevelData, screwPrefab, nutPrefab);
        uiManager.ShowGameScreen();
    }

    public void NextLevel()
    {
        if (currentLevel < 20)
        {
            LoadLevel(currentLevel + 1);
        }
        else
        {
            uiManager.ShowMainScreen();
        }
    }

    public void ResetLevel()
    {
        LoadLevel(currentLevel);
    }

    private void ClearScrews()
    {
        foreach (var screw in screws)
        {
            Destroy(screw.gameObject);
        }
        screws.Clear();
    }

    public void CheckWinCondition()
    {
        int colors = currentLevelData.screws.Count - 1; // n colors
        int[] colorCounts = new int[colors];
        int emptyScrews = 0;

        foreach (var screw in screws)
        {
            var nuts = screw.GetNuts();
            if (nuts.Count == 0)
            {
                emptyScrews++;
                continue;
            }
            if (nuts.Count != 4)
            {
                return;
            }
            int firstColor = nuts[0].ColorIndex;
            bool allSame = true;
            foreach (var nut in nuts)
            {
                if (nut.ColorIndex != firstColor)
                {
                    allSame = false;
                    break;
                }
            }
            if (!allSame)
            {
                return; // Not all nuts on this screw are the same color
            }
            colorCounts[firstColor] += nuts.Count;
        }

        // Check if all colors are fully sorted (4 nuts each)
        for (int i = 0; i < colors; i++)
        {
            if (colorCounts[i] != 4)
            {
                return;
            }
        }

        // Ensure exactly one screw is empty
        if (emptyScrews != 1)
        {
            return;
        }

        uiManager.ShowWinScreen();
    }

    public void RegisterScrew(Screw screw)
    {
        screws.Add(screw);
    }

    public int GetCurrentLevel() => currentLevel;
}