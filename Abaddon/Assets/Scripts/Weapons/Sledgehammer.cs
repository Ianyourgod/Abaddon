using UnityEngine;

public class Sledgehammer : Weapon
{
    [SerializeField]
    public int baseDamage = 5;
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
