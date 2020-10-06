using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//[RequireComponent(typeof(Collider))]
public class MenuButton : MonoBehaviour {

    public int menuGroup = 0;
    public int minLevel = 0;

    public int type = 0;
    public string scene = "";

    public UnityEvent interactEvent;

    void Start() {

    }

    void Update() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
         
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == gameObject) {
                switch (type) {
                    case 0:
                        interactEvent.Invoke();
                        break;
                    case 1:
                        SceneManager.LoadScene(scene);
                        break;
                }
            }
        }
    }
}
