using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class StatManager : MonoBehaviour {

    public static StatManager Instance { get; private set; }
    public GameObject player;
    public Stats stats;
    bool recording = false;

    //RECORDED
    
    Stopwatch time;
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        
    }

    public void startRecording(GameObject go) {
        player = go;
        time = new Stopwatch();
        time.Start();
        recording = true;
    }

    public void stopRecording(bool discard) {
        time.Stop();
        recording = false;
        if (discard) return;
        stats = new Stats();
        stats.time = time.Elapsed;
    }
}
