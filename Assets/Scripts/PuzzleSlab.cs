using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class PuzzleSlab : MonoBehaviour {
    public Action<Player> onPlayerStep;

    public new Renderer renderer {get; private set;}

    public AudioSource audioSource {get; private set;}


    private void Awake() {
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.white;
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
    }

    private void OnTriggerEnter(Collider collider) {
        Rigidbody rigidbody = collider.attachedRigidbody;
        if (rigidbody != null && rigidbody.gameObject.TryGetComponent(out Player player)) {
            onPlayerStep?.Invoke(player);
        }
    }
}