using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo_Script : MonoBehaviour {
    public bool in_Air;

    //


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    // Checks when colliders are inside each other
    private void OnCollisionStay(Collision col) {
     if (col.collider.CompareTag("Floor")) {
            in_Air = false;
        }
    }
}
