using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CanBeInteractedWith
{
    void Interact();
}

public interface CanBeHealed
{
    // the uint returned is the healing overflow (e.g. max health is 100, current health is 90, heal for 20, return 10)
    int Heal(int amount);
}

public interface CanBeDamaged
{
    // the uint returned is the healing overflow (e.g. max health is 100, current health is 90, heal for 20, return 10)
    int Hurt(int damage);
}

public interface CanBeKilled : CanBeDamaged, CanBeHealed
{
    void Die();
}

public enum EnemyType
{
    Gnome,
    Boss1,
    Statue
}

public interface CanFight : CanBeKilled
{
    EnemyType GetEnemyType();
    void Attack();
}

public interface CanMove
{
    void Move(Vector2 direction);
}