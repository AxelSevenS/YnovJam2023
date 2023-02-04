using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

public class Player : Character {


    public static List<Player> players = new List<Player>();



    [SerializeField] private GameObject cameraPrefab;

    private Vector3 _totalMovement = Vector3.zero;
    public float maxEndurance= 10;
    [SerializeField] private float endurance= 10;

    public Vector3 jumpDirection;

    private bool sprinting = false;


    public float hearingDistance {
        get {
            if (flashlight.charging)
                return 40f;

            if (sprinting)
                return 15f;

            return 0;
        }
    }


    [SerializeField] protected CameraController cameraController;

    [SerializeField] private Flashlight flashlight;



    public override float movementSpeed {
        get {
            if (flashlight.charging)
                return 1f;

            return sprinting ? 6f : 3f;
        }
    }

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            GameObject cameraObject = GameObject.Instantiate(cameraPrefab);
            cameraController = cameraObject.GetComponent<CameraController>();
            cameraController.SetTarget(this);
        }
        // if (!IsOwner) {
        //     Destroy(flashlight);
        //     // Destroy(cameraController.gameObject);
        //     // Destroy(this);
        // }
    }



    protected override void CharacterMovement() {

        if (!IsOwner || !enabled)
            return;

        flashlight.PointFlashLight(cameraController.camera.transform.rotation);
        flashlight.chargeInput = Input.GetMouseButton(1);
        flashlight.toggleInput = Input.GetMouseButtonDown(0);
        flashlight.flashInput = Input.GetKeyDown(KeyCode.F);
        flashlight.originPosition = characterCollider.transform.position;


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

        // if (Input.GetKey(KeyCode.LeftShift)&& endurance >0){
        //     // while(endurance>0){
        //     //     endurance-=1;
        //     //     endurance= Mathf.MoveTowards(battery, 0, Time.deltaTime);
        //     // }
        // }
        //if (!Input.GetKey(KeyCode.LeftShift)){
            // while(endurance<maxEndurence){
            //      endurance+=1;
            //      endurance= Mathf.MoveTowards(battery, 0, Time.deltaTime);
            //}
            //}


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
