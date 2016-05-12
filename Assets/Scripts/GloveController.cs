using UnityEngine;
using System.Collections;
using Valve.VR;
using System;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class GloveController : MonoBehaviour
{

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    public GameManager gameManager;


    private GloveSoundManager gloveSoundManager;


    //Variables for determining the moving standard displacement of a controller 
    public float movingAvgDisplacement;
    [SerializeField]
    private float averagingTimeSpan = .1f;
    private Vector3[] displacementPoints;
    private Vector3 standardDeviation;
    private int arrayIndex = 0;
    int lengthOfDisplacementArray;

   

    //used to position glove correctly
    private Vector3 rightGlovePositionOffset = new Vector3(-0.0883f, 0, -.122f);
    private Vector3 leftGlovePositionOffset = new Vector3(.1f, 0, -.09f);
    private bool hasGloveBeenSet = false;
    private GameObject gloveModel;



    void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
            
        //quick test to determine if Calculate Standard Deviation is working
        CalculateDeviationTest();

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

        //The standard deviation as a method of reducing the "Wii syndrome" isn't well optimized and may be removed.
        //RecordPosition();
        //standardDeviation = CalculateDeviation(displacementPoints);
        //movingAvgDisplacement = standardDeviation.magnitude;
        




       

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

            Debug.Log(collidedObject.name);


            //collision.relativevelocity was not returning a correct value, instead I'll use the addition of
            //the controller's velocity and the object collided with. Also the negative is needed to make the
            //relative velocity be relative to the object hit.
            Vector3 relativeVel = -(device.velocity + collidedRigidBody.velocity);
            

            //Find direction of rebound
            Vector3 forceDir = gameManager.FindForceDir(relativeVel, col.contacts[0].normal);
            Debug.Log("Average Displacement of glove: "+movingAvgDisplacement);
            Debug.Log("Direction of force: "+forceDir);

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
                gameManager.isStarted = true;

            }

            //Play punch sound clip
            gloveSoundManager.PlayPunchSound();

            //Simulate some haptic feedback
            Rumble();

        }
        

    }


    //Initializes the Displacement Array with length determined by the rate
    //and total time for averaging. Then initializes all elements with the zero vector.
    private void InitializeDisplacementArray()
    {
        lengthOfDisplacementArray = (int)(averagingTimeSpan / Time.fixedDeltaTime);

        displacementPoints = new Vector3[lengthOfDisplacementArray];

        for (int i = 0; i < lengthOfDisplacementArray; i++)
        {
            displacementPoints[i] = Vector3.zero;
        }
    }


    //TODO: There must be a better way to do this
    //This will loop through the displacementPoints array at a rate defined by timeBetweenPoints
    //each time it loops it will overwrite one of the array elements with the current position
    //This will have displacementPoints only keep lengthOfDisplacementArray number of latest entries
    private void RecordPosition()
    {
        displacementPoints[arrayIndex] = this.transform.position;
        arrayIndex++;
        //resets the index to 0
        if (arrayIndex >= lengthOfDisplacementArray)
        {
            arrayIndex = 0;
        }
        
    }


    public void Rumble()
    {
        device.TriggerHapticPulse(1000);

    }

    //Used to calculate the standard deviation from an array of vector3 points
    //returns a vector3 of the standard deviation for each axis.
    //This is an attempt to avoid what I call "Wii syndrome." short little
    //bursts of high velocity (read flick of wrist) shouldn't work.
    private Vector3 CalculateDeviation(Vector3[] points)
    {
        Vector3 stdDeviation = Vector3.zero;
        Vector3 avg = Vector3.zero;

        for(int i = 0; i < points.Length;i++)
        {
            avg.x += points[i].x;
            avg.y += points[i].y;
            avg.z += points[i].z;
        }

        avg = avg / points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            stdDeviation.x += Mathf.Pow((points[i].x - avg.x), 2);
            stdDeviation.y += Mathf.Pow((points[i].y - avg.y), 2);
            stdDeviation.z += Mathf.Pow((points[i].z - avg.z), 2);

        }

        stdDeviation = stdDeviation / (points.Length-1);
        stdDeviation.x = Mathf.Sqrt(stdDeviation.x);
        stdDeviation.y = Mathf.Sqrt(stdDeviation.y);
        stdDeviation.z = Mathf.Sqrt(stdDeviation.z);

        

        return stdDeviation;

    }

    //Test for CalculateDeviation Method, each axis should equal 1.29099
    //The test will have a tolerance of +/-0.001
    private void CalculateDeviationTest()
    {
        Vector3[] testArray = new[] {  new Vector3(0f,0f,0f),
                                       new Vector3(1f,1f,1f),
                                       new Vector3(2f,2f,2f),
                                       new Vector3(3f,3f,3f)};

        Vector3 resultSTD = CalculateDeviation(testArray);
        if(resultSTD.x > 1.292 || resultSTD.x < 1.290)
        {
            Debug.LogError("CalculateDeviation Test Failed. Error greater than +/-0.001");
        }

    }

}
