using UnityEngine;
using System.Collections;

public class SetKinematics : MonoBehaviour {

    Collider[] objectsNearGlove;



    void FixedUpdate()
    {

        objectsNearGlove = Physics.OverlapSphere(this.transform.position, 0.2f);

        for(int i = 0; i < objectsNearGlove.Length; i++)
        {
            if (objectsNearGlove[i].GetComponent<Rigidbody>())
            {
                objectsNearGlove[i].GetComponent<Rigidbody>().isKinematic = false;
            }
        }


    }


}
