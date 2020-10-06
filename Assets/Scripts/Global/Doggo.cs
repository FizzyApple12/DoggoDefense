using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doggo : MonoBehaviour {
    public bool immutable = false;
    void Start() {
        
    }

    void Update() {
        
    }

    public void OnTriggerEnter(Collider other) {
        if (!immutable && other.gameObject.tag == "bulletBoi" || other.gameObject.tag == "FluffyMcFlufferBottoms") SceneManager.LoadScene("Death");
    }
}
