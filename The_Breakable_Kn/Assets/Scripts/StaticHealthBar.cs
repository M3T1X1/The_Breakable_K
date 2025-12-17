using UnityEngine;

public class StaticHealthBar : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        // Zapamiêtujemy pocz¹tkow¹ skalê paska (t¹ bardzo ma³¹, np. 0.005)
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        // Wymuszamy, aby skala paska by³a zawsze taka sama, 
        // niezale¿nie od tego, czy rodzic (Mag) jest obrócony (-1) czy nie (1)
        Vector3 parentScale = transform.parent.localScale;

        transform.localScale = new Vector3(
            initialScale.x / Mathf.Sign(parentScale.x),
            initialScale.y,
            initialScale.z
        );
    }
}