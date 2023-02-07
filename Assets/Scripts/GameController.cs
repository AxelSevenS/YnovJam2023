using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    [SerializeField] private GameObject finishPrefab;

    private void OnEnable() {
        instance = this;
    }

    public void CompletePuzzles() {
        if (MusicPlatesController.instance.state.Value == MusicPlatesController.State.Completed && SlabsController.completed) {
            Debug.Log("All puzzles completed!");
            Instantiate(finishPrefab, new Vector3(-35, -10f, -120f), Quaternion.identity);
        }
    }
}
