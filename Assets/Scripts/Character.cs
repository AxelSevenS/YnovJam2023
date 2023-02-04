using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

public abstract class Character : MonoBehaviour {

    protected CapsuleCollider characterCollider;
    protected Rigidbody _rigidbody;
    protected Animation animator;



    public static LayerMask groundMask;

    private float _health = 100f;

    protected bool _isGrounded = false;
    protected RaycastHit groundHit;

    public abstract float movementSpeed { get; }
    // public abstract float runSpeed;
    // public abstract float turnSpeed;
    


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

    private void Awake() {
        animator = gameObject.GetComponent<Animation>();
        characterCollider = gameObject.GetComponent<CapsuleCollider>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();

        groundMask = LayerMask.GetMask("default");
        health = 100f;
    }

    private void Update() {

        _isGrounded = characterCollider.ColliderCast(Vector3.zero, Vector3.down * 0.05f, out groundHit, 0.15f, groundMask);

        CharacterMovement();

    }

    protected abstract void CharacterMovement();

    protected abstract void Die();
}