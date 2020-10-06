using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheEnd : MonoBehaviour {

    public AudioClip endDialog;
    public GameObject text;
    public int countDown = 480;
    private AudioSource audioSource;
    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(endDialog);

        StatManager.Instance.stopRecording(false);
    }

    void Update() {
        if (audioSource.isPlaying) {
            text.SetActive(false);
        } else {
            text.SetActive(true);
            countDown--;
            if (countDown <= 0) {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
