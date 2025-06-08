using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CanBeInteractedWith
{
    void Interact();
}

public interface HasHealth : CanBeHurt
{
    void Heal(uint amount);
}

public interface CanBeHurt
{
    bool Hurt(uint damage);
    void Die();
}

public interface CanFight
{
    void Attack();
    uint Heal(uint amount);
    void Hurt(uint damage);
    void Die();
}

public interface CanMove
{
    void Move(Vector2 direction);
}