using System;
using UnityEngine;


public class PuzzleSlab : MonoBehaviour {
    public Action<Player> onPlayerStep;

    public Renderer renderer;


    private void Awake() {
        renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider collider) {
        Rigidbody rigidbody = collider.attachedRigidbody;
        if (rigidbody != null && rigidbody.gameObject.TryGetComponent(out Player player)) {
            onPlayerStep?.Invoke(player);
        }
    }
}