using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject taskItemPrefab; // Drag your Prefab here
    [SerializeField] private Transform taskContainer;   // Drag your 'TaskContainer' here

    // This simulates your "tasks at hand"
    private List<string> activeTasks = new List<string>()
    {
        "Find the luggage",
        "Talk to the Captain",
        "Get on board the ship"
    };

    void Start()
    {
        GenerateTaskUI();
    }

    void GenerateTaskUI()
    {
        // Clear existing items if any (optional safety step)
        foreach (Transform child in taskContainer)
        {
            Destroy(child.gameObject);
        }

        // Loop through your data and create UI for each
        foreach (string task in activeTasks)
        {
            // 1. Instantiate the prefab inside the container
            GameObject newTask = Instantiate(taskItemPrefab, taskContainer);

            // 2. Get the script and set the text
            if (newTask.TryGetComponent<TaskItemUI>(out TaskItemUI itemScript))
            {
                itemScript.Initialize(task);
            }
        }
    }

    // Call this if you want to add a new task dynamically during gameplay
    public void AddNewTask(string taskDescription)
    {
        activeTasks.Add(taskDescription);

        GameObject newTask = Instantiate(taskItemPrefab, taskContainer);
        if (newTask.TryGetComponent<TaskItemUI>(out TaskItemUI itemScript))
        {
            itemScript.Initialize(taskDescription);
        }
    }
}