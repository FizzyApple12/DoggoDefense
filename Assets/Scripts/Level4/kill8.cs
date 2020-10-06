using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class kill8 : MonoBehaviour {

    public int counter = 0;

    public UnityEvent completed;
    void Start() {

    }

    void Update() {
        
    }

    public void inc() {
        counter++;
        gameObject.SetActive(counter >= 8);
        if (counter >= 8) {
            completed.Invoke();
        }
    }
}
