using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

using SevenGame.Utility;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]

public abstract class Character : NetworkBehaviour {

    protected CapsuleCollider characterCollider;
    protected Rigidbody _rigidbody;
    protected Animation animator;



    public static LayerMask groundMask = 1;

    private float _health = 100f;
    [SerializeField] private Image _healthImage;

    [SerializeField] protected bool _isGrounded = false;
    protected RaycastHit groundHit;

    public abstract float movementSpeed { get; }
    


    public float health {
        get { return _health; }
        set {
            _health = value;
            if(_health <= 0f) {
                Die();
            }
        }
    }

    public bool isGrounded {
        get { return _isGrounded; }
    }

    public RaycastHit ground {
        get { return groundHit; }
    }

    protected virtual void Start() {
        
        // groundMask = LayerMask.GetMask("Default");
    }

    protected virtual void Awake() {
        animator = gameObject.GetComponent<Animation>();
        characterCollider = gameObject.GetComponent<CapsuleCollider>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        health = 100f;
    }

    protected virtual void Update() {

        _isGrounded = characterCollider.ColliderCast(characterCollider.transform.position, Vector3.down * 5f, out groundHit, 0.15f, groundMask);

        CharacterMovement();

    }

    protected virtual void FixedUpdate() {

    }
    private void UpdateBarLife(){
        _healthImage.fillAmount = health;
    }

    protected abstract void CharacterMovement();

    protected abstract void Die();
}