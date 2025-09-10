using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    
    [Header("Audio (optional)")]
    public AudioSource sfx;
    public AudioClip gameOverSound; 

    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        if (spawner) spawner.enabled = false;
    }

    public void Play()
    {
        Time.timeScale = 1f;
        if (spawner) spawner.enabled = true; // ← להדליק ספאונר
        if (player) player.SetActiveDuringRun(true);
    }

    public void GameOver()
    {
        playButton.SetActive(true);
        gameOver.SetActive(true);

        if (sfx && gameOverSound) sfx.PlayOneShot(gameOverSound);
        Pause();
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
    
    public void Prepare()
    {
        Time.timeScale = 0f;

        playButton.SetActive(false);
        gameOver.SetActive(false);

        score = 0;
        scoreText.text = "0";

        if (spawner) spawner.enabled = false;
        foreach (var p in FindObjectsOfType<Pipes>())
            Destroy(p.gameObject);

        if (player) player.ResetState();
    }


}
