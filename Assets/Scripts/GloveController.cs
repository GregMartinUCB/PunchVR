using UnityEngine;
using System.Collections;
using Valve.VR;
using System;
using System.IO;


[RequireComponent(typeof(SteamVR_TrackedObject))]
public class GloveController : MonoBehaviour
{

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    public GameManager gameManager;


    private GloveSoundManager gloveSoundManager;
	private bool isMenuDisplayed= false;
	private GameObject restart;
	private GameObject quit;

    //used to position glove correctly
    private Vector3 rightGlovePositionOffset = new Vector3(-0.0883f, 0, -.122f);
    private Vector3 leftGlovePositionOffset = new Vector3(.1f, 0, -.09f);
    private bool hasGloveBeenSet = false;
    private GameObject gloveModel;

	string[] forDebugging = new string[3];
	private bool vectorsRecorded = false;



    void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
		restart = gameManager.restartButton;
		quit = gameManager.quitButton;
            
        //quick test to determine if Calculate Standard Deviation is working

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        //viveControllerModel = GetComponentInChildren<SteamVR_RenderModel>().gameObject;

        device = SteamVR_Controller.Input((int)trackedObj.index);

        //InitializeDisplacementArray();
        //standardDeviation=Vector3.zero;

        gloveSoundManager = GetComponent<GloveSoundManager>();
        //gloveProperties = GetComponent<PhysicsProperties>();



    }


    void FixedUpdate()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (!hasGloveBeenSet)
        {
            //This is to correctly position the gloves to feel like you are wearing boxing gloves
            //The numbers are determined through trial and error.
            if (this.gameObject.tag == "Right")
            {
                gloveModel = GetComponentInChildren<MeshRenderer>().gameObject;
                gloveModel.transform.Rotate(0, 90f, 180f);
                Debug.Log(gloveModel.transform.rotation.eulerAngles);
                gloveModel.transform.localPosition = rightGlovePositionOffset;
                hasGloveBeenSet = true;
            }
            if (this.gameObject.tag == "Left")
            {
                gloveModel = GetComponentInChildren<MeshRenderer>().gameObject;
                gloveModel.transform.Rotate(0, 270f, 0f);
                Debug.Log(gloveModel.transform.rotation.eulerAngles);
                gloveModel.transform.localPosition = leftGlovePositionOffset;
                hasGloveBeenSet = true;
            }
            else
                Debug.Log("Object's Tag does not match requirement to fix glove position");
        }

			
		if (device.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu) && !isMenuDisplayed)
		{
			Invoke ("DisplayMenu", 0.1f);
		}
		if (device.GetPressUp (EVRButtonId.k_EButton_SteamVR_Touchpad)&& !vectorsRecorded) 
		{
			LogDebug (forDebugging);
			vectorsRecorded = true;
		}
		if (device.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu) && isMenuDisplayed) 
		{
			GameObject[] buttons = GameObject.FindGameObjectsWithTag ("Button");
			foreach (GameObject button in buttons) {
				Destroy (button);
			}
			isMenuDisplayed = false;
		}

	}

    

    void OnCollisionEnter(Collision col)
    {
        GameObject collidedObject = col.collider.gameObject;
        Rigidbody collidedRigidBody = collidedObject.GetComponent<Rigidbody>();
        
        if(collidedObject.tag == "Right" || collidedObject.tag == "Left")
        {
            return;
        }
		if (collidedObject.tag == "Mine")
		{
			collidedObject.GetComponent<MineController> ().Explode ();
		}
        if (collidedRigidBody == null)
        {
            Debug.Log("Collision object has no rigid body");
            Debug.Log(collidedObject.name);

        }
		if (collidedRigidBody != null)
        {
            //collision.relativevelocity was not returning a correct value, instead I'll use the addition of
            //the controller's velocity and the object collided with. Also the negative is needed to make the
            //relative velocity be relative to the object hit.
            Vector3 relativeVel = -(device.velocity + collidedRigidBody.velocity);
            

            //Find direction of rebound
            Vector3 forceDir = gameManager.FindForceDir(relativeVel, col.contacts[0].normal);

			//Feed these to the debugger.
			forDebugging [0] = "Device velocity: " + device.velocity.ToString();
			forDebugging [1] = "Relative Velocity: " + relativeVel.ToString();
			forDebugging [2] = "Force Direction: " + forceDir.ToString();
			vectorsRecorded = false;

            //Amplify hit with the moving average squared and an arbitrary scalar.
            //float bonusPower = (float)Math.Pow(movingAvgDisplacement, 2f) * gloveProperties.gloveForceMultiplier;
            float bonusPower = device.velocity.sqrMagnitude;

            //Apply force as an impulse using the normalized direction found with FindForceDir()
            collidedRigidBody.AddForce(forceDir.normalized * bonusPower, ForceMode.Impulse);
            collidedRigidBody.useGravity = true;

            if (collidedObject.tag == "Bomb")
            {
                collidedObject.GetComponent<BombController>().hasBeenHit = true;

            }
            if(collidedObject.tag == "Bag")
            {
				gameManager.StartGame ();

            }

            //Play punch sound clip
            gloveSoundManager.PlayPunchSound();

            //Simulate some haptic feedback
            Rumble();

        }
        

    }
		

	private void LogDebug(params string[] vectorsToPrint)
	{
		File.AppendAllText ("Log.txt", DateTime.Now.ToString()+ "\n");
		for(int i = 0; i< vectorsToPrint.Length; i++)
		{
			
			File.AppendAllText ("Log.txt", vectorsToPrint [i] + "\n");

		}
		File.AppendAllText ("Log.txt", "\n");
	}


    public void Rumble()
    {
        device.TriggerHapticPulse(1000);

    }


	private void DisplayMenu(){
		Instantiate(restart, new Vector3(-0.529f,1.046f,1.333f) , Quaternion.Euler(90f, 180f, 0f));
		Instantiate (quit, new Vector3 (0.5f, 1.046f, 1.333f), Quaternion.Euler (90f, 180f, 0f));
		isMenuDisplayed = true;
	}
}
