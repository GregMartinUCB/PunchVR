using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float lifeSpan = 10f;
    private float secondsLived = 0f;

    void Update()
    {
        secondsLived += Time.deltaTime;

        if (secondsLived >= lifeSpan)
            Destroy(gameObject);

    }

}
