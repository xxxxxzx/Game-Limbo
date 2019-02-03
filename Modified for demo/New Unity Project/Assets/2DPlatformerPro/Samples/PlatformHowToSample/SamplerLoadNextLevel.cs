using UnityEngine;
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;


namespace PlatformerPro.Extras
{
	public class SamplerLoadNextLevel : MonoBehaviour {

		public string nextLevelName = "";
		public string previousLevelName = "";

		public void LoadNext () {
			#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
			if (nextLevelName != "") SceneManager.LoadScene(nextLevelName);
			#else
			if (nextLevelName != "") Application.LoadLevel (nextLevelName);
			#endif
		}

		public void LoadPrevious () {
			#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
			if (nextLevelName != "") SceneManager.LoadScene(previousLevelName);
			#else
			if (previousLevelName != "") Application.LoadLevel (previousLevelName);
			#endif
		}
	}
}