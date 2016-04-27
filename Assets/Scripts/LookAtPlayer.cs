using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour {

    public Transform playerHead;
    //private Vector3 lookAtPos = Vector3.zero;
    private Vector3 lookAtPosLocalized = Vector3.zero;

    

    // Update is called once per frame
    void Update () {

        //lookAtPos = playerHead.position;

        lookAtPosLocalized = this.transform.InverseTransformPoint(playerHead.position);

        

        //lookDir = this.transform.position - playerHead.position;
        this.transform.LookAt(lookAtPosLocalized);
        this.transform.Rotate(new Vector3(0, 90f, 0));
        


    }
}
