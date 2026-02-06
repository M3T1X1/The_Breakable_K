using UnityEngine;

public class ItemPersistence : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.collectedItems.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    public void MarkAsCollected()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.collectedItems.Add(gameObject.name);
        }
    }
}