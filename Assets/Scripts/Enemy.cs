using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

    private NavMeshAgent navMeshAgent;
    
    public EnemyState enemyState;


    private Player targetedPlayer;

    private float stunTimer = 0f;

    private const float wanderTimeMax = 40f;
    public float wanderTimer = 0f;

    public float wanderRadius = 100f;



    public override float movementSpeed {
        get {
            switch (enemyState) {
                case EnemyState.Wander:
                    return 1.5f;
                case EnemyState.Chase:
                    return 6.5f;
                default:
                    return 0f;
            }
        }
    }

    protected override void Die() {
        Debug.Log("Enemy Died !");
    }

    protected override void CharacterMovement() {
        navMeshAgent.speed = movementSpeed;
        switch (enemyState) {
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Stunned:
                Stunned();
                break;
        }
    }

    private void AttackTargetedPlayer() {
        float sqrDistance = (targetedPlayer.transform.position - transform.position).sqrMagnitude;
        // Debug.Log(sqrDistance);
        if (sqrDistance < 4f) {
            targetedPlayer.health = Mathf.MoveTowards( targetedPlayer.health, 0, 25f * Time.deltaTime);
            Debug.Log(targetedPlayer.health);
        }
    }
     
     public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
 
        randDirection += origin;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
        return navHit.position;
    }

    protected void Wander() {
        const float spottingDistance = 10f;

        foreach (Player player in Player.players) {
            float sqrDistanceToPlayer = (player.transform.position - characterCollider.transform.position).sqrMagnitude;

            // See Players
            if ( sqrDistanceToPlayer < Mathf.Pow(spottingDistance, 2f)) {
                targetedPlayer = player;
                enemyState = EnemyState.Chase;
                return;
            }

            // Hear Players
            if ( sqrDistanceToPlayer < Mathf.Pow(player.hearingDistance, 2f) ) {
                navMeshAgent.SetDestination(player.transform.position);
            }
        }

        // Go to random position or alert Position
        float sqrDistanceToDestination = (navMeshAgent.destination - characterCollider.transform.position).sqrMagnitude;
        if (sqrDistanceToDestination < Mathf.Pow(2f, 2f)){
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            Debug.Log(newPos);
            navMeshAgent.SetDestination(newPos);
        }
    }
    

    protected void Chase() {
        const float escapeDistance = 15f;

        if ( !targetedPlayer.enabled || (targetedPlayer.transform.position - characterCollider.transform.position).sqrMagnitude > Mathf.Pow(escapeDistance, 2f) ) {
            targetedPlayer = null;
            enemyState = EnemyState.Wander;
            return;
        }
        navMeshAgent.SetDestination(targetedPlayer.transform.position); 
        AttackTargetedPlayer();
    }

    protected void Stunned() {
        stunTimer = Mathf.MoveTowards(stunTimer, 0f, Time.deltaTime);
        if (stunTimer == 0f)
            enemyState = EnemyState.Wander;
    }
    

    public void Stun(float stunDuration) {
        stunTimer = stunDuration;
        targetedPlayer = null;
        enemyState = EnemyState.Stunned;
    }

    // private void OnAlert(Vector3 alertPosition) {
    //     const float alertDistance = 30f;

    //     float sqrDistanceToAlert = (alertPosition - characterCollider.transform.position).sqrMagnitude;
    //     if (sqrDistanceToAlert < Mathf.Pow(alertDistance, 2f)) {
    //         navMeshAgent.SetDestination(alertPosition);
    //     }
    // }


    // private void OnEnable() {
    //     onAlertEnemies += OnAlert;
    // }

    // private void OnDisable() {
    //     onAlertEnemies -= OnAlert;
    // }

    protected override void Awake() {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Wander;
    }


    public enum EnemyState {
        Wander,
        Chase,
        Stunned
    };

}
