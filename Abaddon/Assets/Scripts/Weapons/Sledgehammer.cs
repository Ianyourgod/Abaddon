using UnityEngine;

public class Sledgehammer : Weapon
{
    public static new uint baseDamage = 2;
    public static new Vector2 baseSize = new Vector2(3f, 1f); // three wide, one long

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override uint GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}