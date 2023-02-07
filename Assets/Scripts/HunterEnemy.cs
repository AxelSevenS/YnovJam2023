using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  


public class HunterEnemy : Enemy {


    public float movementMultiplier = 1f;



    public override float movementSpeed {
        get {
            if (enemyState == EnemyState.Stunned) {
                return 12f;
            }

            return base.movementSpeed * movementMultiplier;
        }
    }




    protected override void CharacterMovement() {

        foreach (Player player in Player.players) {
            // Check if the enemy is in the player's field of view and line of sight

            Vector3 direction = transform.position - player.transform.position;
            Vector3 cameraDirection = player.cameraRotation.Value * Vector3.forward;

            bool inFOV = Vector3.Dot(direction, cameraDirection) > 0.4f;
            bool inLOS = Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude, LayerMask.GetMask("Default"));
            bool isClose = direction.sqrMagnitude < Mathf.Pow(25f, 2f);

            Debug.Log(hit.point + " " + inLOS);
            
            // use direction and cameraDirection to check if the enemy can be seen by the player
            if (inFOV/*  && inLOS */ && isClose) {
                movementMultiplier = 0;
            }
        }
        animator.speed = movementMultiplier;
        audioSource.volume = movementMultiplier;

        Debug.Log($"movementMultiplier: {movementMultiplier}");

        base.CharacterMovement();
        
        movementMultiplier = Mathf.MoveTowards(movementMultiplier, 1f, 5f * Time.deltaTime);
    }

    protected override void Wander() {
        foreach (Player player in Player.players) {

            if (targetedPlayer == null) {
                targetedPlayer = player;
                enemyState = EnemyState.Chase;
                return;
            }

            float sqrDistanceToPlayer = (player.transform.position - characterCollider.transform.position).sqrMagnitude;
            float sqrDistanceToTargetPlayer = (targetedPlayer.transform.position - characterCollider.transform.position).sqrMagnitude;

            // See Players
            if ( targetedPlayer == null || sqrDistanceToPlayer < sqrDistanceToTargetPlayer) {
                targetedPlayer = player;
                enemyState = EnemyState.Chase;
            }
        }

        // // Go to random position or alert Position
        // float sqrDistanceToDestination = (navMeshAgent.destination - characterCollider.transform.position).sqrMagnitude;
        // if (sqrDistanceToDestination < Mathf.Pow(2f, 2f)){
        //     Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        //     navMeshAgent.SetDestination(newPos);
        // }
    }

    protected override void Chase() {
        Wander();

        if (!targetedPlayer)
            return;

        navMeshAgent.SetDestination(targetedPlayer.transform.position); 
        AttackTargetedPlayer();
    }

    // protected override void Stunned() {
    //     base.Stunned();
    // }

}
