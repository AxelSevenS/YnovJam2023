using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SevenGame.Utility;

public class Player : Character {


    public static List<Player> players = new List<Player>();



    // [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject uiPrefab;

    private Vector3 _totalMovement = Vector3.zero;
    private Vector3 relativeDirection = Vector3.zero;
    private Quaternion cameraRotation = Quaternion.identity;

    protected const float maxStamina = 10f;
    [SerializeField] private float _stamina = maxStamina;
    private bool tiredOut = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tiredClip;

    // public Vector3 jumpDirection;

    private bool sprinting = false;


    public override float health {
        get {
            return base.health;
        }
        set {
            base.health = value;
            if (healthImage != null)
                healthImage.fillAmount = health / maxHealth;
        }
    }

    public virtual float stamina {
        get {
            return _stamina;
        }
        set {
            _stamina = value;
            if (staminaImage != null)
                staminaImage.fillAmount = stamina / maxStamina;
        }
    }


    public float hearingDistance {
        get {
            if (flashlight.charging)
                return 200f;

            if (sprinting)
                return 80f;

            return 30f;
        }
    }


    [SerializeField] protected Image healthImage;
    [SerializeField] protected Image staminaImage;
    public CameraController cameraController;

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
            GameObject cameraObject = Camera.main.gameObject;
            cameraController = cameraObject.AddComponent<CameraController>();
            cameraController.SetTarget(this);

            GameObject uiObject = GameObject.Instantiate(uiPrefab);
            healthImage = uiObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            staminaImage = uiObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        }

        if (IsServer) {
            characterCollider.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
        } else {
            characterCollider.transform.position = players[0].transform.position;
        }
    }



    protected override void CharacterMovement() {

        if (!enabled)
            return;

        PlayerControls();

        PlayerMovement();
    }


    protected override void Die() {
        this.enabled = false;
    }

    private void PlayerMovement() {

        Quaternion forwardRotation = relativeDirection.sqrMagnitude != 0f ? Quaternion.LookRotation(relativeDirection) : Quaternion.identity;
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation * forwardRotation, 5f * Time.deltaTime);

        if (relativeDirection.sqrMagnitude == 0)
            return;


        Vector3 movementDirection = cameraRotation * relativeDirection;

        if (isGrounded) {
            Vector3 rightOfDirection = Vector3.Cross(movementDirection, groundHit.normal).normalized;
            Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

            movementDirection = directionConstrainedToGround * movementDirection.magnitude;
        }

        _totalMovement = movementDirection;
    }

    private void PlayerControls() {
        if (!IsOwner)
            return;


        flashlight.PointFlashLight(cameraController.camera.transform.rotation);
        flashlight.chargeInput = Input.GetMouseButton(1);
        flashlight.toggleInput = Input.GetMouseButtonDown(0);
        flashlight.flashInput = Input.GetKeyDown(KeyCode.F);
        flashlight.originPosition = characterCollider.transform.position;


        bool forward = Input.GetKey(KeyCode.Z);
        bool back = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);
        bool left = Input.GetKey(KeyCode.Q);
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);

        float movement = forward ? 1f : back ? -1f : 0f;
        float strafe = right ? 1f : left ? -1f : 0f;

        sprinting = sprintInput && !tiredOut && !flashlight.chargeInput;
        stamina = Mathf.MoveTowards(stamina, sprinting ? 0f : maxStamina, Time.deltaTime);
        if (stamina == 0 && !tiredOut) {
            tiredOut = true;
            audioSource.PlayOneShot(tiredClip);
        } else if (stamina == maxStamina && tiredOut) {
            tiredOut = false;
        }

        relativeDirection = new Vector3(strafe, 0f, movement).normalized;Vector3 groundUp = Vector3.up;

        Vector3 groundForward = Vector3.Cross(cameraController.camera.transform.right, groundUp);
        cameraRotation = Quaternion.LookRotation(groundForward, groundUp);
    }

    


    private void OnEnable() {
        players.Add(this);
    }

    private void OnDisable() {
        players.Remove(this);
        if (players.Count < 1) {
            Debug.Log("Game Over");
            Application.Quit();
        }
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
