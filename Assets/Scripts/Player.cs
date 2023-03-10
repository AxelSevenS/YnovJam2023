using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

using SevenGame.Utility;


[RequireComponent(typeof(Animator))]
public class Player : Character {

    protected Animator animator;

    public static List<Player> players = new List<Player>();
    public static List<Player> playersWon {
        get {
            List<Player> playersWon = new List<Player>();
            foreach (Player player in players) {
                if (player.won) {
                    playersWon.Add(player);
                }
            }
            return playersWon;
        }
    }



    // [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private GameObject uiPrefab;

    private NetworkVariable<Vector3> netPos = new(writePerm : NetworkVariableWritePermission.Server);
    private NetworkVariable<Quaternion> netRot = new(writePerm : NetworkVariableWritePermission.Server);


    private NetworkVariable<Vector3> _totalMovement = new(value: Vector3.zero);

    private NetworkVariable<Vector3> relativeDirection = new(value: Vector3.zero, writePerm : NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> cameraRotation = new(value: Quaternion.identity, writePerm : NetworkVariableWritePermission.Owner);
    
    private NetworkVariable<bool> sprinting = new(value: false, writePerm : NetworkVariableWritePermission.Owner);


    protected const float maxStamina = 10f;
    [SerializeField] private float _stamina = maxStamina;
    private bool tiredOut = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tiredClip;

    public bool won = false;

    // public Vector3 jumpDirection;



    [SerializeField] private int _health = maxHealth;
    public const int maxHealth = 2;

    public int health {
        get {
            return _health;
        }
        set {
            Debug.Log("Health: " + value);
            _health = value;
            if(_health <= 0) {
                StartCoroutine(Die());
            }
            if (healthImage != null)
                healthImage.fillAmount = (float)health / (float)maxHealth;
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
            if (flashlight.charging.Value)
                return 60f;

            if (sprinting.Value)
                return 40f;

            return 0f;
        }
    }


    [SerializeField] protected Image healthImage;
    [SerializeField] protected Image staminaImage;
    [SerializeField] protected Image batteryImage;
    [SerializeField] protected Image flashImage;
    public CameraController cameraController;

    [SerializeField] private Flashlight flashlight;



    public override float movementSpeed {
        get {
            if (flashlight.charging.Value)
                return 1f;

            return sprinting.Value ? 6f : 3f;
        }
    }

    public void JumpScare(GameObject jumpScareObject) {
        if (IsOwner) {
            health -= 1;
            GameObject.Instantiate(jumpScareObject, cameraController.transform);
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
            batteryImage = uiObject.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>();
            flashImage = uiObject.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Image>();
        }

        // if (players.Count > 0) {
            characterCollider.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
        // } else {
        //     characterCollider.transform.position = players[0].transform.position;
        // }
    }



    protected override void CharacterMovement() {

        if (!enabled)
            return;

        PlayerControls();

        PlayerMovement();
    }


    protected virtual IEnumerator Die() {
        this.enabled = false;
        yield return new WaitForSeconds(5f);

        if (players.Count < 1 && playersWon.Count < 1) {
            Debug.Log("Game Over");
            Application.Quit();
        }

    }

    private void PlayerMovement() {

        if (IsServer) {
            Quaternion forwardRotation = relativeDirection.Value.sqrMagnitude != 0f ? Quaternion.LookRotation(relativeDirection.Value) : Quaternion.identity;
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraRotation.Value * forwardRotation, 5f * Time.deltaTime);

            netRot.Value = transform.rotation;
        } else {
            transform.rotation = Quaternion.Slerp(transform.rotation, netRot.Value, 25f * Time.fixedDeltaTime);
        }


        if (!IsServer)
            return;

        _totalMovement.Value = Vector3.zero;

        if (relativeDirection.Value.sqrMagnitude == 0)
            return;


        Vector3 movementDirection = cameraRotation.Value * relativeDirection.Value.normalized;

        if (isGrounded) {
            Vector3 rightOfDirection = Vector3.Cross(movementDirection, groundHit.normal).normalized;
            Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

            movementDirection = directionConstrainedToGround * movementDirection.magnitude;
        }

        _totalMovement.Value = movementDirection;
    }

    private void PlayerControls() {
        if (!IsOwner)
            return;


        flashlight.forwardRotation.Value = cameraController.camera.transform.rotation;
        flashlight.chargeInput.Value = Input.GetMouseButton(1);
        flashlight.toggleInput.Value = Input.GetMouseButtonDown(0);
        flashlight.flashInput.Value = Input.GetKeyDown(KeyCode.F);
        flashlight.originPosition = characterCollider.transform.position;


        flashImage.fillAmount = (float)flashlight.flashCount.Value / (float)Flashlight.maxFlashCount;
        batteryImage.fillAmount = flashlight.battery.Value / Flashlight.maxBattery;


        bool forward = Input.GetKey(KeyCode.Z);
        bool back = Input.GetKey(KeyCode.S);
        bool right = Input.GetKey(KeyCode.D);
        bool left = Input.GetKey(KeyCode.Q);
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);

        float movement = forward ? 1f : back ? -1f : 0f;
        float strafe = right ? 1f : left ? -1f : 0f;

        sprinting.Value = sprintInput && !tiredOut && !flashlight.chargeInput.Value;
        stamina = Mathf.MoveTowards(stamina, sprinting.Value ? 0f : maxStamina, Time.deltaTime);
        if (stamina == 0 && !tiredOut) {
            tiredOut = true;
            audioSource.PlayOneShot(tiredClip);
        } else if (stamina == maxStamina && tiredOut) {
            tiredOut = false;
        }

        relativeDirection.Value = new Vector3(strafe, 0f, movement).normalized;

        Vector3 groundUp = Vector3.up;
        Vector3 groundForward = Vector3.Cross(cameraController.camera.transform.right, groundUp);

        cameraRotation.Value = Quaternion.LookRotation(groundForward, groundUp);
    }


    private void OnEnable() {
        players.Add(this);
    }

    private void OnDisable() {
        players.Remove(this);
    }

    protected override void Awake() {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        animator = GetComponent<Animator>();
    }

    protected override void Start() {
        base.Start();
        health = 3;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (IsServer) {

            Vector3 movement = _totalMovement.Value * movementSpeed * Time.fixedDeltaTime;
            
            bool walkCollision = characterCollider.ColliderCast(characterCollider.transform.position, movement, out RaycastHit walkHit, 0.15f, groundMask);

            Vector3 executedMovement = walkCollision ? movement.normalized * walkHit.distance : movement;
            _rigidbody.MovePosition(_rigidbody.position + executedMovement);


            // Check for penetration and adjust accordingly
            foreach ( Collider worldCollider in characterCollider.ColliderOverlap(Vector3.zero, 0f, groundMask) ) {
                if ( Physics.ComputePenetration(characterCollider, characterCollider.transform.position, characterCollider.transform.rotation, worldCollider, worldCollider.transform.position, worldCollider.transform.rotation, out Vector3 direction, out float distance) ) {
                    _rigidbody.MovePosition(_rigidbody.position + (direction * distance));
                }
            }


            netPos.Value = transform.position;
        } else {
            transform.position = Vector3.Lerp(transform.position, netPos.Value, 15f * Time.fixedDeltaTime);
        }

        // _totalMovement.Value = Vector3.zero;
    }

    
}
