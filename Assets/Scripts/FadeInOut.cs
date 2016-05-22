using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public float fadeTime;

	public IEnumerator FadeOut (AudioSource source)
	{
		while (source.volume > 0f) {
			source.volume -= Time.deltaTime/fadeTime;
			yield return null;
		}

		yield return new WaitForSeconds (.1f);


	}

	public IEnumerator FadeIn (AudioSource source, AudioClip newClip, float maxVolume)
	{
		source.volume = 0f;
		source.clip = newClip;
		source.Play ();

		while (source.volume < maxVolume) {
			source.volume += Time.deltaTime / fadeTime;
			yield return null;
		}
		yield return new WaitForSeconds (.1f);
	}

}
