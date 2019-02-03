#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	[CustomEditor(typeof(PowerUpManager), false)]
	public class PowerUpManagerInspector : Editor
	{

		/// <summary>
		/// Stores visibility for each repsonse type.
		/// </summary>
		protected Dictionary<PowerUpResponse, bool> responseVisibility;

		/// <summary>
		/// Stores visibility of the reset response item.
		/// </summary>
		protected bool resetResponseVisibility;

		/// <summary>
		/// Cached and typed target reference.
		/// </summary>
		protected PowerUpManager myTarget;

		/// <summary>
		/// Cached list of response names.
		/// </summary>
		protected List<string> responseNames;

		/// <summary>
		/// Draw the GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox ("Note that many movement based power-ups can now be handled using the PowerUpActiveCondition.", MessageType.Info);
			// Unity says we don't need to do this, but if we don't do this then serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "PowerUp Update");
			myTarget = (PowerUpManager)target;
			if (responseVisibility == null) {
				responseVisibility = new Dictionary<PowerUpResponse, bool> ();
			}
			if (myTarget.responses == null)
			{
				myTarget.responses = new List<PowerUpResponse>();
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			// Add new actions
			if (GUILayout.Button("Add PowerUp Type"))
			{
				if (myTarget.responses == null)
				{
					myTarget.responses = new List<PowerUpResponse>();
					myTarget.responses.Add (new PowerUpResponse());
				}
				else
				{
					myTarget.responses.Add (new PowerUpResponse());
				}
			}
			EditorGUILayout.EndHorizontal();

			// Cache response names
			if (responseNames == null) responseNames = new List<string>();
			if (myTarget.responses.Count != (responseNames.Count - 1)) 
			{
				responseNames = new List<string>();
				responseNames.Add (" - NONE - ");
				foreach (PowerUpResponse response in myTarget.responses)
				{
					responseNames.Add (response.type);
				}
			}

			if (myTarget.responses != null)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);
				EditorGUILayout.BeginVertical();
				RenderResetResponse();
				for (int i = 0; i < myTarget.responses.Count; i++)
				{
					if (!responseVisibility.ContainsKey(myTarget.responses[i])) responseVisibility.Add (myTarget.responses[i], false);
					RenderResponse(myTarget.responses[i]);
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}

		}

		virtual protected void RenderResponse (PowerUpResponse response) {
			responseVisibility[response] = EditorGUILayout.Foldout(responseVisibility[response], response.type);
			if(responseVisibility[response])
			{

				GUILayout.BeginVertical(EditorStyles.textArea);
				GUILayout.Space(5);
				string type = EditorGUILayout.TextField(new GUIContent("PowerUp Type", "Type of the PowerUp."), response.type);
				if (type != response.type)
				{
					response.type = type;
					responseNames = new List<string>();
					responseNames.Add (" - NONE - ");
					foreach (PowerUpResponse r in myTarget.responses)
					{
						responseNames.Add (r.type);
					}
				}

				bool isReset = EditorGUILayout.Toggle(new GUIContent("Is Reset", "Is this a reset for another power up?"), response.isReset);
				if (isReset != response.isReset)
				{
					response.isReset = isReset;
				}

				if (!response.isReset)
				{
					bool resetOnDamage = EditorGUILayout.Toggle(new GUIContent("Reset on Damage", "Should the power up be removed if the character is damaged."), response.resetOnDamage);
					if (resetOnDamage != response.resetOnDamage)
					{
						response.resetOnDamage = resetOnDamage;
					}

					int timer = EditorGUILayout.IntField(new GUIContent("PowerUp Timer", "Time the PowerUp is active for (use 0 for unlimited)."), response.time);
					if (timer < 0) timer = 0;
					if (timer != response.time)
					{
						response.time = timer;
					}
					// Show resets if timer > 0
					if (timer > 0.0f || resetOnDamage)
					{
						int originalResetIndex = 0;
						if (response.powerUpReset != null) originalResetIndex = responseNames.IndexOf(response.powerUpReset);
						int selectedResetIndex = originalResetIndex;
						GUILayout.BeginHorizontal();
						selectedResetIndex = EditorGUILayout.Popup("Reset Response", originalResetIndex, responseNames.ToArray());
						if (originalResetIndex != selectedResetIndex)
						{
							response.powerUpReset = responseNames[selectedResetIndex];
						}
						if (selectedResetIndex <= 0 && GUILayout.Button ("Create Reset"))
						{
							selectedResetIndex = 0;
							PowerUpResponse resetResponse = new PowerUpResponse();
							resetResponse.type = "Reset " + response.type;
							resetResponse.isReset = true;
							myTarget.responses.Add (resetResponse);
							response.powerUpReset = resetResponse.type;
						}
						GUILayout.EndHorizontal();
						if (selectedResetIndex <= 0)
						{
							selectedResetIndex = 0;
							EditorGUILayout.HelpBox("Power up is set as resetabble but no reset is provided (this could be valid in some cases).", MessageType.Info);
						}

						if (response.powerUpReset == response.type)
						{
							EditorGUILayout.HelpBox("Power-up is set to reset itself. This is probably not desired.", MessageType.Warning);
						}
					}
				}

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				// Add new actions
				if (GUILayout.Button("Add Action"))
				{
					if (response.actions == null)
					{
						response.actions = new EventResponse[1];
					}
					else
					{
						// Copy and grow array
						EventResponse[] tmpActions = response.actions;
						response.actions = new EventResponse[tmpActions.Length + 1];
						System.Array.Copy(tmpActions, response.actions, tmpActions.Length);
					}
				}
				if (GUILayout.Button("Remove PowerUp Type"))
				{
					myTarget.responses.Remove (response);
				}

				EditorGUILayout.EndHorizontal();

				if (response.actions != null)
				{

					for (int i = 0; i < response.actions.Length; i++)
					{
						EditorGUILayout.BeginVertical ("HelpBox");
						
						GUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						if (i == 0) GUI.enabled = false;
						if (GUILayout.Button ("Move Up", EditorStyles.miniButtonLeft))
						{
							EventResponse tmp = response.actions[i-1];
							response.actions[i-1] = response.actions[i];
							response.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						if (i == response.actions.Length - 1) GUI.enabled = false;
						if (GUILayout.Button ("Move Down", EditorStyles.miniButtonRight))
						{
							EventResponse tmp = response.actions[i+1];
							response.actions[i+1] = response.actions[i];
							response.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						// Remove
						GUILayout.Space(4);
						bool removed = false;
						if (GUILayout.Button("Remove", EditorStyles.miniButton))
						{
							response.actions = response.actions.Where (a=>a != response.actions[i]).ToArray();
							removed = true;
						}
						GUILayout.EndHorizontal ();
						if (!removed) 	EventResponderInspector.RenderAction(target, response, response.actions[i]);
						EditorGUILayout.EndVertical();
					}

				}
				GUILayout.EndVertical();
			}
		}

		virtual protected void RenderResetResponse () {

			resetResponseVisibility = EditorGUILayout.Foldout(resetResponseVisibility, "RESET");
			if(resetResponseVisibility)
			{
				GUILayout.BeginVertical(EditorStyles.textArea);
				GUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				// Add new actions
				if (GUILayout.Button("Add Action"))
				{
					if (myTarget.resetResponse.actions == null)
					{
						myTarget.resetResponse.actions = new EventResponse[1];
					}
					else
					{
						// Copy and grow array
						EventResponse[] tmpActions = myTarget.resetResponse.actions;
						myTarget.resetResponse.actions = new EventResponse[tmpActions.Length + 1];
						System.Array.Copy(tmpActions, myTarget.resetResponse.actions, tmpActions.Length);
					}
				}

				EditorGUILayout.EndHorizontal();
				
				if (myTarget.resetResponse.actions != null)
				{
					
					for (int i = 0; i < myTarget.resetResponse.actions.Length; i++)
					{
						EditorGUILayout.BeginVertical ("HelpBox");
						
						GUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						if (i == 0) GUI.enabled = false;
						if (GUILayout.Button ("Move Up", EditorStyles.miniButtonLeft))
						{
							EventResponse tmp = myTarget.resetResponse.actions[i-1];
							myTarget.resetResponse.actions[i-1] = myTarget.resetResponse.actions[i];
							myTarget.resetResponse.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						if (i == myTarget.resetResponse.actions.Length - 1) GUI.enabled = false;
						if (GUILayout.Button ("Move Down", EditorStyles.miniButtonRight))
						{
							EventResponse tmp = myTarget.resetResponse.actions[i+1];
							myTarget.resetResponse.actions[i+1] = myTarget.resetResponse.actions[i];
							myTarget.resetResponse.actions[i] = tmp;
							break;
						}
						GUI.enabled = true;
						// Remove
						GUILayout.Space(4);
						bool removed = false;
						if (GUILayout.Button("Remove", EditorStyles.miniButton))
						{
							myTarget.resetResponse.actions = myTarget.resetResponse.actions.Where (a=>a != myTarget.resetResponse.actions[i]).ToArray();
							removed = true;
						}
						GUILayout.EndHorizontal ();
						if (!removed) EventResponderInspector.RenderAction(target, myTarget.resetResponse, myTarget.resetResponse.actions[i]);
						EditorGUILayout.EndVertical();
					}
					
				}

				EditorGUILayout.EndVertical();
			}

		}

	
	}
}