using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{

    public float speed;

    public GameObject player;
    void Start() {
        
    }

    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "blockyBoi" && player.GetComponent<Player>().swordSwinging) Destroy(gameObject);
        else if (other.gameObject.tag != "blockyBoi" || other.gameObject.tag != "doggyBoi") Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "blockyBoi" && player.GetComponent<Player>().swordSwinging) Destroy(gameObject);
        else if (other.gameObject.tag != "blockyBoi" || other.gameObject.tag != "doggyBoi") Destroy(gameObject);
    }
}
