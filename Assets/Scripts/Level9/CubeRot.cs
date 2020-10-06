using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRot : MonoBehaviour {
    public int direction = 0;
    public int delay = 240;

    public bool auto = false;

    public float rotSpeed = 1;
    System.Random delayRNG = new System.Random();
    void Start() {
        
    }

    void Update() {
        switch (direction) {
            case 1:
                transform.rotation *= Quaternion.Euler(rotSpeed, 0, 0);
                break;
            case 2:
                transform.rotation *= Quaternion.Euler(-rotSpeed, 0, 0);
                break;
            case 3:
                transform.rotation *= Quaternion.Euler(0, 0, rotSpeed);
                break;
            case 4:
                transform.rotation *= Quaternion.Euler(0, 0, -rotSpeed);
                break;
        }

        if (auto) {
            delay--;
            if (delay <= 0) {
                delay = delayRNG.Next(60, 241);
                direction = delayRNG.Next(1, 5);
            }
        }
    }

    public void Set(int dir) {
        direction = dir;
    }
    public void Auto(bool on) {
        auto = on;
    }
}
