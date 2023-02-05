using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {
    private int players = 0;

    [SerializeField] private GameObject finishPrefab;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out Player player)) {
            players++;
            Destroy(player.cameraController);
            Destroy(player.gameObject);
        }
        if (players >= Player.players.Count) {
            Instantiate(finishPrefab);
        }
    }
}
