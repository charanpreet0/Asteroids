using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    public int maxAsteroids = 2;
    public int numAsteroids;
    private int maxLives = 4;
    private int numLivesLeft;
    private float respawnTime = 3f;
    public float timeDied;
    private GameObject spaceship;
    private GameObject gameOverSign;
    private GameObject levelClearedSign;
    public GameObject [] lifeIcons;
    public GameObject asteroidPrefab;
    public GameObject spaceshipPrefab;
    private bool gameFinished = false;
    private bool gameWon = false;
    private float finishTime;
    private int myScore = 0;
    private Score scoreText;
    [SerializeField] private float minCollisionRadius = 2.0f;
    private void Awake()
    {
        numLivesLeft = maxLives;
        myScore = 0;
        scoreText = FindObjectOfType<Score>();
        scoreText.UpdateScore(myScore);

        gameOverSign = GameObject.Find("GameOver");
        levelClearedSign = GameObject.Find("LevelCleared");
        InitializeLevel();

    }

    private void InitializeLevel()
    {
        numAsteroids = maxAsteroids;
        //spawn the asteroids
        for(int i = 0; i < numAsteroids; i++)
        {
            spawnAsteroid();

        }
        //spawnAsteroid();
        spawnSpaceship();

        Assert.IsNotNull(gameOverSign);
        gameOverSign.SetActive(false);

        Assert.IsNotNull(levelClearedSign);
        levelClearedSign.SetActive(false);
        gameFinished = false;
        gameWon = false;
    }

    private void spawnAsteroid()
    {
        bool valid;
        GameObject newAsteroid;
        do
        {
            newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.GetComponent<Asteroid>().setGameController(this);
            newAsteroid.gameObject.tag = "Asteroid"; 
            valid = CheckTooCloseToAsteroid(newAsteroid);

        } while (valid == false);

        return;
    }

    private void spawnSpaceship()
    {
        bool valid;
        Assert.IsNull(spaceship);
        do
        {
            spaceship = Instantiate(spaceshipPrefab);
            spaceship.gameObject.tag = "Spaceship";
            valid = CheckTooCloseToAsteroid(spaceship);
        } while (valid == false);
        spaceship.GetComponent<Spaceship>().setGameController(this);
        numLivesLeft -= 1;

        return;
    }

    public void IncreaseScore()
    {
        myScore += 10;
        scoreText.UpdateScore(myScore);
    }



    private bool CheckTooCloseToAsteroid(GameObject testObject)
    {
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach(GameObject asteroid in asteroids)
        {
            if(asteroid != testObject)
            {
                //Check if they are too close together
                if (Vector3.Distance(testObject.transform.position, asteroid.transform.position) < minCollisionRadius) 
                {
                    Destroy(testObject);
                    return false;
                }
            }
        }
        return true;

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if( spaceship == null)
        {
            if(Time.time - timeDied > respawnTime)
            {
                if (numLivesLeft > 0)
                {

                    spawnSpaceship();

                    //update life icons
                    Destroy(lifeIcons[numLivesLeft]);
                }
                else
                {
                    gameOverSign.SetActive(true);

                }

            }
            //check to see if spaceship has died

        }

        // check to see if I won
        if((numAsteroids == 0) && (gameWon == false))
        {
            if (gameFinished)
            {
                if(Time.time - finishTime > respawnTime)
                {
                    levelClearedSign.SetActive(true);
                    gameFinished = false;
                    gameWon = true;
                    StartCoroutine(Pause());
                }

            }
            else
            {
                gameFinished = true;
                finishTime = Time.time;
            }
            levelClearedSign.SetActive(true);
        }
        Assert.IsTrue(numAsteroids >= 0);
    }
    IEnumerator Pause()
    {
        yield return new WaitForSeconds(3f);
        maxAsteroids = maxAsteroids * 2;

        if(maxAsteroids > 16)
        {
            maxAsteroids = 16;
        }
        Destroy(spaceship);
        spaceship = null;
        numLivesLeft++;
        InitializeLevel();
    }
}
