using UnityEngine;

public class EyeScriptEvent : MonoBehaviour
{
    [SerializeField]
    WeepingEyeAttack weepingEyeAttack;

    public void CreateBeam(DetectAnimationEvent.Direction direction)
    {
        Vector2 dir = DetectAnimationEvent.GetDirectionVector(direction);
        weepingEyeAttack.CreateBeam(weepingEyeAttack.transform.position, dir);
    }
}
