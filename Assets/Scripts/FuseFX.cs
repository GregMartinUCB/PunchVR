using UnityEngine;
using System.Collections;

public class FuseFX : MonoBehaviour {
    public GameObject fuseFX;

	// Use this for initialization
	void Start () {
        GameObject fuseFxInstance = (GameObject)Instantiate(fuseFX, this.transform.position, Quaternion.identity);
        fuseFxInstance.transform.parent = this.transform;
        fuseFxInstance.transform.localScale = new Vector3(1, 1, 1);


    }
	
	
}
