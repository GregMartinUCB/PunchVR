using UnityEngine;
using System.Collections;

public class PhysicsProperties : MonoBehaviour {

    public float gloveForceMultiplier = 50f;
    public Vector3 gloveSpeed = Vector3.zero;

    private SteamVR_TrackedObject trackedObject;

    void FixedUpdate()
    {
        //set the track object variable after glove has been picked up
        if(this.transform.parent != null && trackedObject == null)
        {
            trackedObject = GetComponentInParent<SteamVR_TrackedObject>();

        }
        //Unsure why this is used, part of the Steam VR plugin
        if (trackedObject != null)
        {
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)trackedObject.index);
            
            //Set Velocity as a public vector3 for easy use elsewhere
            gloveSpeed = device.velocity;

        }


    }

}
