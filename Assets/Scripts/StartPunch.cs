using UnityEngine;
using System.Collections;

public class StartPunch : MonoBehaviour {

    public GameManager gameManager;
    private float cleanUpTimer = 5f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();


    }

    void Update()
    {
        if (gameManager.isStarted)
        {
            cleanUpTimer -= Time.deltaTime;
            if(cleanUpTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }





}
