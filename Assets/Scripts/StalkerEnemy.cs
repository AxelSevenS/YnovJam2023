using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

    private NavMeshAgent navMeshAgent;
    
    public EnemyState enemyState;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip screamClip;
    [SerializeField] private AudioClip stepClip;


    private Player targetedPlayer;

    private float stunTimer = 0f;

    private const float wanderTimeMax = 40f;
    public float wanderTimer = 0f;

    public float wanderRadius = 75f;


    public float slowness = 1f;



    public override float movementSpeed {
        get {
            switch (enemyState) {
                case EnemyState.Wander:
                    return 4.5f;
                case EnemyState.Chase:
                    return 6.5f * slowness;
                default:
                    return 12f;
            }
        }
    }

    protected override void Die() {
        Debug.Log("Enemy Died !");
    }

    protected override void CharacterMovement() {

        Debug.Log("Enemy Movement !");

        navMeshAgent.speed = movementSpeed;
        slowness = Mathf.MoveTowards(slowness, 1f, 0.5f * Time.deltaTime);

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
        
        if (sqrDistance < 4f) {
            targetedPlayer.health = Mathf.MoveTowards( targetedPlayer.health, 0, 25f * Time.deltaTime);
            Debug.Log(targetedPlayer.health);
        }
    }
     
     public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = origin + UnityEngine.Random.insideUnitSphere.normalized * dist;
 
        NavMesh.SamplePosition (randDirection, out NavMeshHit navHit, 50f, layermask);
 
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
            navMeshAgent.SetDestination(newPos);
        }

        Debug.Log(navMeshAgent.destination);
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
        NavMesh.SamplePosition(transform.position - transform.forward * 30f, out NavMeshHit navHit, 30f, -1);
        navMeshAgent.SetDestination(navHit.position);
        audioSource.PlayOneShot(screamClip);
        targetedPlayer = null;
        enemyState = EnemyState.Stunned;
    }

    protected override void Awake() {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Wander;
    }

    protected override void Update() {
        base.Update();

        if (!audioSource.isPlaying) {
            audioSource.loop = true;
            audioSource.clip = stepClip;
            audioSource.Play();
        }
    }


    public enum EnemyState {
        Wander,
        Chase,
        Stunned
    };

}
