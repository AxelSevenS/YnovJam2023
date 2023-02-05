using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrap : MonoBehaviour {

    private void OnTriggerEnter(Collider collider) {
        Rigidbody rigidbody = collider.attachedRigidbody;
        if (rigidbody != null && rigidbody.gameObject.TryGetComponent(out Enemy enemy)) {
            enemy.Stun(10f);
        }
        
    }

}
