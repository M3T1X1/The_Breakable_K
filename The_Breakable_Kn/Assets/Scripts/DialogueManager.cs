using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject player;

    private bool isDialogueActive = false;
    private GameObject currentDialogueUI;
    private GameObject currentBoss;

    void Awake() { Instance = this; }

    public void StartDialogue(GameObject dialogueUI, GameObject bossObject = null)
    {
        isDialogueActive = true;
        currentDialogueUI = dialogueUI;
        currentBoss = bossObject;

        if (currentDialogueUI != null) currentDialogueUI.SetActive(true);

        // Blokada ruchu i walki gracza
        if (player != null && player.GetComponent<PlayerMovement>() != null)
            player.GetComponent<PlayerMovement>().enabled = false;

        // Blokada AI Bossa
        if (currentBoss != null && currentBoss.GetComponent<KingBossAI>() != null)
            currentBoss.GetComponent<KingBossAI>().enabled = false;
    }

    void Update()
    {
        // Powrót do LPM (Lewy Przycisk Myszy)
        if (isDialogueActive)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                EndDialogue();
            }
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        if (currentDialogueUI != null) currentDialogueUI.SetActive(false);

        // Odblokowanie gracza
        if (player != null && player.GetComponent<PlayerMovement>() != null)
            player.GetComponent<PlayerMovement>().enabled = true;

        // Odblokowanie AI Bossa
        if (currentBoss != null && currentBoss.GetComponent<KingBossAI>() != null)
            currentBoss.GetComponent<KingBossAI>().enabled = true;
    }
}