using TreeEditor;
using UnityEngine;

public class GoldItem : MonoBehaviour
{
    [SerializeField]
    int gold_count = 1;

    [SerializeField]
    Sprite small_image;

    [SerializeField]
    Sprite mid_image;

    [SerializeField]
    Sprite big_image;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    public void SetGoldCount(int count)
    {
        gold_count = count;
        if (gold_count <= 3)
        {
            spriteRenderer.sprite = small_image;
        }
        else if (gold_count <= 6)
        {
            spriteRenderer.sprite = mid_image;
        }
        else
        {
            spriteRenderer.sprite = big_image;
        }
    }

    public int GetGoldCount()
    {
        return gold_count;
    }

    private void Start()
    {
        SetGoldCount(gold_count);
        Controller.OnMoved += CheckIfGoldShouldBeCollected;
    }

    void CheckIfGoldShouldBeCollected()
    {
        if (WithinBox(Controller.main.transform.position, transform.position, 0.5f))
            CollectGold();
    }

    void CollectGold()
    {
        Controller.OnMoved -= CheckIfGoldShouldBeCollected;
        Controller.main.goldCount += gold_count;
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
