using UnityEngine;

public class Spear : Weapon
{
    public static new uint baseDamage = 2;
    public static new Vector2 baseSize = new Vector2(1f, 2f); // one wide, two long

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override uint GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}