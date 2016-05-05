using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public int maxHealth = 10;
    public GameManager gm;
    public Texture hurtTexture;
    public float displayHurtTime = .5f;


    private bool displayHurtEffect = false;

    
    private int currentHealth = 0;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {

        currentHealth -= 1;
        if (currentHealth <= 0)
        {
            gm.isGameOver = true;
        }
        if (displayHurtEffect == false)
        {
            displayHurtEffect = true;
            StartCoroutine(StopHurtEffect());
        }

    }

    void OnGUI()
    {
        if (displayHurtEffect == true)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hurtTexture, ScaleMode.StretchToFill);
        }
    }

    IEnumerator StopHurtEffect()
    {
        yield return new WaitForSeconds(displayHurtTime);
        displayHurtEffect = false;

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Laser")
        {
            gm.isGameOver = true;
        }
        else
        {
            return;
        }

    }

}
