using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public List<GameObject> dialogueSequence; // Lista dymków w kolejnoœci
    public bool triggerOnStart = true;

    void Start()
    {
        if (triggerOnStart)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogueSequence(dialogueSequence, gameObject);
    }
}