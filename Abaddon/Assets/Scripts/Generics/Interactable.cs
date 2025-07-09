using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CanBeInteractedWith
{
    void Interact();
}

public interface HasHealth
{
    void Heal(uint amount);
}

public enum EnemyType
{
    Gnome,
    Boss1,
    Statue,
}

public interface CanFight
{
    EnemyType GetEnemyType();
    void Attack();
    // the uint returned is the healing overflow (e.g. max health is 100, current health is 90, heal for 20, return 10)
    uint Heal(uint amount);
    void Hurt(uint damage);
    void Die();
}

public interface CanMove
{
    void Move(Vector2 direction);
}