using UnityEngine;
using System.Collections;

public class ChangeMouth : MonoBehaviour {

    public MeshRenderer laserMouth;

    private MeshRenderer normalMouth;

	// Use this for initialization
	void Start () {
        normalMouth = GetComponent<MeshRenderer>();

	
	}

    //A method to be called by the boss controller which switches which mesh renderer is enabled.
    public void ChangeToLaserMouth()
    {
        laserMouth.enabled = true;
        normalMouth.enabled = false;

    }

    //This will be used in a coroutine inside Bosscontroller. This will keep the laser effect past when
    //The shootlaser function is called.
    public IEnumerator ChangeToNormalMouth()
    {
        yield return new WaitForSeconds(1);
        laserMouth.enabled = false;
        normalMouth.enabled = true;

    }

}
