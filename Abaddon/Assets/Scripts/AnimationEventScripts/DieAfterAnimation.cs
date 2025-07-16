using UnityEngine;

public class DieAfterAnimation : MonoBehaviour
{
    public void Die()
    {
        Destroy(gameObject);
    }

    public void KillParent()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Debug.LogWarning("DieAfterAnimation: No parent to destroy.");
        }
    }
}
