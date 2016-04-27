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
            SteamVR_LoadLevel.Begin("MainLevel");
        }

    }

    //Keeps the button from moving in it's local x and y
    private void FreezeLocalPosition()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(buttonRigidBody.velocity);
        localVelocity.x = 0;
        localVelocity.y = 0;
        buttonRigidBody.velocity = transform.TransformDirection(localVelocity);
    }
}
