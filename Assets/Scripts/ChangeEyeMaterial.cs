using UnityEngine;
using System.Collections;

public class ChangeEyeMaterial : MonoBehaviour {

    public Material bloodShotEye;
    public Material normalEye;

    private MeshRenderer eyeMeshRend;
    private float blootShotLength = 1f;


    void Start()
    {
        eyeMeshRend = GetComponent<MeshRenderer>();

    }

    //Public method to make the boss's eyes display a blood shot effect, signaling damage
    //This method then calls the MakeEyesNormal Method as a coroutine to set the eyes back
    //to normal after a set length of time.
    public void MakeEyesBloodShot()
    {
        eyeMeshRend.materials[1] = bloodShotEye;
        StartCoroutine( MakeEyesNormal());

    }

    IEnumerator MakeEyesNormal()
    {
        yield return new WaitForSeconds(blootShotLength);
        eyeMeshRend.materials[1] = normalEye;

    }


}
