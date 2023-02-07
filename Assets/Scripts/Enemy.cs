using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : Character {

    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    [SerializeField] protected AudioSource audioSource;

    [SerializeField] protected AudioClip stepClip;
    [SerializeField] protected AudioClip screamClip;
    [SerializeField] protected GameObject jumpScarePrefab;

    [SerializeField] protected Player targetedPlayer;
    


    public EnemyState enemyState;

    protected const float wanderTimeMax = 40f;
    protected const float wanderRadius = 75f;

    protected float wanderTimer = 0f;
    protected float stunTimer = 0f;


    protected virtual float spottingDistance => 10f;
    protected virtual float escapeDistance => 15f;



    public override float movementSpeed {
        get {
            switch (enemyState) {
                case EnemyState.Wander:
                    return 4.5f;
                case EnemyState.Chase:
                    return 6.5f;
                default:
                    return 12f;
            }
        }
    }

    protected override void CharacterMovement() {

        Debug.Log("Enemy Movement !");

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


    protected virtual void Wander() {
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
    }

    protected virtual void Chase() {

        if (!targetedPlayer)
            return;

        if ( !targetedPlayer.enabled || (targetedPlayer.transform.position - characterCollider.transform.position).sqrMagnitude > Mathf.Pow(escapeDistance, 2f) ) {
            targetedPlayer = null;
            enemyState = EnemyState.Wander;
            return;
        }
        navMeshAgent.SetDestination(targetedPlayer.transform.position); 
        AttackTargetedPlayer();
    }

    protected virtual void Stunned() {
        stunTimer = Mathf.MoveTowards(stunTimer, 0f, Time.deltaTime);
        if (stunTimer == 0f)
            enemyState = EnemyState.Wander;
    }

    public virtual void Stun(float stunDuration) {
        stunTimer = stunDuration;
        NavMesh.SamplePosition(transform.position - transform.forward * 30f, out NavMeshHit navHit, 30f, -1);
        navMeshAgent.SetDestination(navHit.position);
        targetedPlayer = null;
        enemyState = EnemyState.Stunned;
        audioSource.PlayOneShot(screamClip);
    }


    protected void AttackTargetedPlayer() {

        if (!targetedPlayer)
            return;
            
        float sqrDistance = (targetedPlayer.transform.position - transform.position).sqrMagnitude;

        if (sqrDistance < 4f) {
            targetedPlayer.JumpScare(jumpScarePrefab);
            Stun(7f);
        }
    }


     
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = origin + UnityEngine.Random.insideUnitSphere.normalized * dist;

        NavMesh.SamplePosition (randDirection, out NavMeshHit navHit, 50f, layermask);

        return navHit.position;
    }

    protected override void Update() {
        base.Update();

        if (!audioSource.isPlaying) {
            audioSource.loop = true;
            audioSource.clip = stepClip;
            audioSource.Play();
            Debug.Log(audioSource.clip);
        }
    }

    protected override void Awake() {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Wander;

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
    }


    public enum EnemyState {
        Wander,
        Chase,
        Stunned
    };
}
