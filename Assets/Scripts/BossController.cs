using UnityEngine;
using System.Collections;
using System;

public class BossController : MonoBehaviour {

    public GameManager gameManager;
    public GameObject player;
    public float maxHealth = 100f;
    private float currentHealth = 0f;


    //Variables for Bombs
    public GameObject bomb;
    public float maxBombSpawnTime = 3f;
    public float scoreModifier = 50f;
    private float timeSinceBombSpawn = 0f;
    private float currentBombSpawnTime;
    private int whichEyeSpawnBomb = 0;

	//Variables for mine
	public GameObject mine;
	[SerializeField]
	private float delayBeforeMine = 10;


    //Variables for boss positioning
    private bool destinationReached = false;
    private float startingHeight = 1f;
    private float ascensionSpeed = 2f;
    private SpringJoint recoilSpring;

    //Variables for Bloodshot eye effect
    private Transform[] eyeTransform;
    private ChangeEyeMaterial[] eyeChangers;
    public bool testEyes = false;

    //Variables for Laser
    public GameObject laser;
    public GameObject laserChargeEffect;
	public GameObject laserOutline;
    public float timeBetweenLaser = 10f;
    private float timeSinceLastLaser = 0f;
    private ParticleSystem charging;
    private bool isCharging = false;
    private ChangeMouth mouthChanger;

    void Awake()
    {
        currentHealth = maxHealth;
        currentBombSpawnTime = maxBombSpawnTime;
        //There should only ever be one game manager because it is a singleton nopt destroyed on load.
        gameManager = FindObjectOfType<GameManager>();
        
    }

    void Start()
    {
        eyeChangers = GetComponentsInChildren<ChangeEyeMaterial>();

        eyeTransform = new Transform[2];
        GetEyeTransforms();

        charging = laserChargeEffect.GetComponent<ParticleSystem>();
        mouthChanger = GetComponentInChildren<ChangeMouth>();


    }

  

    void FixedUpdate()
    {
        //Game has started, boss needs to appear infront of player
        if (gameManager.isStarted && !destinationReached)
        {
            Ascend();
        }

        
        if (gameManager.isStarted && destinationReached && !gameManager.isGameOver)
        {
           

            timeSinceBombSpawn += Time.deltaTime;
			delayBeforeMine -= Time.deltaTime;
            if (timeSinceBombSpawn > currentBombSpawnTime)
            {
				if (delayBeforeMine > 0) {
					//Shoot bomb, reset counter till next bomb and re-evaluate what the spawn rate should be.
					ShootBomb ();

				} else
				{
					int mineOrBomb = UnityEngine.Random.Range (0, 2);
					if (mineOrBomb == 0)
					{
						ShootBomb ();
					} else
					{
						ShootMine ();
					}
				}
            }
            timeSinceLastLaser += Time.deltaTime;
            if (timeSinceLastLaser > timeBetweenLaser - (charging.duration + 1f) && !isCharging)
            {
                ChargeLaser();
                isCharging = true;
            }

            if (timeSinceLastLaser > timeBetweenLaser)
            {
                ShootLaser();
                StartCoroutine(mouthChanger.ChangeToNormalMouth());
                timeSinceLastLaser = 0f;
                
                
            }
        }

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }


        if (testEyes)
        {
            TestEyeMaterialChange();
        }
    }

    private void ChargeLaser()
    {
        Instantiate(laserChargeEffect, this.transform.position + new Vector3(-6f, .9f, 0), Quaternion.identity);
		Instantiate(laserOutline, this.transform.position + new Vector3(0,.9f,0), Quaternion.Euler(new Vector3(-90f, 180f, 0)));
        mouthChanger.ChangeToLaserMouth();
    }

    private void ShootLaser()
    {
        Instantiate(laser, this.transform.position + new Vector3(0,.9f,0), Quaternion.Euler(new Vector3(-90f, 180f, 0)));
        isCharging = false;
    }

    //This method adjusts the timing between bomb spawns based on the score.
    //As the score approaches infinity the new spawn time will be 0.5 sec between bombs.
    //at score = (scoreModifier^2)/4 the time between bombs will be halfway between max and min.
    private float DetermineSpawnTime()
    {
        float scoreAdjustedTime = Mathf.Sqrt(gameManager.score);
        scoreAdjustedTime /= (scoreModifier/2f + Mathf.Sqrt(gameManager.score));

        float newSpawnTime = maxBombSpawnTime - (maxBombSpawnTime - 0.5f) * scoreAdjustedTime;

        return newSpawnTime;

    }

    //A method designed to test whether the eyes are changing by making testEyes true
    //in editor during game.
    private void TestEyeMaterialChange()
    {

        foreach (ChangeEyeMaterial eye in eyeChangers)
        {
            eye.MakeEyesBloodShot();
        }
        
    }   
    

    public void TakeDamage(float projectileVelocity)
    {

        //By scaling the damage based on the projectile speed there should be an incentive
        //to try and hit as hard as possible.
        currentHealth -= 1f * projectileVelocity;
        gameManager.IncreaseScore(scoreModifier);

        //Made this scalable in case I want a multi-eyed boss. Most likely overcoding though.
        foreach (ChangeEyeMaterial eye in eyeChangers)
        {
            eye.MakeEyesBloodShot();
        }



    }   


    //Rise up and intimidate the player
    private void Ascend()
    {
        transform.Translate(Vector3.up*Time.fixedDeltaTime * ascensionSpeed);
        if(transform.position.y >= startingHeight)
        {
            destinationReached = true;
            


        }

    }


    private void ShootBomb()
    {
        //whichEyeSpawnBomb%2 will alternate between 0 and 1 making which eye the bomb spawn from alternate.
        GameObject bombInstance = (GameObject)Instantiate(bomb, eyeTransform[whichEyeSpawnBomb%2].position, Quaternion.identity);
        whichEyeSpawnBomb++;

        Rigidbody bombRB = bombInstance.GetComponent<Rigidbody>();

        bombRB.isKinematic = true;

		timeSinceBombSpawn = 0f;
		currentBombSpawnTime = DetermineSpawnTime ();

    }
	private void ShootMine()
	{
		//whichEyeSpawnBomb%2 will alternate between 0 and 1 making which eye the bomb spawn from alternate.
		GameObject mineInstance = (GameObject)Instantiate(mine, eyeTransform[whichEyeSpawnBomb%2].position, Quaternion.identity);
		whichEyeSpawnBomb++;

		//Might be able to remove this and just set isKinematic in the Prefab
		Rigidbody mineRB = mineInstance.GetComponent<Rigidbody>();
		//mineRB.isKinematic = true;

		timeSinceBombSpawn = 0f;
		currentBombSpawnTime = DetermineSpawnTime ();

	}


    //This method finds the transforms of the eyes. These transforms will be used to position bombs.
    private void GetEyeTransforms()
    {

        Transform[] childrenTransforms = GetComponentsInChildren<Transform>();
        int count = 0;
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            if (childrenTransforms[i].tag == "Eye")
            {
                //Incase more eyes are found than expected
                if (count < 2)
                {
                    eyeTransform[count] = childrenTransforms[i];
                    count++;
                    
                }
                else
                {
                    Debug.LogError("Incorrect number of eyes. Number of eyes found: " + count);
                }
                
            }
        }
        
    }
}
