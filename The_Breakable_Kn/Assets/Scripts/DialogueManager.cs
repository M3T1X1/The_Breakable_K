using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject player;

    private bool isDialogueActive = false;
    private List<GameObject> dialogueQueue = new List<GameObject>();
    private int currentIndex = 0;
    private GameObject currentBoss;

    void Awake() { Instance = this; }

    public void StartDialogueSequence(List<GameObject> sequence, GameObject bossObject = null)
    {
        dialogueQueue = sequence;
        currentIndex = 0;
        currentBoss = bossObject;
        isDialogueActive = true;

        if (player != null && player.GetComponent<PlayerMovement>() != null)
            player.GetComponent<PlayerMovement>().enabled = false;

        if (currentBoss != null && currentBoss.GetComponent<KingBossAI>() != null)
        {
            currentBoss.GetComponent<KingBossAI>().enabled = false;
            Rigidbody2D bossRb = currentBoss.GetComponent<Rigidbody2D>();
            if (bossRb != null) bossRb.linearVelocity = Vector2.zero;
        }

        ShowCurrentDialogue();
    }

    void Update()
    {
        if (isDialogueActive && Mouse.current.leftButton.wasPressedThisFrame)
        {
            NextDialogue();
        }
    }

    void ShowCurrentDialogue()
    {
        foreach (var obj in dialogueQueue) obj.SetActive(false);

        if (currentIndex < dialogueQueue.Count)
        {
            dialogueQueue[currentIndex].SetActive(true);
        }
    }

    void NextDialogue()
    {
        currentIndex++;
        if (currentIndex < dialogueQueue.Count)
        {
            ShowCurrentDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        foreach (var obj in dialogueQueue) obj.SetActive(false);

        if (player != null && player.GetComponent<PlayerMovement>() != null)
            player.GetComponent<PlayerMovement>().enabled = true;

        if (currentBoss != null && currentBoss.GetComponent<KingBossAI>() != null)
        {
            currentBoss.GetComponent<KingBossAI>().enabled = true;
        }

    }
}