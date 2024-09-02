using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.VisualScripting;


public abstract class Enemy : MonoBehaviour, Damageable, Interactable, Attackable, Moveable {

    [Header("References")]
    [SerializeField] protected LayerMask collideLayers;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Pathfinding2D pathfinding;
    [SerializeField] protected string animation_prefix = "Goblin";
    [SerializeField] protected List<Attack> attacks;
    [SerializeField] protected Attack currentAttack;
    [SerializeField] protected SfxPlayer walkingSfxPlayer;
    [SerializeField] protected SfxPlayer hurtSfxPlayer;
    [SerializeField] protected ItemDropper itemDropper;

    [Header("Attributes")]
    [SerializeField] protected float detectionDistance = 1;
    [SerializeField] protected float followDistance = 3f;


    public uint health = 10;
    protected Vector2 direction = Vector2.zero;
    protected Vector3 StartPosition;
    protected EnemySfx sfxPlayer;

    [SerializeField] protected GameObject textFadePrefab;


    public static List<Enemy> enemies = new List<Enemy>();


    public abstract void Hurt(uint damage, bool canDodge);
    public abstract void Attack(uint damage);

    public abstract void Interact();

    public abstract void Attack();

    public abstract void Die();

    public abstract void Move();

    public abstract void MakeDecision();
}