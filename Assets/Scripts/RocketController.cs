using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour {

    [SerializeField]
    private float force_Amount = 50f, rotate_Amount = 100f;

    [SerializeField]
    private AudioClip rocket_Sound, win_Sound, lose_Sound;

    [SerializeField]
    private ParticleSystem rocket_Particle, win_Particle, lose_Particle;

    [SerializeField]
    private float transition_Time = 2f;

    private enum State { WIN, LOSE, ALIVE }
    private State playerState = State.ALIVE;

    private AudioSource audioSource;
    private Rigidbody myBody;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        myBody = GetComponent<Rigidbody>();
    }

    void Update() {
        if(playerState == State.ALIVE) {
            HandleRotate();
        }
    }

    void FixedUpdate() {
        if (playerState == State.ALIVE) {
            HandleMovement();
        }
    }

    void HandleRotate() {

        myBody.freezeRotation = true;

        float rotationThisFrame = rotate_Amount * Time.deltaTime;

        if(Input.GetAxisRaw("Horizontal") < 0) {

            transform.Rotate(Vector3.forward * rotationThisFrame);


        } else if (Input.GetAxisRaw("Horizontal") > 0) {

            transform.Rotate(-Vector3.forward * rotationThisFrame);


        }

        myBody.freezeRotation = false;


    }

    void HandleMovement() { 

        if(Input.GetKey(KeyCode.Space)) {

            myBody.AddRelativeForce(Vector3.up * force_Amount);

            if(!audioSource.isPlaying) {
                audioSource.PlayOneShot(rocket_Sound);
            }

            rocket_Particle.Play();

        } else {
            rocket_Particle.Stop();
        }

    }

    void LevelFinished() {
        playerState = State.WIN;
        audioSource.Stop();
        audioSource.PlayOneShot(win_Sound);
        win_Particle.Play();

        Invoke("LoadNextLevel", transition_Time);
    }

    void PlayerDied() {
        playerState = State.LOSE;
        audioSource.Stop();
        audioSource.PlayOneShot(lose_Sound);
        lose_Particle.Play();

        Invoke("RestartLevel", transition_Time);
    }

    void LoadNextLevel() {
        int currentScene_Plus_NextScene = SceneManager.GetActiveScene().buildIndex;
        currentScene_Plus_NextScene += 1;
        int count = SceneManager.sceneCountInBuildSettings;

        if(currentScene_Plus_NextScene == count) {
            SceneManager.LoadScene(0);
        } else {
            SceneManager.LoadScene(currentScene_Plus_NextScene);
        }

    }

    void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnCollisionEnter(Collision target) {

        if (playerState != State.ALIVE)
            return;

        switch(target.gameObject.tag) {

            case "Friendly":
                break;

            case "Finish":
                LevelFinished();
                break;

            default:
                PlayerDied();
                break;

        }

    }

} // class



































