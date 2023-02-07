using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  


public class StalkerEnemy : Enemy {


    public float slowness = 1f;



    public override float movementSpeed => base.movementSpeed * slowness;



    protected override void CharacterMovement() {
        
        slowness = Mathf.MoveTowards(slowness, 1f, 0.5f * Time.deltaTime);

        base.CharacterMovement();
    }

    // protected override void Wander() {
    // }
    

    // protected override void Chase() {
    // }

}
