using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour {
    [SerializeField] uint damage = 1;

    public enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    public void Attack(Direction direction) {
        Controller.main.DamagePlayer(damage);
    }
}