using UnityEngine;

public class GhostOneWay : MonoBehaviour
{
    [Tooltip("Trigger für den Käfig-Ausgang (isTrigger = true)")]
    public Collider exitTrigger;

    [Tooltip("Blocker-Collider außerhalb (isTrigger = false)")]
    public Collider exitBlocker;

    private Collider ghostCollider;

    void Awake()
    {
        ghostCollider = GetComponent<Collider>();
        if (ghostCollider == null)
            Debug.LogError("GhostOneWay benötigt einen Collider am Geister-Objekt!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == exitTrigger)
        {
            // Ab jetzt ignorieren wir Kollision mit dem äußeren Blocker
            Physics.IgnoreCollision(ghostCollider, exitBlocker, true);
        }
    }
}