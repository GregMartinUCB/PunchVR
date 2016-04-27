using UnityEngine;
using System.Collections;
using System;

public class BossController : MonoBehaviour {

    public GameManager gameManager;
    public GameObject player;
    public float maxHealth = 100f;
    public GameObject bomb;
    public float scoreModifier = 50f;

    private float currentHealth = 0f;
    private bool destinationReached = false;
    private float startingHeight = 3f;
    private float ascensionSpeed = 2f;
    private SpringJoint recoilSpring;
    [SerializeField]
    private float bombSpawnTime = 1f;

    private ChangeEyeMaterial[] eyeChangers;
   
   
    private float timeSinceBombSpawn = 0f;

    void Awake()
    {
        currentHealth = maxHealth;
        eyeChangers = GetComponentsInChildren<ChangeEyeMaterial>();
        //There should only ever be one game manager because it is a singleton nopt destroyed on load.
        gameManager = FindObjectOfType<GameManager>();
        
    }

    void FixedUpdate()
    {
        if (gameManager.isStarted && !destinationReached)
        {
            Ascend();
        }
        if(gameManager.isStarted && destinationReached && !gameManager.isGameOver)
        {
            timeSinceBombSpawn += Time.deltaTime;
            if (timeSinceBombSpawn > bombSpawnTime)
            {
                ShootBomb();
                timeSinceBombSpawn = 0f;
            }
        }
        
        if(currentHealth <= 0f)
        {
            Destroy(gameObject);
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
        GameObject bombInstance = (GameObject)Instantiate(bomb, this.transform.position, Quaternion.identity);

        Rigidbody bombRB = bombInstance.GetComponent<Rigidbody>();

        bombRB.isKinematic = true;


    }

}
