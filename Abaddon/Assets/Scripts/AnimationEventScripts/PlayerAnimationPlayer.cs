using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationPlayer : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    static Vector2 DirToVec(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            _ => Vector2.zero,
        };
    }

    [SerializeField]
    Animator animator;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void PlayIdleAnimation(Direction direction)
    {
        Controller.main.PlayAnimation("Idle", DirToVec(direction));
    }

    public void AttackAnimationFinishHandler()
    {
        if (Controller.main == null)
            return;

        Controller.main.AttackAnimationFinishHandler();
    }
}
