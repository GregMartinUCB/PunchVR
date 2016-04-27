using UnityEngine;
using Valve.VR;
using System.Collections;

public class BagController : MonoBehaviour {

    public bool hasBeenHit = false;

    private Rigidbody bagRigidBody;
    private GloveSoundManager punchSound;

    void Start()
    {

        bagRigidBody = GetComponent<Rigidbody>();


    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Glove")
        {
           
            bagRigidBody.useGravity = true;
            hasBeenHit = true;
            //Gives it an impulse. This makes the player feel stong and the starting punching bag to "Team Rocket Away"
            float bonusForce = col.collider.gameObject.GetComponent<PhysicsProperties>().gloveForceMultiplier;

            //Determine speed of punch, use to increase speed of bag.

            Vector3 punchSpeed = col.collider.gameObject.GetComponent<PhysicsProperties>().gloveSpeed;
            bagRigidBody.AddForce(col.impulse.normalized * bonusForce * punchSpeed.magnitude, ForceMode.Impulse);

            //Play punch sound clip
            punchSound = col.collider.gameObject.GetComponent<GloveSoundManager>();
            punchSound.PlayPunchSound();

            //Simulate some haptic feedback
            if (col.collider.gameObject.GetComponentInParent<GloveController>() != null)
            {
                col.collider.gameObject.GetComponentInParent<GloveController>().Rumble();
            }
            else
            {
                Debug.Log("Glove's parent does not have a GloveController Script");
            }
        }

    }

}
