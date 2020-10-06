using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoBehaviour {

    public GameObject player;
    public GameObject dog;
    public GameObject trigger;
    public GameObject enemy;
    public GameObject coverPanel;
    public GameObject Laser;
    public Vector3 startPos;
    public Vector3 idlePos;
    public Vector3 defaultRotation;
    public Vector3 looking;
    public Vector3 otherLooking;
    public Vector3[] enemySpawns;
    private Vector3 ps1;
    private Vector3 ps2;

    private int clip = 1;
    private bool readyForClip = true;
    public AudioClip F1;
    public AudioClip F2;
    public AudioClip F3;
    public AudioClip F4;
    public AudioClip C1;
    public AudioClip C2;
    public AudioClip C3;
    public AudioClip C4;
    public AudioClip C5;
    public AudioClip C6;
    public AudioClip Slam;

    private AudioSource catSound;


    private float raiseStartTime = 0;
    public float raiseTransitionSpeed = 0;
    private bool raiseComplete = true;
    private bool attacking = false;
    private int attack = 0;
    private int attackIntervalMax = 0;
    private int attackWait = 0;
    private System.Random attackRNG = new System.Random();

    public int health = 4;

    private bool deathSequence = false;
    private bool canAttack = false;

    public float lowerTime = 10;

    public bool saved = false;

    private List<GameObject> enemies = new List<GameObject>();

    private bool stoppedMusic = false;
    private bool playedFall = false;

    void Start() {
        if (clip == 0) gameObject.transform.position = startPos;
        else gameObject.transform.position = idlePos;
        gameObject.transform.rotation = Quaternion.Euler(looking);
        catSound = gameObject.GetComponent<AudioSource>();
    }

    void Update() {

        if (!trigger.GetComponent<Trigger>().triggered) return;

        if (!saved) {
            SaveManager.Instance.save.bossVisits++;
            SaveManager.Instance.saveSave();
            saved = true;
        }

        if (deathSequence) {
            if (!stoppedMusic) trigger.GetComponent<AudioSource>().Stop();
            stoppedMusic = true;
            float fractionOfJourney = (Time.time - raiseStartTime) / lowerTime;
            transform.position = Vector3.Lerp(ps1, startPos, fractionOfJourney);
            if (fractionOfJourney >= 0.5) {
                Color color = coverPanel.GetComponent<Image>().color;
                color.a = (((Time.time - raiseStartTime) / lowerTime) - 0.5f) * 2;
                coverPanel.GetComponent<Image>().color = color;
            }
            if (fractionOfJourney >= 1) SceneManager.LoadScene("TheEnd");
            return;
        }

        if (attacking) {
            if (attackWait <= 0) {
                if (attack == -1) attack = attackRNG.Next(0, 3);
                else attack = -1;
                switch (attack) {
                    default:
                        attackWait = attackRNG.Next(0, attackIntervalMax + 1);
                        break;
                    case 0:
                        attackWait = 500;
                        break;
                    case 1:
                        attackWait = 1000;
                        break;
                    case 2:
                        attackWait = 500;

                        foreach (Vector3 point in enemySpawns) {
                            GameObject go = Instantiate(enemy, point, Quaternion.Euler(0, 0, 0));
                            go.GetComponent<Enemy>().player = player;
                            go.GetComponent<Enemy>().dog = dog;
                            enemies.Add(go);
                        }
                        break;
                }
            }
            attackWait--;
            switch (attack) {
                default:
                
                    break;
                case 0:
                    if (attackWait >= 300) {
                        Vector3 relativePos = player.transform.position - transform.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        transform.rotation = Quaternion.Euler(new Vector3(defaultRotation.x, rotation.eulerAngles.y + 90, defaultRotation.z));
                        ps1 = Laser.transform.InverseTransformPoint(dog.transform.position) * 1000;
                        ps2 = dog.transform.position;
                    }

                    if (attackWait <= 200) {
                        Laser.GetComponent<LineRenderer>().SetPosition(1, ps1);
                        RaycastHit hit;
                        if (Physics.Linecast(ps2, Laser.transform.position, out hit)) {
                            if (hit.transform.tag == "doggyBoi" || hit.transform.tag == "blockyBoi") SceneManager.LoadScene("Death");
                        }
                    }

                    if (attackWait <= 1) {
                        Laser.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));
                    }
                    break;
                case 1:
                    if (attackWait >= 800) {
                        Vector3 relativePos = player.transform.position - transform.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        transform.rotation = Quaternion.Euler(new Vector3(defaultRotation.x, rotation.eulerAngles.y + 90, defaultRotation.z));
                        raiseStartTime = Time.time;
                        looking = transform.rotation.eulerAngles;
                        otherLooking = transform.rotation.eulerAngles;
                        otherLooking.z = 82.5f;
                    }

                    if (attackWait <= 800 && attackWait > 600) {
                        canAttack = true;
                        float fractionOfJourney = EaseIn(Time.time - raiseStartTime, 0, 1, 2);
                        transform.rotation = Quaternion.Lerp(Quaternion.Euler(looking), Quaternion.Euler(otherLooking), fractionOfJourney);
                        if (fractionOfJourney >= 0.75) readyForClip = true;
                        if (fractionOfJourney >= 1) {
                            raiseComplete = true;
                            if (!playedFall) {
                                catSound.PlayOneShot(Slam);
                                playedFall = true;
                            }
                        }
                    }

                    if (attackWait <= 600 && attackWait >= 200) {
                        raiseStartTime = Time.time;
                    }

                    if (attackWait <= 200) {
                        canAttack = false;
                        playedFall = false;
                        float fractionOfJourney = EaseInOut(Time.time - raiseStartTime, 0, 1, 1);
                        transform.rotation = Quaternion.Lerp(Quaternion.Euler(otherLooking), Quaternion.Euler(defaultRotation), fractionOfJourney);
                        if (fractionOfJourney >= 0.75) readyForClip = true;
                        if (fractionOfJourney >= 1) raiseComplete = true;
                    }
                    break;
                case 2:

                    break;
            }
        }

        if (!catSound.isPlaying && readyForClip) {
            switch (clip) {
                case -3:
                    attacking = true;
                    break;
                case -2:
                    catSound.PlayOneShot(C3, 1);
                    clip = -3;
                    break;
                case -1:
                    catSound.PlayOneShot(C2, 1);
                    readyForClip = false;
                    raiseComplete = false;
                    raiseStartTime = Time.time;
                    clip = -2;
                    break;
                case 0:
                    catSound.PlayOneShot(C1, 1);
                    clip = -1;
                    break;
                case 1:
                    catSound.PlayOneShot(C4, 1);
                    clip = -3;
                    break;
                case 2:
                    catSound.PlayOneShot(C5, 1);
                    clip = -3;
                    break;
                default:
                    catSound.PlayOneShot(C6, 1);
                    clip = -3;
                    break;
            }
        }

        if (!raiseComplete) {
            float fractionOfJourney = EaseInOut(Time.time - raiseStartTime, 0, 1, raiseTransitionSpeed);
            transform.localPosition = Vector3.Lerp(startPos, idlePos, fractionOfJourney);
            if (fractionOfJourney >= 0.75) readyForClip = true;
            if (fractionOfJourney >= 1) raiseComplete = true;
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.tag == "slicyBoi") {
            if (canAttack) {
                canAttack = false;
                if (attack == 1) attackWait = 201;
                health--;
                switch (health) {
                    case 3:
                        catSound.PlayOneShot(F1);
                        break;
                    case 2:
                        catSound.PlayOneShot(F2);
                        break;
                    case 1:
                        catSound.PlayOneShot(F3);
                        break;
                    case 0:
                        catSound.PlayOneShot(F4);
                        deathSequence = true;
                        attacking = false;
                        foreach (GameObject go in enemies) {
                            try {
                                Destroy(go);
                            } catch (UnityException e) {}
                        }
                        dog.GetComponent<Doggo>().immutable = true;
                        ps1 = transform.position;
                        raiseStartTime = Time.time;
                        break;
                }
            }
        }
    }

    public void OnCollisionEnter(Collision other) {
        
    }

    float EaseIn(float t, float b, float c, float d) {
        return c * Mathf.Pow( 2, 10 * (t/d - 1) ) + b;
    }
    float EaseOut(float t, float b, float c, float d) {
        return c * ( -Mathf.Pow( 2, -10 * t/d ) + 1 ) + b;
    }
    float EaseInOut(float t, float b, float c, float d) {
        t /= d/2;
	    if (t < 1) return c/2 * Mathf.Pow( 2, 10 * (t - 1) ) + b;
	    t--;
	    return c/2 * ( -Mathf.Pow( 2, -10 * t) + 2 ) + b;
    }

}
