using UnityEngine;
using System.Collections;

public class Sound{
	private static AudioClip error = Resources.Load ("Sound/error") as AudioClip;
	private static AudioClip button = Resources.Load ("Sound/button") as AudioClip;
	private static AudioClip perfect = Resources.Load ("Sound/perfect") as AudioClip;
	private static AudioClip chat = Resources.Load ("Sound/chat") as AudioClip;

	public static void Error(){
		AudioSource.PlayClipAtPoint (error, Vector3.zero);
	}
	public static void Button(){
		AudioSource.PlayClipAtPoint (button, Vector3.zero);
	}
	public static void Perfect(){
		AudioSource.PlayClipAtPoint (perfect, Vector3.zero);
	}
	public static void Chat(){
		AudioSource.PlayClipAtPoint (chat, Vector3.zero);
	}
}
