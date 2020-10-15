using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Spaceship : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    private float turnSpeed = 180;
    private float thrust = 1f;
    private Vector3 shipDirection = new Vector3(0, 1, 0);
    private Rigidbody2D rb;
    private float maxX = 9.2f;
    private float maxY = 5.3f;
    private float maxSpeed = 2.0f;
    private float bulletSpeed = 20f;
    private GameController gameController;


    //audio stuff
    private AudioSource audioSource;
    public AudioClip shootingSoundFX;
    public AudioClip thrusterFX;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        gameObject.tag = "Spaceship";
        gameObject.name = "Spaceship";
        //set Random position
        transform.position = new Vector3(Random.Range(-maxX + 2f, maxX - 2f), Random.Range(-maxY + 2f, maxY - 2f), 0);
    }

    public void setGameController(GameController _gameController)
    {
        gameController = _gameController;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float turnAngle;
        if (Input.GetKey("j"))
        {
            //turn left
            turnAngle = turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);
            shipDirection = Quaternion.Euler(0, 0, turnAngle) * shipDirection;
        }
        if (Input.GetKey("l"))
        {
            //turn right
            turnAngle = -turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);
            shipDirection = Quaternion.Euler(0, 0, turnAngle) * shipDirection;
        }
        if (Input.GetKey("k"))
        {
            rb.AddForce(shipDirection * thrust);
        }
        if (Input.GetKeyDown("k"))
        {
            audioSource.clip = thrusterFX;
            audioSource.Play();


        }
        if (Input.GetKeyUp("k"))
        {
            audioSource.clip = thrusterFX;
            audioSource.Stop();
        }

        if (Input.GetKeyDown("space"))
        {
            audioSource.PlayOneShot(shootingSoundFX);
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation * Quaternion.Euler(0,0,90);
            bullet.GetComponent<Rigidbody2D>().velocity = shipDirection * bulletSpeed;
        }

        //throttle speed
        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        //Screen Wrapping
        if(transform.position.x < -maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y);
        }
        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(-maxX, transform.position.y);
        }
        if (transform.position.y < -maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY);
        }
        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, -maxY);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Asteroid")
        {
            gameController.timeDied = Time.time;
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
