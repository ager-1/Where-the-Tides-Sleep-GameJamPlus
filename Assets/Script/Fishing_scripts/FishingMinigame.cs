using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // Needed for scene switching
using TMPro;


public class FishingMinigame : MonoBehaviour
{
    public enum State { Idle, WaitingForBite, Running, Result, GameOver }

    [Header("Game Settings")]
    public int maxLives = 3;
    public string gameOverSceneName = "GameOverScene"; // EXACT name of your scene
    private int currentLives;

    [Header("Data References")]
    public FishInventory inventory; // Where we save the fish
    public FishData currentFish;    // The fish we are trying to catch

    [Header("UI References")]
    public RectTransform trackArea;   // The grey background bar
    public RectTransform marker;      // The moving red hook
    public RectTransform successZone; // The green target area
    public TMP_Text statusText;       // "Catch!" or "Miss!"
    public TMP_Text livesText;        // "Lives: 3/3"

    [Header("Events")]
    public UnityEvent onCatch;      // Drag FishVisualizer here
    public UnityEvent onGameOver;   // Optional: Sound effects etc.

    // Internal State
    private State state = State.Idle;
    private float t = 0.5f;
    private int dir = 1;
    private float biteTimer;
    private float currentSpeed;

    void Start()
    {
        // 1. Force Unity to calculate UI size to prevent "Invisible Bar" bug
        Canvas.ForceUpdateCanvases();

        // 2. Setup Lives
        currentLives = maxLives;
        UpdateLivesUI();

        // 3. Start the loop
        if (currentFish != null) StartFishing();
        else Debug.LogError("No FishData assigned in the Inspector!");
    }

    void Update()
    {
        // Check Input (Spacebar or Left Click)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && state == State.Running)
        {
            Evaluate();
        }

        // State Machine
        if (state == State.WaitingForBite)
        {
            biteTimer -= Time.deltaTime;
            if (biteTimer <= 0f)
            {
                state = State.Running;
                if (statusText) statusText.text = "PRESS NOW!";
            }
        }
        else if (state == State.Running)
        {
            MoveMarker();
        }
    }

    public void StartFishing()
    {
        if (state == State.GameOver) return;

        // Get difficulty from the Fish Asset
        currentSpeed = currentFish.movementSpeed;
        SetZoneSize(currentFish.zoneSize);
        RandomizeZonePosition();

        // Reset Marker
        t = Random.Range(0.2f, 0.8f);

        // Randomize wait time
        biteTimer = Random.Range(1f, 2.5f);

        state = State.WaitingForBite;
        if (statusText) statusText.text = "Waiting...";
    }

    void MoveMarker()
    {
        // Move the value 't' back and forth between 0 and 1
        t += dir * currentSpeed * Time.deltaTime;

        if (t >= 1f) { t = 1f; dir = -1; }
        else if (t <= 0f) { t = 0f; dir = 1; }

        // Apply that value to the UI position
        float y = Mathf.Lerp(-trackArea.rect.height / 2, trackArea.rect.height / 2, t);
        marker.anchoredPosition = new Vector2(0, y);
    }

    void Evaluate()
    {
        state = State.Result;

        if (IsInsideZone())
        {
            // --- SUCCESS ---
            if (statusText) statusText.text = $"CAUGHT {currentFish.fishName.ToUpper()}!";

            // 1. Save to asset
            if (inventory) inventory.AddFish(currentFish);

            // 2. Show Visuals
            onCatch.Invoke();

            // 3. Restart after 4 seconds (time to admire the fish)
            Invoke(nameof(StartFishing), 4f);
        }
        else
        {
            // --- FAIL ---
            currentLives--;
            UpdateLivesUI();

            if (currentLives > 0)
            {
                if (statusText) statusText.text = "MISSED!";
                // Retry quickly
                Invoke(nameof(StartFishing), 1.5f);
            }
            else
            {
                HandleGameOver();
            }
        }
    }

    void HandleGameOver()
    {
        state = State.GameOver;
        if (statusText) statusText.text = "GAME OVER";
        onGameOver.Invoke();

        // Load the scene specified in the Inspector
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            Debug.Log("Loading Scene: " + gameOverSceneName);
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogError("You forgot to type the Scene Name in the Inspector!");
        }
    }

    // --- Helpers ---

    void UpdateLivesUI()
    {
        if (livesText) livesText.text = $"LIVES: {currentLives}/{maxLives}";
    }

    bool IsInsideZone()
    {
        float markerY = marker.anchoredPosition.y;
        float zoneY = successZone.anchoredPosition.y;
        float halfZone = successZone.rect.height / 2;
        return markerY >= (zoneY - halfZone) && markerY <= (zoneY + halfZone);
    }

    void SetZoneSize(float percentage)
    {
        // Clamp it to prevent invisible zones
        float safePercentage = Mathf.Clamp(percentage, 0.1f, 0.9f);

        var size = successZone.sizeDelta;
        size.y = trackArea.rect.height * safePercentage;
        successZone.sizeDelta = size;
    }

    void RandomizeZonePosition()
    {
        float range = (trackArea.rect.height / 2) - (successZone.rect.height / 2);
        float y = Random.Range(-range, range);
        successZone.anchoredPosition = new Vector2(0, y);
    }
}