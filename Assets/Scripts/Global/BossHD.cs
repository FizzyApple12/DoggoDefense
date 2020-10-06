using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHD : MonoBehaviour {

    public GameObject boss;

    void Start() {

    }

    void Update() {

    }

    public void OnTriggerEnter(Collider other) {
        boss.GetComponent<Boss>().OnTriggerEnter(other);
    }

    public void OnCollisionEnter(Collision other) {
        boss.GetComponent<Boss>().OnCollisionEnter(other);
    }

}
