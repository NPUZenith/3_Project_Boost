using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    // todo: hit lighting bug

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainthrust = 100f;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Living, Dying, Transcending}
    State state = State.Living;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // todo somwhere stop sound on death
        if (state == State.Living)
        {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Living){ return; } // All on one line because why not. Ignores collisions when dead.

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextLevel", 1f); // Parameterize the time
                break;
            default:
                state = State.Dying;
                Invoke("LoadLevel1", 1f); // Parameterize this too
                break;
        }
    }

    private void LoadLevel1()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // Allow for more than 2 levels
    }

    private void Thrust()
    {

        float thrustThisFrame = mainthrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space)) // Allows for thrusting while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying) // So tha it doesn't layer on top of each other
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation

    }
}
