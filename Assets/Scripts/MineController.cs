using UnityEngine;
using System.Collections;

public class MineController : MonoBehaviour {

	public float mineLifeSpan = 2f;
	public GameManager gm;
	public GameObject explosion;
	public bool hasBeenHit;

	private Transform playerHeadTransform;
	private float timeSinceHit = 0f;
	private Vector3 playerDir;
	[SerializeField]
	private float mineSpeed = .5f;




	// Use this for initialization
	void Start () {
		hasBeenHit = false;
		gm = GameObject.FindObjectOfType<GameManager>();

		playerHeadTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();

		if (playerHeadTransform != null)
		{
			//This will direct the bomb to move to the players head minus half a meter down, Thus aiming for his/her body.
			playerDir = (playerHeadTransform.position - new Vector3(0,.2f,0) )- this.transform.position;


		}
		else
		{
			Debug.Log("Player's position not found");
		}

	
	}
	
	// Update is called once per frame
	void Update () {
	

		timeSinceHit += Time.deltaTime;


		if (hasBeenHit)
		{

			Explode();
		}
		if (timeSinceHit >= mineLifeSpan)
		{
			Destroy(gameObject);
		}

	}

	void FixedUpdate()
	{
		this.transform.Translate (playerDir * mineSpeed * Time.deltaTime);
	}


	void OnTriggerEnter(Collider col)
	{
		//If the bomb enters the laser it will be destroyed
		if(col.tag == "Laser")
		{
			Destroy(gameObject);

		}
		if (col.tag == "Player") {
			Explode ();
		}
	}

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
			if (allImpactedColliders[i].tag == "Player")
			{
				allImpactedColliders[i].gameObject.GetComponent<PlayerManager>().TakeDamage();
			}


		}

		Destroy(gameObject);
	}


}
