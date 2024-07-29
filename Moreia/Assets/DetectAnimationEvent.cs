using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAnimationEvent : MonoBehaviour
{
    [SerializeField] EnemyMovement enemyMovement;

    // this is called by the animation event
    public void AttackTiming(int direction) {
        enemyMovement.AttackTiming((BaseAttack.Direction)direction);
    }
}
