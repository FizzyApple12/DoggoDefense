using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class DeathDoggo : MonoBehaviour {

    private List<GameObject> slices = new List<GameObject>();
    public GameObject backButton;

    public Material dogMaterial;
    public float maxHeight = 1.5f;
    public float minHeight = -1.5f;

    public int timeToExplode = 120;
    private bool exploded = false;
    public float movement = 0.03f;

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        backButton.SetActive(false);
        StatManager.Instance.stopRecording(true);
    }

    void Update() {
        if (timeToExplode > 0)  {
            timeToExplode--;
            transform.position = new Vector3(Random.Range(-movement, movement), Random.Range(-movement, movement), Random.Range(-movement, movement));
        }

        if (timeToExplode <= 0 && !exploded) {
            Explode();
        }
    }

    void Explode() {
        for (int i = 0; i < transform.childCount; i++) { 
            GameObject[] slicesMade = transform.GetChild(0).gameObject.SliceInstantiate(new Vector3(0, Random.Range(minHeight, maxHeight), 0), new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized, dogMaterial);
            if (slicesMade != null) slices.AddRange(slicesMade);
            else slices.Add(transform.GetChild(0).gameObject);
        }

        gameObject.SetActive(false);

        foreach (GameObject slice in slices) {
            slice.transform.localScale = new Vector3(50, 50, 50);
            slice.transform.position = transform.position;
            slice.AddComponent(typeof(CapsuleCollider));
            slice.AddComponent(typeof(Rigidbody));
        }
        
        backButton.SetActive(true);
        exploded = true;
    }
}
