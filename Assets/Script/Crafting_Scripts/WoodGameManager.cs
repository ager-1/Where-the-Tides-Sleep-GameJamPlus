using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;



public class WoodGameManager3D : MonoBehaviour
{
    [Header("3D Settings")]
    public GameObject woodPrefab;    // Your 3D Wood Model
    public Transform basket;         // Your 3D Basket
    public Transform spawnPoint;     // An empty object where wood starts falling
    public float spawnWidth = 8f;    // How wide the spawn area is (Left to Right)

    [Header("Data")]
    public WoodInventory inventory;  // Drag your Inventory Asset
    public WoodData woodData;        // Drag your Wood Data Asset

    [Header("Game Logic")]
    public float spawnRate = 1.0f;
    public int maxLives = 3;
    public string gameOverScene = "GameOverScene";

    [Header("UI (Canvas)")]
    public TMP_Text scoreText;
    public TMP_Text livesText;

    private float nextSpawn;
    private int score;
    private int lives;
    private bool isGameOver;

    void Start()
    {
        lives = maxLives;
        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (Time.time > nextSpawn)
        {
            Spawn();
            nextSpawn = Time.time + spawnRate;
        }
    }

    void Spawn()
    {
        // 1. Pick a random X position
        float randomX = Random.Range(-spawnWidth, spawnWidth);
        Vector3 spawnPos = new Vector3(randomX, spawnPoint.position.y, spawnPoint.position.z);

        // 2. Spawn the specific "SM_Prop_Debris_03" model
        // We use 'Quaternion.identity' here to respect the Prefab's default rotation first
        GameObject newWood = Instantiate(woodPrefab, spawnPos, Quaternion.identity);

        // 3. Give it a random tumble/rotation so it doesn't look stiff
        newWood.transform.rotation = Random.rotation;

        // 4. Initialize Script
        // We add the script automatically so you don't have to edit the prefab
        FallingWood3D script = newWood.AddComponent<FallingWood3D>();
        script.Setup(this, woodData, basket);
    }

    public void CatchWood(WoodData data)
    {
        score++;
        inventory.AddWood(data);
        UpdateUI();
    }

    public void MissWood()
    {
        lives--;
        UpdateUI();
        if (lives <= 0) GameOver();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Wood: " + score;
        if (livesText) livesText.text = "Lives: " + lives;
    }

    void GameOver()
    {
        isGameOver = true;
        SceneManager.LoadScene(gameOverScene);
    }
}