using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour {
    
    public Vector3[] positions;
    public Vector3[] rotations;
    public GameObject[] menuButtons;

    private List<bool> visible = new List<bool>();

    public float transitionSpeed;

    public int current = 0;
    public int target = 0;

    private float startTime;

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        visible.Add(true);
        for (int i = 1; i < positions.Length; i++) {
            visible.Add(false);
        }
        startTime = Time.time;
        transform.position = positions[current];
        transform.rotation = Quaternion.Euler(rotations[current]);
        SaveManager.Instance.loadSave();
    }

    void Update() {
        if (current != target) {
            float fractionOfJourney = EaseOut(Time.time - startTime, 0, 1, transitionSpeed);
            transform.position = Vector3.Lerp(positions[current], positions[target], fractionOfJourney);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(rotations[current]), Quaternion.Euler(rotations[target]), fractionOfJourney);
            if (fractionOfJourney >= 1) {
                current = target;
            }
            if (fractionOfJourney >= 0.5) {
                visible[current] = false;
                visible[target] = true;
            }
        } else {
            startTime = Time.time;
        }

        foreach (GameObject go in menuButtons){
            MenuButton button = go.GetComponent<MenuButton>();
            go.SetActive(visible[button.menuGroup] && button.minLevel <= SaveManager.Instance.save.unlocked);
        }
    }

    public void Menu() { Change(0); }
    public void Levels() { Change(1); }
    public void Credits() { Change(2); }

    public void Change(int to) {
        current = target;
        startTime = Time.time;
        target = to; 
    }
    public void Quit() { Application.Quit(); }

    public float EaseOut(float t, float b, float c, float d) {
        return c * (-Mathf.Pow(2, -10 * t/d) + 1) + b;
    }
}
