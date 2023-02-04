using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using UnityEngine.Ai;

public class EnemyController : MonoBehaviour {
    // public static EnemyState enemyState = EnemyState.Wander;
    

    public UnityEngine.AI.NavMeshAgent enemy;
    public List<Player> players = new List<Player>();
    public int maxEnemyCount = 1;
    // private int _selectedPlayer = 0;
    public float spawnRate = 5.0f;
    public float wanderRadius = 5.0f;

    private float nextSpawnTime = 0.0f;

    // private void Wander(){
    //     // if (Time.time > nextSpawnTime){
    //     //     nextSpawnTime = Time.time + spawnRate;
    //     //     UnityEngine.AI.NavMeshAgent spawnedHunter = Instantiate(enemy, transform.position, transform.rotation);
    //     //     EnemyMover HunterMover = spawnedHunter.GetComponent<EnemyMover>();
    //     //     HunterMover.wanderRadius = wanderRadius;
    //     // }
    // }

    // private void Update(){
    // }

    // private void HunterController(){
    //     enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
    //     foreach(GameObject player in GameObject.FindWithTag("Player")){
    //         players.Add(player);
    //     }
    // }
    // private void Start(){
    //     // Movements();
    //     HunterController();
    // }

    // void Spawn(){
    //     if(enemy.Count < maxEnemyCount){
    //         //int spawnPoint =
    //         currentEnemies++; 
    //     }
    //     else{
            
    //     }
    // }
}
