using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public float fadeTime;
	private bool isOff = false;


	public IEnumerator SwitchSong (AudioSource source, AudioClip newClip, float maxVolume)
	{
		while (source.volume > 0f && !isOff) {
			source.volume -= Time.deltaTime/fadeTime;
			yield return null;
		}

		if (source.volume <= 0 && !isOff) {
			isOff = true;
			source.clip = newClip;
			source.Play ();
		}
		while (source.volume < maxVolume && isOff) {
			source.volume += Time.deltaTime / fadeTime;
			yield return null;
		}
		if (source.volume >= maxVolume && isOff) {

			isOff = false;

		}

		yield return null;
	}

}
