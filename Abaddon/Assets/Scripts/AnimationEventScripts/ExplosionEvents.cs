using UnityEngine;

public class ExplosionEvents : MonoBehaviour
{
    public void TriggerDrop()
    {
        GetComponent<ItemDropper>().DropRandomItem();
    }
}
