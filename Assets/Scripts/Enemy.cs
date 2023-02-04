using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    private Player targetedPlayer;
    public EnemyState enemyState;

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

    private void Update() {

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


    private float wanderTargetTimer = 0f;
    private const float wonderTargetCooldown = 30f;
    protected void Wander() {
        if (wanderTargetTimer > wonderTargetCooldown) {
            wanderTargetTimer = 0f;

            // int index = Random.Range(0, Player.players.Count);
            // targetedPlayer = Player.players[index];
        }
        wanderTargetTimer += Time.deltaTime;


        // Move Towards targetPlayer
    }

    protected void Chase() {
        // Move Towards targetPlayer
        //if ((targetedPlayer.transform.position - transform.position).sqrMagnitude > 1f) {
          //  Vector3 direction = (targetedPlayer.transform.position - transform.position).normalized;
            //transform.position += direction * movementSpeed * Time.deltaTime;
        //}
        
        // Attack targeted Player
        if ((targetedPlayer.transform.position - transform.position).sqrMagnitude < 1f) {
            targetedPlayer.health -= 25f * Time.deltaTime;

        }
    }

    private float stunTimer = 0f;
    protected void Stunned() {
        stunTimer = Mathf.MoveTowards(stunTimer, 0f, Time.deltaTime);
        
    }




    protected override void CharacterMovement() {
        
    }

    protected override void Die() {
        Debug.Log("You Died !");
        // Player.players.Remove(targetedPlayer);
 
    }



    public void Stun(float stunDuration) {
        stunTimer = stunDuration;
        enemyState = EnemyState.Stunned;
        

    }


    public enum EnemyState {
        Wander,
        Chase,
        Stunned
    };

}
