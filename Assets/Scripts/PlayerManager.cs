using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public int maxHealth = 10;
    public GameManager gm;
    public Texture hurtTexture;
    public float displayHurtTime = .5f;


    private bool displayHurtEffect = false;
	private ScoreKeeper scorer;
	private UnityStandardAssets.ImageEffects.ScreenOverlay hurtRenderer;

    
    private int currentHealth = 0;

    void Start()
    {
		hurtRenderer = GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay> ();
		hurtRenderer.enabled = false;
        gm = FindObjectOfType<GameManager>();
		scorer = FindObjectOfType<ScoreKeeper> ();
        currentHealth = maxHealth;
    }

    public void TakeDamage()
    {

        currentHealth -= 1;
        if (currentHealth <= 0)
        {
            gm.isGameOver = true;
			scorer.LoseLife (currentHealth);
        }
        if (displayHurtEffect == false)
        {
            displayHurtEffect = true;
			hurtRenderer.enabled = true;
            StartCoroutine(StopHurtEffect());
        }

    }
		

    IEnumerator StopHurtEffect()
    {
        yield return new WaitForSeconds(displayHurtTime);
        displayHurtEffect = false;
		hurtRenderer.enabled = false;

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
