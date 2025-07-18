using UnityEngine;

public class ExplosionEvents : MonoBehaviour
{
    public void TriggerDrop()
    {
        print("Triggering item drop");
        var itemDropper = GetComponent<ItemDropper>();
        if (itemDropper != null)
        {
            itemDropper.DropRandomItem();
        }
        else
        {
            Debug.LogWarning("ItemDropper component not found on explosion object.");
        }
    }
}
