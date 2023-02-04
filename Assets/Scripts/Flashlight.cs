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

    private Quaternion forwardRotation = Quaternion.identity;

    private bool flashLightOn = false;
    public bool charging = false;

    private List<Enemy> flashTargets = new List<Enemy>();
    
    private const float flashLightChargeTime = 5f;

    const float normalLightIntensity = 5f;
    private float lightIntensity = normalLightIntensity;
    
    const float normalLightRange = 10f;
    private float lightRange = normalLightRange;

    const float normalLightSize = 100f;
    private float lightAngle = normalLightSize;

    private const int maxFlashCount = 5;
    private int flashCount = maxFlashCount;

    private const float maxBattery = 30f;
    [SerializeField] private float battery = maxBattery;

    public bool chargeInput = false;
    public bool toggleInput = false;
    public bool flashInput = false;

    public Vector3 originPosition;



    private void UpdateFlashLightTargets() {
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

    public void PointFlashLight(Quaternion rotation) {
        forwardRotation = rotation;
    }

    public void ActivateFlash(){
        if (flashCount > 0){

            lightIntensity = 50f;
            lightAngle = 150f;
            lightRange = 20f;
            audioSource.clip = flashlightFlashClip;
            audioSource.Play();

            foreach (Enemy enemy in flashTargets) {
                enemy?.Stun(5f);
            }

            flashCount -= 1;
        }
        else{
            audioSource.clip = flashlightDeadClip;
            audioSource.Play();
            Debug.Log("No more flash");
        }
    }

    public void ToggleFlashLight() {
        flashLightOn = !flashLightOn;

        audioSource.clip = flashlightToggleClip;
        audioSource.Play();
    }

    private void Start() {
        battery = maxBattery;
        flashCount = maxFlashCount;

        lightIntensity = normalLightIntensity;
        lightRange = normalLightRange;
        lightAngle = normalLightSize;
    }

    private void Update() {
        float batteryAmount = battery/maxBattery;

        lightIntensity = Mathf.MoveTowards(lightIntensity, batteryAmount * normalLightIntensity, 25f * Time.deltaTime);
        light.intensity = lightIntensity;

        lightRange = Mathf.MoveTowards(lightRange, batteryAmount * normalLightRange, 25f * Time.deltaTime);
        light.range = lightRange;

        lightAngle = Mathf.MoveTowards(lightAngle, normalLightSize, 25f * Time.deltaTime);
        light.spotAngle = lightAngle;


        // Charge Flashlight
        if (!charging && chargeInput) {
            audioSource.clip = flashlightChargeClip;
            audioSource.Play();
        } else if (charging && !chargeInput) {
            audioSource.Stop();
        }
        charging = chargeInput;
        if (charging) {
            battery = Mathf.MoveTowards(battery, maxBattery, maxBattery / flashLightChargeTime * Time.deltaTime);
        }
            
        bool canToggle = true;
        if (battery == 0){
            flashLightOn = false;
            canToggle = false;
        }
        
        if( toggleInput && canToggle ) {
            ToggleFlashLight();
        }

        flashTargets.Clear();

        light.enabled = flashLightOn && !charging;
        if (!light.enabled)
            return;

        UpdateFlashLightTargets();

        if (flashInput){
            ActivateFlash();
        }

        battery = Mathf.MoveTowards(battery, 0, Time.deltaTime);
    }

    private void FixedUpdate() {
        
        transform.rotation = forwardRotation * Quaternion.AngleAxis(90f, Vector3.right);
        // transform.position = originPosition + transform.rotation * new Vector3(0.75f, 0, 0.3f);
    }
}
