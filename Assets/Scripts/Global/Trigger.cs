using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    public bool triggered = false;
    void Start() {

    }

    void Update() {

    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "blockyBoi") {
            if (!triggered) {
                GetComponent<AudioSource>().Play();
            }
            triggered = true;
        }
    }
}
