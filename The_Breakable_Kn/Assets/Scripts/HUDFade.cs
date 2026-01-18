using UnityEngine;

public class HUDFade : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private GameObject player;

    [Header("Ustawienia")]
    public float fadeAlpha = 0.25f; // Jak bardzo przezroczysty (0.25 = 25%)
    public float fadeSpeed = 5f;

    [Header("Obszar znikania (dopasuj w grze)")]
    public float minX = -8f; // Lewa krawêdŸ
    public float maxX = -4f; // Prawa krawêdŸ obszaru pod barem
    public float minY = 2f;  // Wysokoœæ, od której bar znika

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // Szukamy gracza po tagu "Player"
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        // Sprawdzamy czy pozycja rycerza mieœci siê w "strefie paska"
        bool isBehind = player.transform.position.x > minX &&
                        player.transform.position.x < maxX &&
                        player.transform.position.y > minY;

        float targetAlpha = isBehind ? fadeAlpha : 1f;

        // P³ynne przejœcie przezroczystoœci
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}