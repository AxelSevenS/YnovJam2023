using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

public class Player : Character {


    public static List<Player> players = new List<Player>();


    [SerializeField] protected CameraController cameraController;

    private Vector3 _totalMovement = Vector3.zero;

    public Vector3 jumpDirection;

    private bool sprinting = false;


    public override float movementSpeed {
        get {
            return sprinting ? 6f : 3f;
        }
    }



    private void OnEnable() {
        players.Add(this);
    }

    private void OnDisable() {
        players.Remove(this);
    }



    protected override void CharacterMovement() {

        PlayerMovement();


        bool jumping = Input.GetKeyDown(KeyCode.Space);
        
        if (jumping && _isGrounded){
            _rigidbody.AddForce(jumpDirection, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    private void PlayerMovement() {

        
        bool CanMove = true;
        if (!CanMove){
            return;
        }
            

        bool forward = Input.GetKey(KeyCode.Z);
        bool back = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);
        bool left = Input.GetKey(KeyCode.Q);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        float movement = forward ? 1f : back ? -1f : 0f;
        float strafe = right ? 1f : left ? -1f : 0f;


        Vector3 relativeDirection = new Vector3(strafe, 0f, movement);

        // if (relativeDirection.sqrMagnitude == 0f) 
        //     return;


        Vector3 absoluteDirection = cameraController.camera.transform.rotation * relativeDirection;

        if ( isGrounded ) {
            Vector3 rightOfDirection = Vector3.Cross(absoluteDirection, groundHit.normal).normalized;
            Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

            absoluteDirection = directionConstrainedToGround * absoluteDirection.magnitude;
        }

        _totalMovement = absoluteDirection;
    }

    private void FixedUpdate() {

        Vector3 movement = _totalMovement * movementSpeed * Time.fixedDeltaTime;
        
        bool walkCollision = characterCollider.ColliderCast(characterCollider.transform.position, movement, out RaycastHit walkHit, 0.15f, groundMask);

        Vector3 executedMovement = walkCollision ? movement.normalized * walkHit.distance : movement;
        _rigidbody.MovePosition(_rigidbody.position + executedMovement);


        // Check for penetration and adjust accordingly
        foreach ( Collider worldCollider in characterCollider.ColliderOverlap(Vector3.zero, 0f, groundMask) ) {
            if ( Physics.ComputePenetration(characterCollider, characterCollider.transform.position, characterCollider.transform.rotation, worldCollider, worldCollider.transform.position, worldCollider.transform.rotation, out Vector3 direction, out float distance) ) {
                _rigidbody.MovePosition(_rigidbody.position + (direction * distance));
            }
        }

        _totalMovement = Vector3.zero;
    }


    protected override void Die() {
        // throw new System.NotImplementedException();
    }
}
