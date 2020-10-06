using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using TMPro;

public class Player : MonoBehaviour {

    public GameObject head;
    public GameObject sword;
    public GameObject dog;
    public GameObject canvas;
    public GameObject announcement;

    public bool TCOMM_canvasActive = false;
    public string TCOMM_announcementText = "";
    public bool TCOMM_announcing = false;
    private bool dogMoved = false;
    private bool dogMoving = false;
    private float dogStartTime;

    public Vector3 dogPosStart;
    public Vector3 dogPosEnd;
    public Vector3 dogRotStart;
    public Vector3 dogRotEnd;
    public float dogTransitionSpeed;

    public float speed = 1.0f;
    public float jumpSpeed = 0.1f;
    public float maxVelocityChange = 10.0f;
    private bool touchingFloor = true;

    public float rotationX = 0.0f;
    public float rotationY = 0.0f;
    public float lookSpeed = 0.0f;

    public Vector3 swordRestingPos;
    public Vector3 swordRestingRot;
    public Vector3[] swordSwingPos;
    public Vector3[] swordSwingRot;
    public int swordResetTime;
    public float swordSwingSpeed;
    public float swordResetSpeed;
    private bool swordSwingAllow = true;
    public bool swordSwinging = false;
    private bool swordResetting = false;
    private Vector3 swordAnimatingFromPos;
    private Vector3 swordAnimatingFromRot;
    private int swordTarget = -1;
    private int swordResetTimer = 0;
    private float swordStartTime = 0;
    private System.Random swordRng = new System.Random();

    public bool paused = false;
    public AudioClip swing;
    private bool previousFramePaused = false;
    Rigidbody rigidBody;

    void Start() {
        rigidBody = gameObject.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = true;

        dog.transform.localPosition = dogPosStart;
        dog.transform.localRotation = Quaternion.Euler(dogRotStart);
        Announce("DONT LET HIM DIE", 500, true);

        sword.transform.localPosition = swordRestingPos;
        sword.transform.localRotation = Quaternion.Euler(swordRestingRot);

        StatManager.Instance.startRecording(gameObject);
    }

    void Update() {
        canvas.SetActive(TCOMM_canvasActive);
        announcement.GetComponent<TextMeshProUGUI>().text = TCOMM_announcementText;

        if (!dogMoved && dogMoving) {
            float fractionOfJourney = EaseOut(Time.time - dogStartTime, 0, 1, dogTransitionSpeed);
            dog.transform.localPosition = Vector3.Lerp(dogPosStart, dogPosEnd, fractionOfJourney);
            dog.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(dogRotStart), Quaternion.Euler(dogRotEnd), fractionOfJourney);
            if (fractionOfJourney >= 1) {
                dogMoved = true;
                dogMoving = false;
            }
        } else {
            dogStartTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) paused = !paused;

        if (paused != previousFramePaused) {
            previousFramePaused = paused;
            if (paused) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (!TCOMM_announcing) {
            TCOMM_announcementText = "PAUSED";
            TCOMM_canvasActive = paused;
        }

        head.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        if (paused) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;

        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	    targetVelocity = transform.TransformDirection(targetVelocity);
	    targetVelocity *= speed;
	    Vector3 velocity = rigidBody.velocity;
	    Vector3 velocityChange = (targetVelocity - velocity);
	    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
	    velocityChange.y = 0;
	    rigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
	    if (touchingFloor && Input.GetButtonDown("Jump")) {
	        rigidBody.velocity = velocity + jumpSpeed * transform.up;
            touchingFloor = false;
	    }


        if (swordSwingAllow && Input.GetMouseButtonDown(0)) {
            sword.GetComponent<AudioSource>().PlayOneShot(swing);
            swordTarget++;
            if (swordTarget >= swordSwingPos.Length) swordTarget = 0;

            swordStartTime = Time.time;
            swordSwinging = true;
            swordResetting = false;
            swordSwingAllow = false;
            swordAnimatingFromPos = sword.transform.localPosition;
            swordAnimatingFromRot = sword.transform.localRotation.eulerAngles;
        }

        if (swordSwinging) {
            float fractionOfJourney = EaseOut(Time.time - swordStartTime, 0, 1, (swordResetting) ? swordResetSpeed : swordSwingSpeed);
            sword.transform.localPosition = Vector3.Lerp(swordAnimatingFromPos, (swordResetting) ? swordRestingPos : swordSwingPos[swordTarget] , fractionOfJourney);
            sword.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(swordAnimatingFromRot), (swordResetting) ? Quaternion.Euler(swordRestingRot) : Quaternion.Euler(swordSwingRot[swordTarget]), fractionOfJourney);
            if (fractionOfJourney >= 0.8) swordSwingAllow = true;
            if (fractionOfJourney >= 1) {
                swordSwinging = false;
                swordResetting = !swordResetting;
                swordResetTimer = 0;
            }
        }

        if (swordResetting && !swordSwinging) {
            swordResetTimer++;
            if (swordResetTimer >= swordResetTime) {
                swordSwinging = true;
                swordAnimatingFromPos = sword.transform.localPosition;
                swordAnimatingFromRot = sword.transform.localRotation.eulerAngles;
                swordTarget = -1;
            }
        }

    }

    public void Announce(string message, int delay, bool unpause) {
        Thread announceThread = new Thread(new ParameterizedThreadStart(T_Announce));
        announceThread.Start(new AnnouncementData(message, delay, unpause));
    }
    public void Announce(string message) {
        Thread announceThread = new Thread(new ParameterizedThreadStart(T_Announce));
        announceThread.Start(new AnnouncementData((string) message, 500, false));
    }

    public void T_Announce(object message) {
        TCOMM_announcing = true;
        AnnouncementData data = (AnnouncementData) message;
        string[] words = data.message.Split(' ');
        TCOMM_canvasActive = true;
        foreach (string word in words) {
            TCOMM_announcementText = word;
            Thread.Sleep(data.delay);
        }
        TCOMM_canvasActive = false;
        if (!dogMoved) dogMoving = true;
        if (data.unpause) paused = false;
        TCOMM_announcing = false;
    }

    public float EaseOut(float t, float b, float c, float d) {
        return c * (-Mathf.Pow(2, -10 * t/d) + 1) + b;
    }

    public void OnCollisionEnter(Collision other) {
        touchingFloor = true;
        if (other.gameObject.tag == "spicyBoi") SceneManager.LoadScene("Death");
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "spicyBoi") SceneManager.LoadScene("Death");
    }
}
