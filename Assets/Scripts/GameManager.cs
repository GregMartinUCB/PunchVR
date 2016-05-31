using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using Random = UnityEngine.Random;
using System.IO;

public class GameManager : MonoBehaviour {


    public bool isStarted = false;
    public bool isGameOver = false;
    public GameObject bomb;
    public GameObject bombSpawnPoint;
    public GameObject gameOverSign;
    public GameObject restartButton;
	public GameObject quitButton;
    public float bombSpawnTime = 1f;
	public bool isGameOverDisplayed = false;
	public AudioClip activeSong;
	public AudioClip passiveSong;
	public AudioSource music;
	public FadeInOut fader;

	private ScoreKeeper scorer;
    private float timeSinceBomb = 0f;
    private bool spawnClear = false;
    private Vector3 spawnSpot;
    //Creates a singleton
    //****************************************************
    private static GameManager _instance = null;
    

    public static GameManager Instance
	{
		get{return _instance;}
	}

	void Awake(){
		if (_instance != null && _instance != this) {
			Destroy (gameObject);
			return;
		} else {
			_instance = this;

		}
		DontDestroyOnLoad (this.gameObject);
		gameObject.name = "$GameManager";

	}

    //*****************************************************


    void Start()
    {
		music = GetComponent<AudioSource> ();
		scorer = FindObjectOfType<ScoreKeeper> ();
		fader = GetComponent<FadeInOut> ();

        FindForceDirTest();
		music.Play ();
    }

	void Update()
	{
		if (isStarted && music.clip != activeSong) 
		{
			StartCoroutine(fader.SwitchSong (music, activeSong, 0.75f));

		}
   
    }

	public void StartGame ()
	{
		StartCoroutine(fader.SwitchSong (music, activeSong, 0.75f));
		isStarted = true;

	}

    //This method was used to generate bombs infront of the player. This method has been rendered obsolete
    //by programming the boss to shoot bombs at the player. This however is useful for prototyping.
    private void SelfSpawnBombs()
    {
        //First generate the spawn point and check if it is clear;
        if (spawnClear == false)
        {
            spawnSpot = GenerateSpawnPoint();
            spawnClear = IsSpawnClear(spawnSpot);
        }

        timeSinceBomb += Time.deltaTime;
        if (timeSinceBomb >= bombSpawnTime && spawnClear)
        {

            SpawnBomb();
            timeSinceBomb = 0f;
            spawnClear = false;
        }

    }

    private Vector3 GenerateSpawnPoint()
    {
        Vector3 spawnOffset = Random.insideUnitCircle / 2;
        //The factor of 2 for the y offset is because we don't want the player having to reach down to punch.
        Vector3 spawnPoint = bombSpawnPoint.transform.position + new Vector3(0, spawnOffset.y/2, spawnOffset.x);

        return spawnPoint;
    }

    private bool IsSpawnClear(Vector3 checkPoint)
    {
        //Generate array of all colliders in the spawn space
        Collider[] spaceCheck = Physics.OverlapSphere(checkPoint, 0.15f);

        //if no colliders space if safe to spawn
        if (spaceCheck == null)
        {
            return true;
        }
        //if colliders are found check for rigid body
        else
        {
            bool hasRB = false;
            foreach (Collider col in spaceCheck)
            {
                //if a rigidbody is found do not spawn
                if (col.gameObject.GetComponent<Rigidbody>() != null)
                {
                    hasRB = true;

                }
            }
            //Case where a collider was found but no rigid body, e.g. player's collider
            if (!hasRB)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }

    private void SpawnBomb()
    {
       
        Instantiate(bomb, spawnSpot, Quaternion.identity);
        
    }


    //Calculates the rebound direction of a collision, collision.impulse was not working properly
    //uses formula: V_f = V - 2(V*n)n
    public Vector3 FindForceDir(Vector3 relativeVelocity, Vector3 normal)
    {

        Vector3 forceDir = -2f * Vector3.Dot(relativeVelocity, normal.normalized) * normal.normalized;
        forceDir += relativeVelocity;
        return forceDir;
    }

    //Tests FindForceDir Method, FindForceDir should return (1,1,0)
    //Without loss of generality we used only 2 dimensions for the test
    private void FindForceDirTest()
    {
        Vector3 relativeVel = new Vector3(1, -1, 0);
        Vector3 testnormal = new Vector3(0, 1, 0);
        Vector3 result = FindForceDir(relativeVel, testnormal);
        if (result != new Vector3(1, 1, 0))
        {
            Debug.LogError("FindForceDirTest has Failed");
        }
    }

	public void DisplayGameOver()
	{
		//These values for positioning the game over sign and restart button are hard coded unfortunately
		//A better design would make them appear based on player position/rotation.
		Instantiate (gameOverSign, new Vector3 (4.34f, 1.86f, 0), Quaternion.Euler (0, -90f, 0));
		Instantiate (restartButton, new Vector3 (-0.529f, 1.046f, 1.333f), Quaternion.Euler (90f, 180f, 0f));
		Instantiate (quitButton, new Vector3 (0.5f, 1.046f, 1.333f), Quaternion.Euler (90f, 180f, 0f));
	}

    //Functions to reset the boolean logic and restart the game
    public void ResetStartConditions()
    {
        scorer.scoreRenderer.enabled = false;
        isStarted = false;
        isGameOver = false;
        isGameOverDisplayed = false;
		scorer.score = 0f;
		scorer.scoreText.fontSize = 150;
		music.clip = passiveSong;

		scorer.ResetLives ();

    }
    public void RestartLevel()
    {
        StartCoroutine(RestartDelay());
       
    }
    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(1f);
        SteamVR_LoadLevel.Begin("MainLevel");
    }
}
