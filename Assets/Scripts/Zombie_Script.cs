using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Script : MonoBehaviour {
    // Serialized Fields
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float lineOfSight;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private Animator zombie_animator;
    [SerializeField]
    private Rigidbody zombie_rb;

    // Private values
    private GameObject player;
    private Player_Script _playerScript;
    private Vector3 target;
    private int nextCheckpoint;
    private List<Transform> points = new List<Transform>();
    private float waitTime;
    private float startWaitTime = 1;
    private bool isDead;
    private AudioSource audioSource;
    private Rigidbody zombieRb;

    // Public values
    public int zombieHealth;
    public GameObject checkpoint;
    public AudioClip zombie_hurt;
    public AudioClip zombie_walk;
    public AudioClip zombie_attack;

    // Start is called before the first frame update
    void Start() {
        isDead = false;

        // player gameobject and script reference
        player = GameObject.FindGameObjectWithTag("Player");
        _playerScript = player.GetComponent<Player_Script>();

        audioSource = GetComponent<AudioSource>();
        zombieRb = GetComponent<Rigidbody>();

        // randomoze first checkpoint
        nextCheckpoint = Random.Range(0, checkpoint.transform.childCount);
        // populate the list
        for (int i = 0; i < checkpoint.transform.childCount; i++) {
            points.Add(checkpoint.transform.GetChild(i));
        }

        waitTime = startWaitTime;
    }

    // Update is called once per frame
    void Update() {
        // If zombie is dead
        if (isDead == true) {
            Dead();
        } 
        // else if zombie is alive 
        if (zombieHealth > 0) {

            if (Vector3.Distance(player.transform.position, transform.position) <= lineOfSight) {
                Follow_Player();
                checkpoint.transform.position = transform.position;
            } else {
                Patrol();
            }

            if (Vector3.Distance(player.transform.position, transform.position) <= attackRange) {
                Attack();
            }

        }

    }

    // Move towards the player
    private void Follow_Player() {
        zombieRb.isKinematic = false;

        // Make the player a target
        target = player.transform.position - transform.position;
        zombie_animator.SetFloat("MoveSpeed", target.magnitude);
        // Walk towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runSpeed * Time.deltaTime);
        // Face the player
        transform.LookAt(player.transform, Vector3.up);
    }

    // Walks randomly to different points
    private void Patrol() {
        zombieRb.isKinematic = true;
        //Debug.Log("Zombie is patrolling");
        // Make the next checkpoint the target
        target = points[nextCheckpoint].position - transform.position;
        zombie_animator.SetFloat("MoveSpeed", target.magnitude);
        // walk towards the next checkpoint
        transform.position = Vector3.MoveTowards(transform.position, points[nextCheckpoint].position, walkSpeed * Time.deltaTime);
        transform.LookAt(points[nextCheckpoint].position, Vector3.up);

        // patrol through random points
        if (Vector2.Distance(transform.position, points[nextCheckpoint].position) < 0.2f) {
            if (waitTime <= 0) {
                audioSource.PlayOneShot(zombie_walk, 1);
                nextCheckpoint = Random.Range(0, checkpoint.transform.childCount);
                waitTime = startWaitTime;
            } else {
                //print("waiting");
                zombie_animator.SetFloat("MoveSpeed", Vector3.zero.magnitude);
                waitTime -= Time.deltaTime;
            }
        }
    }

    // Attacks the player
    private void Attack() {
        zombie_animator.SetTrigger("Attack");
        //Debug.Log("Zombie Attacked");
    }

    // Zombie dies
    private void Dead() {
        zombie_animator.SetTrigger("Dead");
        transform.GetChild(2).gameObject.SetActive(true); // particles
        // make it throwable
        transform.GetChild(0).gameObject.SetActive(true);

        transform.gameObject.GetComponent<Animator>().enabled = false;
        transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        _playerScript.zombieCount--;
        _playerScript.zombieText.text = "Zombies: " + _playerScript.zombieCount.ToString();
        //transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        //Debug.Log("Zombie died");
        // Disable rigidbody

        isDead = false;
    }

    // Checks when colliders enter
    private void OnCollisionEnter(Collision col) {
        // collide with Ammo tag
        if (col.gameObject.CompareTag("Ammo") && col.gameObject.GetComponent<Ammo_Script>().in_Air == true) {
            Debug.Log("was hit with Ammo");
            audioSource.PlayOneShot(zombie_hurt, 1);
            zombieHealth--;
            if (zombieHealth <= 0) {
                isDead = true;
            }
        }

        // check for animation
        if (col.collider.CompareTag("Player") && Vector3.Distance(player.transform.position, transform.position) <= attackRange && zombieHealth >= 1) {
            Debug.Log("Hit player");
            audioSource.PlayOneShot(zombie_attack, 0.5f);
            _playerScript.playerHealth--;
            _playerScript.audioSource.PlayOneShot(_playerScript.player_hurt, 1);
        }
    }

}
