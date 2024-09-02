using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    public void Hurt(uint damage, bool dodgeable = false);
}