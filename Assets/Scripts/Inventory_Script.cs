using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Script : MonoBehaviour {
    private Animator player_animator;
    private GameObject player;
    private Player_Script _playerScript;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        _playerScript = player.GetComponent<Player_Script>();
        player_animator = player.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {
        
    }

    // Checks when trigger colliders start to touch
    private void OnTriggerStay(Collider col) {
        // collide with inventory
        if (col.gameObject.CompareTag("Ammo") && Input.GetKeyDown(KeyCode.RightShift)) {
            // Pickup only if less than 5 items
            if (_playerScript.inventory.Count >= 5) {
                Debug.Log("Can't carry anymore items");
            } else {
                player_animator.SetTrigger("Pickup");
                _playerScript.inventory.Add(col.gameObject);

                col.gameObject.SetActive(false);
                col.transform.parent = transform;
                col.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}
