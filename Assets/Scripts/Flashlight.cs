using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

public class Flashlight : NetworkBehaviour {

    [SerializeField] protected Light light;

    [Space(15)]

    [SerializeField] private AudioClip flashlightDeadClip;
    [SerializeField] private AudioClip flashlightFlashClip;
    [SerializeField] private AudioClip flashlightToggleClip;
    [SerializeField] private AudioClip flashlightChargeClip;

    [SerializeField] private AudioSource audioSource;

    [Space(15)]

    public NetworkVariable<Quaternion> forwardRotation = new(value: Quaternion.identity, writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private NetworkVariable<bool> flashLightOn = new(value: false, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> charging = new(value: false, writePerm: NetworkVariableWritePermission.Server);

    private List<Enemy> flashTargets = new List<Enemy>();
    
    private const float flashLightChargeTime = 5f;

    private const float normalLightIntensity = 50f;
    [SerializeField] private NetworkVariable<float> lightIntensity = new(value: normalLightIntensity, writePerm: NetworkVariableWritePermission.Server);
    
    private const float normalLightRange = 75f;
    [SerializeField] private NetworkVariable<float> lightRange = new(value: normalLightRange, writePerm: NetworkVariableWritePermission.Server);

    private const float normalLightSize = 100f;
    [SerializeField] private NetworkVariable<float> lightAngle = new(value: normalLightSize, writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> flashInitiated = new(value: false, writePerm: NetworkVariableWritePermission.Owner);
    private const int maxFlashCount = 5;
    [SerializeField] private NetworkVariable<int> flashCount = new(value: maxFlashCount, writePerm: NetworkVariableWritePermission.Server);

    private const float maxBattery = 30f;
    [SerializeField] private NetworkVariable<float> battery = new(value: maxBattery, writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> chargeInput = new(value: false, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> toggleInput = new(value: false, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> flashInput = new(value: false, writePerm: NetworkVariableWritePermission.Owner);

    public Vector3 originPosition;


    public bool lightEnabled => flashLightOn.Value && !charging.Value;
    public bool canToggle => battery.Value > 0 && !charging.Value;



    private void UpdateFlashLightTargets() {

        if (!IsServer) 
            return;

        flashTargets.Clear();

        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = capsuleStart + transform.forward * 7f;

        Collider[] hitColliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, 4f);
        foreach (Collider collider in hitColliders) {
            if (collider.attachedRigidbody != null && collider.attachedRigidbody.TryGetComponent(out Enemy targetedEnemy)) {
                flashTargets.Add(targetedEnemy);
            }
        }
    }

    public void ActivateFlash(){
        Debug.Log("Flash");
        if (flashCount.Value > 0){

            if (IsServer) {

                lightIntensity.Value = 500f;
                lightAngle.Value = 150f;
                lightRange.Value = 150f;

                foreach (Enemy enemy in flashTargets) {
                    enemy?.Stun(5f);
                }

                flashCount.Value -= 1;
            }

            audioSource.clip = flashlightFlashClip;
            audioSource.Play();

        } else {
            audioSource.clip = flashlightDeadClip;
            audioSource.Play();
        }
    }

    private void ToggleLight() {
        Debug.Log("Toggle");
        audioSource.clip = flashlightToggleClip;
        audioSource.Play();

        if (IsOwner)
            flashLightOn.Value = !flashLightOn.Value;
    }

    private void Start() {
        if (!IsServer)
            return;
            
        battery.Value = maxBattery;
        flashCount.Value = maxFlashCount;

        lightIntensity.Value = normalLightIntensity;
        lightRange.Value = normalLightRange;
        lightAngle.Value = normalLightSize;
    }

    private void Update() {
        float batteryAmount = battery.Value/maxBattery;


        // Charging Flashlight
        if (!charging.Value && chargeInput.Value) {
            audioSource.clip = flashlightChargeClip;
            audioSource.Play();
        } else if (charging.Value && !chargeInput.Value) {
            audioSource.Stop();
        }


        if (battery.Value == 0) {

            if (IsServer) {
                flashLightOn.Value = false;
            }

        } else if (toggleInput.Value && canToggle) {
            ToggleLight();
        }


        if (flashInput.Value) {
            ActivateFlash();
        }



        if (IsServer) {

            charging.Value = chargeInput.Value;
            if (charging.Value) {
                battery.Value = Mathf.MoveTowards(battery.Value, maxBattery, maxBattery / flashLightChargeTime * Time.deltaTime);
            }

            lightIntensity.Value = Mathf.MoveTowards(lightIntensity.Value, batteryAmount * normalLightIntensity, 100f * Time.deltaTime);
            lightRange.Value = Mathf.MoveTowards(lightRange.Value, batteryAmount * normalLightRange, 25f * Time.deltaTime);
            lightAngle.Value = Mathf.MoveTowards(lightAngle.Value, normalLightSize, 25f * Time.deltaTime);

            flashTargets.Clear();

            if (lightEnabled) {
                UpdateFlashLightTargets();

                foreach (Enemy enemy in flashTargets) {
                    enemy.slowness = 0.5f;
                }

                battery.Value = Mathf.MoveTowards(battery.Value, 0, Time.deltaTime);
            }

        }

        light.enabled = lightEnabled;

        light.intensity = lightIntensity.Value;
        light.range = lightRange.Value;
        light.spotAngle = lightAngle.Value;


    }

    private void FixedUpdate() {
        
        transform.rotation = Quaternion.Slerp(transform.rotation, forwardRotation.Value * Quaternion.AngleAxis(90f, Vector3.right), 15f * Time.fixedDeltaTime);
        // transform.position = originPosition + transform.rotation * new Vector3(0.75f, 0, 0.3f);
    }
}
