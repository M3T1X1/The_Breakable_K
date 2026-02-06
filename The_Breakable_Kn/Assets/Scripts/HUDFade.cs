using UnityEngine;

public class HUDFade : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private GameObject player;

    [Header("Ustawienia")]
    public float fadeAlpha = 0.25f; 
    public float fadeSpeed = 5f;

    [Header("Obszar znikania (dopasuj w grze)")]
    public float minX = -8f; 
    public float maxX = -4f; 
    public float minY = 2f;  

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        bool isBehind = player.transform.position.x > minX &&
                        player.transform.position.x < maxX &&
                        player.transform.position.y > minY;

        float targetAlpha = isBehind ? fadeAlpha : 1f;

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}