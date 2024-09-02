using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventMessenger : MonoBehaviour {
    [SerializeField] EnemyMovement enemyMovement;

    public void AttackAnimationEvent() {
        enemyMovement.AttackAnimationEvent();
    }
}
