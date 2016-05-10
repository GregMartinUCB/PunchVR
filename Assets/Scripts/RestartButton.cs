using UnityEngine;
using System.Collections;
using System;

public class RestartButton : MonoBehaviour {

    public GameManager gameManager;
    public BossController bossManager;

    private Vector3 startPos;
    private float distantPushed;
    private Rigidbody buttonRigidBody;
    

    // Use this for initialization
    void Start () {

        startPos = this.transform.position;
        buttonRigidBody = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        //should be only one boss, however he is not setup as a singleton so multiple could exist.
        bossManager = FindObjectOfType<BossController>();
        
	
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        FreezeLocalPosition();
        
        distantPushed = (this.transform.position - startPos).magnitude;
        
        
        if (distantPushed > .2f)
        {
            gameManager.ResetStartConditions();
            //Stops the button from moving too far.
            buttonRigidBody.velocity = Vector3.zero;
            gameManager.RestartLevel();
            //Destroy(gameObject);
            Destroy(this.transform.parent.gameObject);
        }

    }

    //Keeps the button from moving in it's local x and y
    private void FreezeLocalPosition()
    {
        //The button shouldn't be moving in the global y direction at anytime, unless it is rotated.
        buttonRigidBody.velocity = new Vector3(buttonRigidBody.velocity.x, 0f, buttonRigidBody.velocity.z);
        //Get the local velocity
        Vector3 localVelocity = transform.InverseTransformDirection(buttonRigidBody.velocity);
        localVelocity.x = 0;
        localVelocity.y = 0;
        //Convert the local velocity to a global one.
        buttonRigidBody.velocity = transform.TransformDirection(localVelocity);
    }

}
