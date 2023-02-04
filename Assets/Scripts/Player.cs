using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

public class Player : Character {


    public static List<Player> players = new List<Player>();

    private Vector3 _totalMovement = Vector3.zero;

    public Vector3 jumpDirection;

    private bool sprinting = false;


    [SerializeField] protected CameraController cameraController;

    [SerializeField] private Flashlight flashLight;



    public override float movementSpeed {
        get {
            return sprinting ? 6f : 3f;
        }
    }



    protected override void CharacterMovement() {

        flashLight.PointFlashLight(cameraController.camera.transform.rotation);
        flashLight.chargeInput = Input.GetKey(KeyCode.C);
        flashLight.toggleInput = Input.GetKeyDown(KeyCode.L);
        flashLight.flashInput = Input.GetKeyDown(KeyCode.F);
        flashLight.originPosition = characterCollider.transform.position;

        if (flashLight.charging || !enabled)
            return;

        PlayerMovement();

        bool jumping = Input.GetKeyDown(KeyCode.Space);
        
        if (jumping && _isGrounded){
            _rigidbody.AddForce(jumpDirection, ForceMode.Impulse);
        }
    }


    protected override void Die() {
        this.enabled = false;
    }

    private void PlayerMovement() {
        bool forward = Input.GetKey(KeyCode.Z);
        bool back = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);
        bool left = Input.GetKey(KeyCode.Q);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        float movement = forward ? 1f : back ? -0.75f : 0f;
        float strafe = right ? 1f : left ? -1f : 0f;


        Vector3 relativeDirection = new Vector3(strafe, 0f, movement);

        Vector3 groundUp = Vector3.up;
        Vector3 groundForward = Vector3.Cross(cameraController.camera.transform.right, groundUp);
        Quaternion forwardRotation = relativeDirection.sqrMagnitude != 0f ? Quaternion.LookRotation(relativeDirection) : Quaternion.identity;

        Quaternion cameraRotation = Quaternion.LookRotation(groundForward, groundUp);

        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation * forwardRotation, 5f * Time.deltaTime);

        if (relativeDirection.sqrMagnitude == 0)
            return;


        Vector3 movementDirection = cameraRotation * relativeDirection;

        if ( isGrounded ) {
            Vector3 rightOfDirection = Vector3.Cross(movementDirection, groundHit.normal).normalized;
            Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

            movementDirection = directionConstrainedToGround * movementDirection.magnitude;
        }

        _totalMovement = movementDirection;
    }

    private void Flashlight() {

        

    }

    


    private void OnEnable() {
        players.Add(this);
    }

    private void OnDisable() {
        players.Remove(this);
    }

    protected override void Start() {
        base.Start();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

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

    
}
