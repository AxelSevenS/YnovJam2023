using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

using SevenGame.Utility;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public abstract class Character : NetworkBehaviour {

    protected CapsuleCollider characterCollider;
    protected Rigidbody _rigidbody;



    public static LayerMask groundMask = 1;

    [SerializeField] protected bool _isGrounded = false;
    protected RaycastHit groundHit;

    public abstract float movementSpeed { get; }

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
        characterCollider = gameObject.GetComponent<CapsuleCollider>();
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    protected virtual void Update() {

        _isGrounded = characterCollider.ColliderCast(characterCollider.transform.position, Vector3.down * 5f, out groundHit, 0.15f, groundMask);

        Debug.Log("Grounded: " + _isGrounded);

        CharacterMovement();

    }

    protected virtual void FixedUpdate() {

    }

    protected abstract void CharacterMovement();
}