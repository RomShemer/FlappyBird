using UnityEngine;

    /*public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    public AudioClip hitSound;
    public AudioSound pointSound;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            direction = Vector3.up * strength;
        }

        // Apply gravity and update the position
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        // Tilt the bird based on the direction
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            audioSource.PlayOneShot(hitSound);
            GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            GameManager.Instance.IncreaseScore();
        }
    }

}
*/

using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;

    [Header("SFX")]
    public AudioClip hitSound;
    public AudioClip pointSound;   
    [Range(0f,1f)] public float sfxVolume = 1f;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;
    private bool isDead;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;   
        spawnPos = transform.position;
        spawnRot = transform.rotation;
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void OnEnable()
    {
        isDead = false;
        var p = transform.position;
        p.y = 0f;
        transform.position = p;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            direction = Vector3.up * strength;

        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;

        var rot = transform.eulerAngles;
        rot.z = direction.y * tilt;
        transform.eulerAngles = rot;
    }

    private void AnimateSprite()
    {
        spriteIndex = (spriteIndex + 1) % sprites.Length;
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && !isDead)
        {
            isDead = true;
            if (hitSound) audioSource.PlayOneShot(hitSound, sfxVolume);
            GameManager.Instance.GameOver();
        }
        else if (other.CompareTag("Scoring"))
        {
            if (pointSound) audioSource.PlayOneShot(pointSound, sfxVolume);
            GameManager.Instance.IncreaseScore();
        }
    }
    
    public void ResetState()
    {
        isDead = false;

        transform.position = spawnPos;
        transform.rotation = spawnRot;

        direction = Vector3.zero;

        if (sprites != null && sprites.Length > 0)
        {
            spriteIndex = 0;
            spriteRenderer.sprite = sprites[0];
        }

        enabled = false;
        CancelInvoke(nameof(AnimateSprite));
    }

    public void SetActiveDuringRun(bool on)
    {
        enabled = on;

        CancelInvoke(nameof(AnimateSprite));
        if (on)
        {
            InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
        }
    }
}

