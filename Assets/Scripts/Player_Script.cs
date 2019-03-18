using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_Script : MonoBehaviour{
    // Serialized variables
    [SerializeField]
    private float walk_speed;
    [SerializeField]
    private float rotate_speed;
    [SerializeField]
    private float jump_force;
    [SerializeField]
    private Animator player_animator;
    [SerializeField]
    private Rigidbody player_rb;
    [SerializeField]
    private Sprite heart_full;
    [SerializeField]
    private Sprite heart_empty;
    [SerializeField]
    private Image[] hearts;
    [SerializeField]
    private Sprite item_full;
    [SerializeField]
    private Sprite item_empty;
    [SerializeField]
    private Image[] items;

    // private variables
    private bool is_grounded;
    private GameObject inv_gameobject;
    private int maxPlayerHealth;
    private bool display_Text;


    // public variables
    public int playerHealth;
    public AudioClip player_hurt;
    public AudioSource audioSource;
    public int zombieCount;
    public Text zombieText;
    public Text start_Text;

    // public variables
    public List<GameObject> inventory = new List<GameObject>();

    // Start is called before the first frame update
    private void Start() {
        maxPlayerHealth = hearts.Length;
        playerHealth = maxPlayerHealth;
        //zombieCount = 30;
        display_Text = true;
        player_rb = GetComponent<Rigidbody>();
        inv_gameobject = GameObject.Find("Inventory");

        audioSource = GetComponent<AudioSource>();

        //Instantiate(gameObjectModelPrefab, transform);
        zombieText.text = "Zombies: " + zombieCount.ToString();
    }

    // Update is called once per frame
    private void Update(){
        Initial_Text();
        Player_Movement();
        Player_Jump();
        Camera_Rotation();
        Resources();

        if (Input.GetKeyDown(KeyCode.Return)) {
            Throw_Item();
        }

        if (playerHealth <= 0) {
            SceneManager.LoadScene(2);
        }

        player_animator.SetBool("Grounded", is_grounded);

    }

    private void Initial_Text() {
        if (display_Text == true) {
            start_Text.text = "Objective:\nSurvive by grabbing as many objects as you can to defend yourself!";
        } else {
            start_Text.text = "";
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            display_Text = false;
        }
    }

    // Movement player control
    private void Player_Movement(){
        Vector3 movement = (Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right).normalized;
        player_rb.AddForce(movement * walk_speed);
        player_animator.SetFloat("MoveSpeed", movement.magnitude);
    }

    // Makes the player jump
    private void Player_Jump() {
        // Player Jump
        if (Input.GetKeyDown(KeyCode.Space) && is_grounded) {
            player_rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        } else if (!is_grounded) {
            player_rb.AddForce(Vector3.down * jump_force, ForceMode.Force);
        }

        if (is_grounded == false) {
            player_animator.SetTrigger("Jump");
        } 

        if (is_grounded == true) {
            player_animator.SetTrigger("Land");
        }
    }

    // Rotate camera around the player
    private void Camera_Rotation() {
        // Camera rotation control
        if (Input.GetKey(KeyCode.LeftArrow)) {
            //Debug.Log("turn camera left");
            transform.Rotate(0, -rotate_speed * Time.deltaTime, 0);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            //Debug.Log("turn camera right");
            transform.Rotate(0, rotate_speed * Time.deltaTime, 0);
        }
    }

    // Updates the health and item UI
    private void Resources(){
        // updates health in the ui canvas
        for (int i = 0; i < hearts.Length; i++) {
            if (i < playerHealth) {
                hearts[i].sprite = heart_full;
            } else {
                hearts[i].sprite = heart_empty;
            }
        }

        // updates items carries in the ui canvas
        for (int i = 0; i < items.Length; i++) {
            if(i < inventory.Count) {
                items[i].sprite = item_full;
            } else {
                items[i].sprite = item_empty;
            }
        }
    }

    // Throw invetory item
    private void Throw_Item() {
        if (inventory.Count <= 0) {
            //Debug.Log("Nothing in inventory");
        } else {
            //Debug.Log("Throw object");
            inventory[0].GetComponent<Ammo_Script>().in_Air = true;
            inventory[0].gameObject.transform.parent = null;
            inventory[0].gameObject.SetActive(true);
            inventory[0].GetComponent<Rigidbody>().AddForce(inv_gameobject.transform.forward * 400);
            inventory.RemoveAt(0);
            
        }

    }

    // Checks when colliders enter
    private void OnCollisionEnter(Collision col) {
        // collide with Floor
        if (col.gameObject.CompareTag("Floor")) {
            is_grounded = true;
            //Debug.Log("Colliding with floor: " + is_grounded);
        }

    }

    // Checks when colliders are inside
    private void OnCollisionStay(Collision col) {
        if (col.gameObject.CompareTag("Floor")) {
            is_grounded = true;
            //Debug.Log("Colliding with floor: " + is_grounded);
        }
    }

    // Checks when colliders are not in contact
    private void OnCollisionExit(Collision col) {
        // collide with Floor
        if (col.gameObject.CompareTag("Floor")) {
            is_grounded = false;
            //Debug.Log("Colliding with floor: " + is_grounded);
        }

    }

}