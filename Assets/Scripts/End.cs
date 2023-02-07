using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {
    [SerializeField] private GameObject finishPrefab;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out Player player)) {

            player.won = true;

            if (Player.playersWon.Count >= Player.players.Count) {
                foreach (Player wonPlayer in Player.players) {
                    
                    if (wonPlayer.IsOwner)
                        Instantiate(finishPrefab);

                    StartCoroutine(EndGame());

                }
            }
            Destroy(player.cameraController);
            Destroy(player.gameObject);
        }
    }

    private IEnumerator EndGame() {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }
}
