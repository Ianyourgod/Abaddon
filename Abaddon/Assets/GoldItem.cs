using TreeEditor;
using UnityEngine;

public class GoldItem : MonoBehaviour
{
    private void Start()
    {
        Controller.OnMoved += CheckIfGoldShouldBeCollected;
    }

    void CheckIfGoldShouldBeCollected()
    {
        if (
            Vector2.Distance(transform.position, Controller.main.transform.position)
            < GetComponent<CircleCollider2D>().radius
        )
            CollectGold();
    }

    void CollectGold()
    {
        Controller.OnTick -= CheckIfGoldShouldBeCollected;
        Controller.main.goldCount++;
        //TODO: Add a sound effect for collecting gold
        Destroy(gameObject);
    }

    bool WithinBox(Vector2 point, Vector2 boxCenter, float halfSize)
    {
        return point.x >= boxCenter.x - halfSize
            && point.x <= boxCenter.x + halfSize
            && point.y >= boxCenter.y - halfSize
            && point.y <= boxCenter.y + halfSize;
    }
}
