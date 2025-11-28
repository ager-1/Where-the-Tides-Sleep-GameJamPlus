using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ConversationManager.Instance.StartConversation(myConversation);
        }
    }
}