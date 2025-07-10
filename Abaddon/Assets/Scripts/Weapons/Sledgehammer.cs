using UnityEngine;

public class Sledgehammer : Weapon
{
    public static new uint baseDamage = 2;
    public static new Vector2 baseSize = new Vector2(1f, 3f); // one long, three wide

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override uint GetDamage()
    {
        if (Controller.main == null)
            return baseDamage;

        return baseDamage + Controller.main.GetDamageModifier();
    }
}
