using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

    private int level = 0;
    public int kills = 0;
    private float animationTime = 0;
    public float animationSpeed = 0;
    private float pushTime = 0;
    public float pushSpeed = 0;
    private Vector3 to;
    private Vector3 from;
    public GameObject[] pushers;

    private bool moving = false;
    private bool pushing = false;
    public bool okToMove = false;

    private float pb = 15;
    private float pe = 5;
    void Start() {
        Push();
    }

    void Update() {
        if (moving) {
            float fractionOfJourney = (Time.time - animationTime) / animationSpeed;
            transform.position = Vector3.Lerp(from, to, fractionOfJourney);
            if (fractionOfJourney >= 1) {
                moving = false;
                Push();
            }
        }
        if (pushing) {
            float fractionOfJourney = (Time.time - pushTime) / pushSpeed;
            pushers[level].transform.position = Vector3.Lerp(new Vector3(pushers[level].transform.position.x, pushers[level].transform.position.y, pb), new Vector3(pushers[level].transform.position.x, pushers[level].transform.position.y, pe), fractionOfJourney);
            if (fractionOfJourney >= 1) {
                pushing = false;
                okToMove = true;
            }
        }

        if (okToMove) {
            switch (kills) {
                case 1:
                    Move();
                    okToMove = false;
                    break;
                case 3:
                    Move();
                    okToMove = false;
                    break;
                case 7:
                    Move();
                    okToMove = false;
                    break;
                case 15:
                    Move();
                    okToMove = false;
                    break;
            }
        }
    }

    void Move() {
        animationTime = Time.time;
        from = new Vector3(0, level * 10, 0);
        level++;
        to = new Vector3(0, level * 10, 0);
        moving = true;
    }
    void Push() {
        pushTime = Time.time;
        pushing = true;
    }
    public void Defeat() {
        kills++;
    }
}
