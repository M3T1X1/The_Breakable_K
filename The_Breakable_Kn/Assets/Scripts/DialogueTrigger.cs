using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject myDialogueUI; // Tu przeci¹gniesz swój BossDialogueText z Canvasa
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
        DialogueManager.Instance.StartDialogue(myDialogueUI, gameObject);
    }
}