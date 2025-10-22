using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    public Text scoreText; 
    public GameObject scorePanel; 

    private int collectedPackages = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (scorePanel != null)
        {
            Image panelImage = scorePanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.enabled = false; 
            }
        }

        UpdateScoreUI();
    }

    public void AddPackage()
    {
        collectedPackages++;
        UpdateScoreUI();
        Debug.Log($"Подарок собран! Всего: {collectedPackages}");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Подарки: {collectedPackages}";
        }
    }

    public void ResetScore()
    {
        collectedPackages = 0;
        UpdateScoreUI();
    }
}