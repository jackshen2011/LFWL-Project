using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏启动类，负责加载场景主要对象，并防止重复加载
/// </summary>
public class GameLoader : MonoBehaviour {

	/// <summary>
	/// SceneControl里的MainRoot对象
	/// </summary>
	private static MainRoot GL_MAINROOT;
	/// <summary>
	/// BKCamera里的Camera
	/// </summary>
	private static Camera GL_BKCAMERA;
	/// <summary>
	/// RoomCardNetClientModule里的RoomCardNeDevice
	/// </summary>
	//private static RoomCardNeDevice GL_NET;
	/// <summary>
	/// MusicPlayer里的AudioListenerCtrl
	/// </summary>
	private static AudioListenerCtrl GL_AUDIOLIST;
	/// <summary>
	/// SoundEffectAudioSource里的AudioSource
	/// </summary>
	private static AudioSource GL_AUDIOSOURCE;

	void Start ()
	{
		try
		{
			GameObject temp;

			if (GL_BKCAMERA == null)
			{
				temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/BKCamera"));
				GL_BKCAMERA = temp.GetComponent<Camera>();
			}
			if (GL_MAINROOT == null)
			{
				temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/SceneControl"));
				GL_MAINROOT = temp.GetComponent<MainRoot>();
			}
		}
		catch (System.Exception e)
		{

			throw;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
