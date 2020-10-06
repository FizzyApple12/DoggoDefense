using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using EzySlice;
using TMPro;

public class Goal : MonoBehaviour {

    GameObject camera;
    public GameObject text;
    public GameObject box;
    public Material goalMaterial;
    private GameObject[] slices;
    public float spinSpeed;
    private bool slicable = true;
    public int timeToEnd = 240;
    public int levelNum = 1;

    private TimeSpan time;

    void Start() {
        camera = Camera.main.gameObject;
    }

    void Update() {
        if (slicable) text.transform.rotation *= Quaternion.Euler(0, spinSpeed, 0);
        if (!slicable) timeToEnd--;
        if (timeToEnd > 160) camera.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "GOOD";
        else if (timeToEnd <= 160 && timeToEnd > 80) camera.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "JOB";
        else camera.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = StatManager.Instance.stats.time.Minutes + ":" + StatManager.Instance.stats.time.Seconds + "." + StatManager.Instance.stats.time.Milliseconds;
        if (timeToEnd <= 0) SceneManager.LoadScene("Menu");
    }

    public void Hit(Vector3 slicePos, Vector3 sliceDirection) {
        transform.parent = null;
        slices = gameObject.SliceInstantiate(slicePos, sliceDirection, goalMaterial);
        if (slices == null) {
            slices = new GameObject[1]{ gameObject };
        }
        foreach (GameObject slice in slices) {
            slice.transform.position = transform.position;
            slice.AddComponent(typeof(CapsuleCollider));
            slice.AddComponent(typeof(Rigidbody));
            slice.tag = "goallyBoi";
        }
        GetComponent<Renderer>().enabled = false;
        slicable = false;

        camera.transform.parent = null;

        foreach (object obj in GameObject.FindObjectsOfType(typeof(GameObject))) {
            try {
                GameObject go = (GameObject) obj;
                if (go.tag != "goallyBoi" && go.tag != "MainCamera" && go.tag != "endSave") Destroy(go);
            } catch (UnityException e) { }
        }

        Instantiate(box, transform.position, Quaternion.Euler(0, 0, 0));
        camera.transform.GetChild(0).gameObject.SetActive(true);
        camera.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().font = text.GetComponent<TextMeshPro>().font;
        Destroy(text);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "slicyBoi" && slicable) {
            StatManager.Instance.stopRecording(false);
            if (SaveManager.Instance.save.unlocked < levelNum + 1) SaveManager.Instance.save.unlocked = levelNum + 1;
            SaveManager.Instance.saveSave();
            Hit(other.transform.position, other.gameObject.transform.rotation.normalized.eulerAngles);
        }
    }
}
