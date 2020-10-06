using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EzySlice;

public class Enemy : MonoBehaviour {

    public GameObject head;
    public GameObject body;
    public GameObject gunBarrel;
    public GameObject player;
    public GameObject dog;
    public GameObject bulletPrefab;
    public Material bodyMaterial;
    public bool alive = true;
    public int despawnTime = 0;

    private GameObject[] slices;

    public int despawnTimer = 0;
    public int shootInterval = 180;
    private int shootTime = 0;
    private int shootAt = 0;
    private System.Random shootRNG = new System.Random();

    private int moveMode = 0;
    private bool playerSeen = false;
    public float speed = 5.0f;
    public float maxVelocityChange = 5.0f;
    private Vector3 target;
    private System.Random moveRNG = new System.Random();
    public AudioClip shootsfx;

    public UnityEvent playerDetect;
    public UnityEvent playerKill;

    private Rigidbody rigidBody;

    void Start() {
        shootAt = shootRNG.Next(0, shootInterval);
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    void Update() {
        if (!alive) {
            despawnTimer++;
            if (despawnTimer >= despawnTime) {
                Destroy(gameObject);
                foreach (GameObject slice in slices) {
                    Destroy(slice);
                }
            }
            return;
        }

        if (player.GetComponent<Player>().paused) return;

        if (playerSeen) head.transform.LookAt(dog.transform.position);

        if (playerSeen) shootTime++;

        if (shootTime > shootInterval) {
            shootTime = 0;
            shootAt = shootRNG.Next(0, shootInterval);
        }

        if (shootTime == shootAt) Shoot();

        RaycastHit hit;
        if (!playerSeen && Physics.Linecast(head.transform.position, player.transform.position, out hit) && hit.transform.tag == "blockyBoi") {
            playerSeen = true;
            playerDetect.Invoke();
            moveMode = 0;
        }
        
        if (playerSeen) {
            switch (moveMode) {
                case 0:
                    target = player.transform.position;
                    if (Vector3.Distance(player.transform.position, transform.position) <= 2) moveMode = moveRNG.Next(3, 5);
                    if (Vector3.Distance(player.transform.position, transform.position) >= 10) moveMode = 2;
                    break;
                case 1:
                    target = transform.position + (transform.position - player.transform.position);
                    if (Vector3.Distance(player.transform.position, transform.position) <= 2) moveMode = 0;
                    if (Vector3.Distance(player.transform.position, transform.position) >= 10) moveMode = 2;
                    break;
                case 2:
                    target = transform.position;
                    if (Vector3.Distance(player.transform.position, transform.position) <= 8) moveMode = moveRNG.Next(3, 5);
                    if (Vector3.Distance(player.transform.position, transform.position) >= 15) moveMode = 0;
                    break;
                case 3:
                    target = transform.position + Vector3.Cross((transform.position - player.transform.position), Vector3.up);
                    if (Vector3.Distance(player.transform.position, transform.position) <= 2) moveMode = 1;
                    if (Vector3.Distance(player.transform.position, transform.position) >= 10) moveMode = 0;
                    break;
                case 4:
                    target = transform.position + Vector3.Cross((transform.position - player.transform.position), Vector3.down);
                    if (Vector3.Distance(player.transform.position, transform.position) <= 2) moveMode = 1;
                    if (Vector3.Distance(player.transform.position, transform.position) >= 10) moveMode = 0;
                    break;
            }
        } else {
            target = transform.position;
        }

        Vector3 targetVelocity = (transform.position - target).normalized;
	    targetVelocity = transform.TransformDirection(targetVelocity);
	    targetVelocity *= speed;
	    Vector3 velocity = rigidBody.velocity;
	    Vector3 velocityChange = (targetVelocity - velocity);
	    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
	    velocityChange.y = 0;
	    rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void Kill(Vector3 slicePos, Vector3 sliceDirection) {
        slices = body.SliceInstantiate(slicePos, sliceDirection, bodyMaterial);
        if (slices == null) {
            slices = new GameObject[1]{ body };
        }
        foreach (GameObject slice in slices) {
            slice.transform.position = transform.position;
            slice.AddComponent(typeof(CapsuleCollider));
            slice.AddComponent(typeof(Rigidbody));
        }
        body.GetComponent<Renderer>().enabled = false;
        head.transform.parent = slices[0].transform;
        alive = false;
        playerKill.Invoke();
    }

    void Shoot() {
        GameObject go = Instantiate(bulletPrefab, gunBarrel.transform.position, gunBarrel.transform.rotation);
        go.GetComponent<Bullet>().player = player;
        go.transform.LookAt(dog.transform.position);
        GetComponent<AudioSource>().PlayOneShot(shootsfx);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "slicyBoi" && alive) Kill(other.transform.position, other.gameObject.transform.rotation.normalized.eulerAngles);
    }
    public void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "spicyBoi") Destroy(gameObject);
    }
}
