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

    public float wanderRadius = 10f;


    public override float movementSpeed {
        get {
            switch (enemyState) {
                case EnemyState.Wander:
                    return 1f;
                case EnemyState.Chase:
                    return 3f;
                default:
                    return 0f;
            }
        }
    }



    protected override void Die() {
        Debug.Log("Enemy Died !");
    }

    protected override void CharacterMovement() {
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


    protected void Wander() {
        const float spottingDistance = 5f;

        foreach (Player player in Player.players) {
            float distanceToPlayer;
            if ((player.transform.position - characterCollider.transform.position).sqrMagnitude < Mathf.Pow(spottingDistance, 2f)) {
                targetedPlayer = player;
                enemyState = EnemyState.Chase;
                return;
            }
        }
        //Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        //navMeshAgent.SetDestination(newPos);

        // Guillaume ici
    }

    protected void Chase() {
        const float escapeDistance = 10f;

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
