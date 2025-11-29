using UnityEngine;
using UnityEngine.UI;

public class ShowButtonOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject button; // Assign your button here

    private void Start()
    {
        // Hide button at start
        if (button != null)
        {
            button.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Show button when entering trigger
        if (button != null)
        {
            button.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide button when leaving trigger
        if (button != null)
        {
            button.SetActive(false);
        }
    }
}