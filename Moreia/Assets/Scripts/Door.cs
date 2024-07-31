using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] public bool NeedsKey;
    [SerializeField] Direction direction = Direction.Left;
    [SerializeField] Inventory inventory;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (direction == Direction.Up) {
            spriteRenderer.sprite = Resources.Load<Sprite>($"tilemaps/wall_tilemap/real_wall_tilemap/DoorUp");
        } else {
            spriteRenderer.sprite = Resources.Load<Sprite>($"tilemaps/wall_tilemap/real_wall_tilemap/Door{direction.ToString()}{Controller.main.rnd.Next(1, 4)}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inventory.CheckIfItemExists(1)) {
            // we set the image to the unlocked door
            spriteRenderer.sprite = Resources.Load<Sprite>($"tilemaps/wall_tilemap/real_wall_tilemap/Door{direction.ToString()}");
        }
    }
}
