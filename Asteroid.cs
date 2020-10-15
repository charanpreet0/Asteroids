using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject asteroidPrefab;
    public GameObject asteroidExplosionPrefab;
    private GameController gameController;
    [SerializeField] private float maxSpeed = 2.5f;
    private float maxX = 10.2f;
    private float maxY = 6.3f;
    private int health = 1;
    private int maxScale = 3;
    public int scale; // size asteroid
    public float childAsteroidOffset = 4f;
    private void Awake()
    {
        scale = maxScale;
        rb = GetComponent<Rigidbody2D>();
        gameObject.name = "Asteroid";
        transform.position = new Vector3(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), 0);

        rb.velocity = Quaternion.Euler(0, 0, Random.Range(0,360)) * new Vector3(Random.Range(0.5f, maxSpeed), 0.0f, 0.0f);
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
        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if (transform.position.x < -maxX)
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

    private void Die()
    {
        GameObject asteroidExplosion = Instantiate(asteroidExplosionPrefab);
        int scaleFactor = maxScale - scale;
        asteroidExplosion.GetComponent<AsteroidExplosion>().SetAudio(0.8f - scaleFactor * 0.25f, 1f + scaleFactor * 0.5f);
        asteroidExplosion.transform.position = transform.position;
        ParticleSystem partSys = asteroidExplosion.GetComponent<ParticleSystem>();
        partSys.Stop();

        var main = partSys.main;
        if((scale < 3) && (scale > 0))
        {
            main.startSize = scale;
        }
        else if (scale == 0)
        {
            main.startSize = 0.05f;
        }
        main.simulationSpeed = 1 * (maxScale - scale + 1);
        partSys.Play();
        

        if(scale > 0)
        {
            spawnChildAsteroids();
            //update number of asteroids(+4)
            gameController.numAsteroids += 4;

        }
        gameController.numAsteroids -= 1;
        Destroy(gameObject);
    }

    private void spawnChildAsteroids()
    {
        Vector2[] newDirection = new Vector2[4];
        newDirection[0] = new Vector2(1, 0);
        newDirection[1] = new Vector2(0, 1);
        newDirection[2] = new Vector2(-1, 0);
        newDirection[3] = new Vector2(0, -1);

        //rand angle

        float randAngle = Random.Range(0, 360);

        for(int i = 0; i < 4; i++)
        {
            GameObject newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.GetComponent<Asteroid>().setGameController(gameController);

            Asteroid asteroidHandle = newAsteroid.GetComponent<Asteroid>();
            newAsteroid.tag = "Asteroid";


            newDirection[i] = Quaternion.Euler(0, 0, randAngle + Random.Range(-30,30)) * newDirection[i];
            newAsteroid.transform.position = transform.position + (Vector3)(newDirection[i] * childAsteroidOffset);
            newAsteroid.transform.localScale = transform.localScale / 2;
            asteroidHandle.scale = scale - 1;
            asteroidHandle.childAsteroidOffset = childAsteroidOffset / 2;

            Rigidbody2D childRb = newAsteroid.GetComponent<Rigidbody2D>();
            childRb.mass = rb.mass / 8;
            childRb.AddForce((Vector3) newDirection[i] * childAsteroidOffset * childAsteroidOffset * childAsteroidOffset * 5);
        }

    }


    public void takeDamage()
    {
        //decrease asteroid health;
        health -= 1;
        if(health == 0)
        {
            Die();
            gameController.IncreaseScore();
        }
    }
}
