using UnityEngine;

public class ExplosionEvents : MonoBehaviour
{
    public void TriggerDrop()
    {
        print("Triggering item drop");
        var itemDropper = GetComponent<ItemDropper>();
        itemDropper.DropRandomItem();
    }
}
