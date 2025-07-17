using UnityEngine;

public class Spear : Weapon
{
    [SerializeField]
    public int baseDamage = 4;
    public static new Vector2 baseSize = new Vector2(1f, 2f); // one wide, two long
    public override string AnimationName => "Spear";

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override int GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}
