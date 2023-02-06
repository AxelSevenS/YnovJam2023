using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class PuzzleSlab : MonoBehaviour {
    public Action<Player> onPlayerStep;

    public Renderer renderer {get; private set;}

    public AudioSource audioSource {get; private set;}


    private void Awake() {
        renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider) {
        Rigidbody rigidbody = collider.attachedRigidbody;
        if (rigidbody != null && rigidbody.gameObject.TryGetComponent(out Player player)) {
            onPlayerStep?.Invoke(player);
        }
    }
}