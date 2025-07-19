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
        Right
    }

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayIdleAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                animator.Play("Player_animation_back_level_0_idle");
                break;
            case Direction.Down:
                animator.Play("Player_animation_front_level_0_idle");
                break;
            case Direction.Left:
                animator.Play("Player_animation_left_level_0_idle");
                break;
            case Direction.Right:
                animator.Play("Player_animation_right_level_0_idle");
                break;
        }
    }

    public void AttackAnimationFinishHandler()
    {
        Controller.main.AttackAnimationFinishHandler();
    }
}
