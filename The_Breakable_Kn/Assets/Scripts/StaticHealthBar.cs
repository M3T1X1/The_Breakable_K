using UnityEngine;

public class StaticHealthBar : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        Vector3 parentScale = transform.parent.localScale;

        transform.localScale = new Vector3(
            initialScale.x / Mathf.Sign(parentScale.x),
            initialScale.y,
            initialScale.z
        );
    }
}