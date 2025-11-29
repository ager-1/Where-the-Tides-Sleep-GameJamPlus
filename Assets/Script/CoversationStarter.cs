using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;
    private bool isInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        isInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInTrigger = false;
    }

    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            ConversationManager.Instance.StartConversation(myConversation);
        }
    }
}