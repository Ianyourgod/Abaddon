using UnityEngine;

public class Sledgehammer : Weapon
{
    public static new int baseDamage = 2;
    public override string AnimationName => "Hammer";
    public static new Vector2 baseSize = new Vector2(3f, 1f); // three wide, one long

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override int GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}
