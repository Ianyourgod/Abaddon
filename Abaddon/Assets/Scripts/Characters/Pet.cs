using UnityEngine;

public class Pet : MonoBehaviour
{
    [HideInInspector]
    public static Pet main;

    [SerializeField]
    public SpriteRenderer spriteRenderer;
    public Item petItem;
    public static string petString = "Pet";

    [HideInInspector]
    private Vector2 targetPosition;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        main.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Update()
    {
        if (Controller.main == null || petItem == null)
            return;

        if (Vector2.Distance(Controller.main.transform.position, transform.position) > 0.25f)
        {
            targetPosition = new Vector2(
                Controller.main.transform.position.x,
                Controller.main.transform.position.y
            );
            Vector2 vec = transform.position - Controller.main.transform.position;
            vec.Normalize();
            targetPosition += vec * 0.75f;
            transform.position = Vector2.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * 5f
            );
        }
    }

    public void setPetItem(Item item)
    {
        petItem = item;
        if (petItem == null)
        {
            // clear pet
            spriteRenderer.sprite = null;
            return;
        }
        else
        {
            spriteRenderer.sprite = petItem.itemSprite;
            return;
        }
    }
}
