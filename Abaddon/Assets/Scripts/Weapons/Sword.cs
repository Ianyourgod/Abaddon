using UnityEngine;

public class Sword : Weapon
{
    [SerializeField]
    public int baseDamage = 3;
    public override string AnimationName => "Sword";
    public static new Vector2 baseSize = new Vector2(1f, 1f); // one wide, one long

    public override Vector2 GetSize()
    {
        return baseSize;
    }

    public override int GetDamage()
    {
        return baseDamage + Controller.main.GetDamageModifier();
    }
}
