using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class StartSequence : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup getReadyGroup; 
    public Image getReadyImage;
    public TMP_Text countdownText;    
    public GameObject playButton;
    public GameObject gameOverPanel;
    public GameObject playerBird;

    [Header("Timing")]
    public float readyHold = 0.25f;   
    public float betweenNumbers = 1.0f; 
    public bool freezeTimeDuringCountdown = true;

    [Header("Behavior")]
    public bool overlayCountdownOnImage = false; 

    [Header("Audio (optional)")]
    public AudioSource sfx;
    public AudioClip tick; 
    public AudioClip go;    

    bool running;
    void Awake()
    {
        if (getReadyGroup) getReadyGroup.gameObject.SetActive(false);
        if (getReadyImage) getReadyImage.gameObject.SetActive(false);
        if (countdownText) countdownText.gameObject.SetActive(false);
    }


    public void OnPlayPressed()
    {
        if (running) return;
        GameManager.Instance.Prepare(); 
        if (playerBird) playerBird.gameObject.SetActive(false);
        if (playButton) playButton.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (getReadyGroup)
        {
            getReadyGroup.gameObject.SetActive(true);
            getReadyGroup.alpha = 1f;
        }
        if (getReadyImage) getReadyImage.gameObject.SetActive(true);
        if (countdownText) countdownText.gameObject.SetActive(false);
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        running = true;

        if (getReadyImage) {
            getReadyImage.gameObject.SetActive(true);
            getReadyImage.enabled = true;
            yield return new WaitForSecondsRealtime(0.4f);
        }

        float prevScale = Time.timeScale;
        if (freezeTimeDuringCountdown) Time.timeScale = 0f;

        if (readyHold > 0f)
            yield return new WaitForSecondsRealtime(readyHold);

        if (!overlayCountdownOnImage)
        {
            getReadyImage.gameObject.SetActive(false);
        }

        if (countdownText)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.transform.SetAsLastSibling(); 
        }

        if (getReadyImage) {
            getReadyImage.gameObject.SetActive(false);
        }

        for (int n = 3; n >= 1; n--)
        {
            if (countdownText)
            {
                countdownText.SetText("{0}", n);
                countdownText.transform.localScale = Vector3.one * 1.1f;
            }
            if (sfx && tick) sfx.PlayOneShot(tick);
            yield return new WaitForSecondsRealtime(betweenNumbers);
        }

        if (countdownText) countdownText.SetText("GO!");
        if (sfx && go)
        {
            sfx.PlayOneShot(go);
            {
                float len = go.length / (sfx.pitch == 0f ? 1f : Mathf.Abs(sfx.pitch));
                float t = 0f;
                while (t < len)
                {
                    t += Time.unscaledDeltaTime; 
                    yield return null;
                }
            }
        }else
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }

        if (freezeTimeDuringCountdown) Time.timeScale = prevScale;
        playerBird.gameObject.SetActive(true);
        GameManager.Instance.Play();

        if (countdownText)  countdownText.gameObject.SetActive(false);
        if (getReadyGroup)  getReadyGroup.gameObject.SetActive(false);

        running = false;
    }
    
    IEnumerator FadeCanvas(CanvasGroup cg, float duration, float target)
    {
        float start = cg.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        cg.alpha = target;
    }
}
