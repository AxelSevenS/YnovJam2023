using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class torch : MonoBehaviour {


    private float battery = 0;
    private const float maxBattery = 50f;
    private Light light;
    private int flashCount = 5;
    // private Enemy.EnemyState enemyController;


    private bool turnedOn = false;


    private void Start() {
        light = GetComponent<Light>();
        battery = maxBattery;
    }

    private void charge(){
        // battery = maxBattery;
        // CanMove= false;
        // yield return new WaitForSeconds(5);
        // CanMove= true;

    }

    private void flash(){
        if (flashCount > 0){
            // EnemyController.enemyState = EnemyController.EnemyState.Stunt;
            flashCount -= 1;
        }
    }

    private void Update() {

        if( Input.GetKeyDown(KeyCode.L) ){
            turnedOn = !turnedOn;
        }

        if (Input.GetKeyDown(KeyCode.F)){
            flash();
        }

        
        if( Input.GetKeyDown(KeyCode.C) ){
            
            charge();
        }
            
        if (battery == 0){
            turnedOn = false;
        }

        light.enabled = turnedOn;
        if (!turnedOn)
            return;

        battery -= 1f * Time.deltaTime;

        
    }
}