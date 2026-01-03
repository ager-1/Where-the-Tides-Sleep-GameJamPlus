using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TaskItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private Toggle taskToggle; // Changed from Button to Toggle

    public void Initialize(string taskDescription)
    {
        // Automatically add a bullet point
        taskText.text = "• " + taskDescription;
        
        // Reset toggle to false (unchecked) just in case
        taskToggle.isOn = false;

        // Listen for when the checkmark changes
        taskToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isChecked)
    {
        if (isChecked)
        {
            StartCoroutine(CompleteTaskRoutine());
        }
    }

    private IEnumerator CompleteTaskRoutine()
    {
        // 1. Wait a moment so the player sees the checkmark appear
        yield return new WaitForSeconds(0.5f);

        // 2. Fade out (Optional Visual Polish)
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>(); // Add one if missing
        
        float fadeTime = 0.5f;
        float startAlpha = cg.alpha;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(startAlpha, 0, t / fadeTime);
            yield return null;
        }

        // 3. Destroy the object
        Destroy(gameObject);
    }
}