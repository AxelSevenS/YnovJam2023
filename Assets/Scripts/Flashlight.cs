using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    [SerializeField] protected Light light;

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
    private int flashCount = 5;

    [SerializeField] private float battery = 0;
    private const float maxBattery = 30f;

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

            lightIntensity = 35f;
            lightAngle = 150f;
            // Play Flash Sound

            foreach (Enemy enemy in flashTargets) {
                enemy?.Stun(5f);
            }

            flashCount -= 1;
        }
        else{
            // Play Broken Light Sound

            Debug.Log("No more flash");
        }
    }

    public void ToggleFlashLight() {
        flashLightOn = !flashLightOn;
        // Play Click Sound
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

        // Enable Flashlight when finished Charging
        if (charging && !chargeInput)
            ToggleFlashLight();

        // Charge Flashlight
        charging = chargeInput;
        if (charging) {
            battery = Mathf.MoveTowards(battery, maxBattery, maxBattery / flashLightChargeTime * Time.deltaTime);
        }
            
        bool canToggle = true;
        if (battery == 0 || charging){
            flashLightOn = false;
            canToggle = false;
        }
        
        if( toggleInput && canToggle ) {
            ToggleFlashLight();
        }

        flashTargets.Clear();

        light.enabled = flashLightOn;
        if (!flashLightOn)
            return;

        UpdateFlashLightTargets();

        if (flashInput){
            ActivateFlash();
        }

        battery = Mathf.MoveTowards(battery, 0, Time.deltaTime);
    }

    private void FixedUpdate() {
        
        light.transform.rotation = forwardRotation;
        light.transform.position = originPosition + transform.rotation * new Vector3(0.75f, 0, 0.3f);
    }
}
