using UnityEngine;
using System.Collections;
using System;

public class BombController : MonoBehaviour {

    public float bombLifeSpan = 2f;
    public GameManager gm;
    public GameObject explosion;
    public bool hasBeenHit;

    private float timeSinceHit = 0f;
    private Rigidbody bombRB;
    private GloveSoundManager punchSound;
    [SerializeField]
    private AudioClip explosionSound;
    [SerializeField]
    private float bombSpeed = 3f;
    private Vector3 playerDir = Vector3.zero;
    private Transform playerHeadTransform;

    void Start () {

        hasBeenHit = false;
        bombRB = GetComponent<Rigidbody>();
        gm = GameObject.FindObjectOfType<GameManager>();

        //There should only be one object with the tag MainCamera, so this method of instantiating should
        //not cause a problem.
        playerHeadTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
	
	}
	
	void Update () {

        if (hasBeenHit)
        {
            timeSinceHit += Time.deltaTime;
            
        }

        if (timeSinceHit >= bombLifeSpan)
        {

            Explode();
        }
	
	}

    void FixedUpdate()
    {
        if (!hasBeenHit)
        {
            if (playerHeadTransform != null)
            {
                playerDir = playerHeadTransform.position - this.transform.position;
                this.transform.Translate(playerDir.normalized * Time.deltaTime * bombSpeed);

            }
            else
            {
                Debug.Log("Player's position not found");
            }
        }
        
        if(playerDir.magnitude <= 0.2)
        {
            Explode();
        }
        if (gm.isGameOver)
        {
            Destroy(gameObject);
        }

    }


    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Boss" && hasBeenHit)
        {
            GameObject boss = col.gameObject;
            boss.GetComponent<BossController>().TakeDamage(1f);

            //bombRB.AddExplosionForce(1000, this.transform.position, 10f);

            Explode();


        }
    }

    //Instatiates a explosion particle effect and sound then destroys the bomb gameobject
    public void Explode()
    {
        Instantiate(explosion, this.transform.position, this.transform.rotation);

        AudioSource explosionSoundSource = GetComponent<AudioSource>();
        
        explosionSoundSource.Play();

        //Checks to see if a player was in the explosion
        Collider[] allImpactedColliders = Physics.OverlapSphere(this.transform.position, 1.5f);

        for (int i = 0; i < allImpactedColliders.Length; i++)
        {
            //The hmd has the tag MainCamera, so if the tag matches that have the player take damage!
            if (allImpactedColliders[i].tag == "MainCamera")
            {
                allImpactedColliders[i].gameObject.GetComponent<PlayerManager>().TakeDamage();
            }

           
        }

        Destroy(gameObject);
    }

   
}
