using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Breakable : MonoBehaviour, Interactable
{
    [SerializeField] LootTable lootTable;
    [SerializeField] BetterAnimator animationEventHandler;

    private BreakableSfx sfxPlayer;

    public void Start() {
        sfxPlayer = GetComponent<BreakableSfx>();
        lootTable?.RunTests(1000);
    }

    public void Interact()
    {
        sfxPlayer?.PlayBreakSound();
        
        if (lootTable is not null) {
            Instantiate(lootTable.PickItem(), transform.position, Quaternion.identity);
        } 
        animationEventHandler?.QueueAnimation(new BetterAnimation(
            animation_name: "Explode",
            priority: 10,
            shouldLoop: false,
            persistUntilPlayed: true,
            onAnimationEnd: () => Destroy(gameObject)
        ));
    }
}