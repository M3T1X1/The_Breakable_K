using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;
    public GameObject playerHUD;       // Ca³y obiekt PlayerHUD
    public TextMeshProUGUI statusText; // Obiekt GameOverText
    public GameObject restartBtn;      // Przycisk RestartButton

    void Awake()
    {
        // To sprawi, ¿e manager bêdzie dostêpny na ka¿dej scenie
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void TriggerVictory()
    {
        Invoke("ShowVictoryUI", 2f); // Pokazuje UI po 2 sekundach od œmierci bosa
    }

    void ShowVictoryUI()
    {
        // Szukamy HUDa na aktualnej scenie (bo po zmianie sceny stare referencje znikaj¹)
        GameObject hud = GameObject.Find("PlayerHUD");
        if (hud != null)
        {
            hud.SetActive(true);
            // Szukamy tekstu wewn¹trz HUDa i go zmieniamy
            TextMeshProUGUI txt = hud.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = "ZAMEK WYZWOLONY!";
                txt.color = Color.yellow;
            }
            // Szukamy przycisku
            Transform btn = hud.transform.Find("RestartButton");
            if (btn != null) btn.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0); // Wraca do pocz¹tku (Scena z Magiem)
    }
}