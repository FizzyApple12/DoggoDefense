using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour {

    public GameObject[] platforms;
    public GameObject goal;
    public GameObject enemy;
    public GameObject player;
    public GameObject dog;
    public int kills = 0;
    public int killGoal = 0;
    private System.Random platRNG = new System.Random();

    public float spawnDelay = 480;
    public float spawnTimer = 0;
    
    public int moving = 1;
    private float movePercent = 0;
    public float moveSpeed = 0;

    void Start() {
        
    }

    void Update() {
        spawnTimer--;
        if (spawnTimer <= 0) {
            spawnTimer = spawnDelay;
            int spawnAt = platRNG.Next(0, platforms.Length-1);
            Vector3 spawnLoc = platforms[spawnAt].transform.position;
            spawnLoc += new Vector3(0, 5, 0);
            GameObject go = Instantiate(enemy, spawnLoc, Quaternion.Euler(0, 0, 0));
            go.GetComponent<Enemy>().player = player;
            go.GetComponent<Enemy>().dog = dog;
            go.GetComponent<Enemy>().speed = 0;
            go.GetComponent<Enemy>().playerKill.AddListener(RegisterKill);
        }

        Vector3 position = platforms[moving].transform.position;
        position.y = Mathf.Sin(2 * Mathf.PI * movePercent + Mathf.PI / 2) * 9 - 9;
        platforms[moving].transform.position = position;
        movePercent += moveSpeed;
        if (movePercent >= 1) {
            position.y = 0;
            platforms[moving].transform.position = position;
            moving = platRNG.Next(0, platforms.Length-1);
            movePercent = 0;
        }
    }

    void RegisterKill() {
        kills++;
        goal.SetActive(kills >= killGoal);
    }
}
