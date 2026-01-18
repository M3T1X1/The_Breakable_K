using UnityEngine;

public class ItemPersistence : MonoBehaviour
{
    void Start()
    {
        // Sprawdzamy, czy nazwa TEGO obiektu jest ju¿ na liœcie w GameManagerze
        if (GameManager.instance != null && GameManager.instance.collectedItems.Contains(gameObject.name))
        {
            // Jeœli tak, to znaczy, ¿e ju¿ go zebraliœmy wczeœniej - usuwamy go ze sceny
            Destroy(gameObject);
        }
    }

    // Tê funkcjê wywo³amy w momencie podnoszenia przedmiotu
    public void MarkAsCollected()
    {
        if (GameManager.instance != null)
        {
            // Dodajemy nazwê tego obiektu do listy zebranych rzeczy
            GameManager.instance.collectedItems.Add(gameObject.name);
        }
    }
}