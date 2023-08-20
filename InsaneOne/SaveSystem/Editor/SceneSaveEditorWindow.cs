/*
 * Copyright 2023 Oleg Dzhuraev <godlikeaurora@gmail.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InsaneOne.SaveSystem.Dev
{
	public sealed class SceneSaveEditorWindow : EditorWindow
	{
		/// <summary> Temporary list to store tool handled results. </summary>
		readonly List<Saveable> saveablesWithNoId = new List<Saveable>();

		string sceneSetupCheckResult;

		Vector2 scrollPos;
		
		SceneSaveHolder saveHolder;
		
		[MenuItem("Tools/InsaneOne/Save System")]
		static void ShowWnd()
		{
			var wnd = GetWindow<SceneSaveEditorWindow>();
			wnd.titleContent = new GUIContent("Save System");
		}

		void OnGUI()
		{
			DrawActualSave();
			DrawTools();
		}

		void DrawTools()
		{
			GUI.enabled = !Application.isPlaying;
			
			GUILayout.Label("Tools", EditorStyles.boldLabel);

			if (!saveHolder)
			{
				var saveBehaviourOnScene = FindObjectOfType<SceneSaveHolder>();

				if (saveBehaviourOnScene)
				{
					saveHolder = saveBehaviourOnScene;
				}
				else
				{
					EditorGUILayout.HelpBox("No Save Behaviour on scene! Save system will not work. Create?", MessageType.Warning);

					if (GUILayout.Button("Create Save Behaviour"))
						saveHolder = SceneSaveHolder.Get();
				}

				return;
			}
			
			var msgType = MessageType.Info;
			
		
			// todo objects with same IDs!
			if (string.IsNullOrEmpty(sceneSetupCheckResult))
				EditorGUILayout.HelpBox("Scene state info is not actual. Run scene check.", MessageType.Warning);
	
			if (GUILayout.Button("Check scene Saveables setup"))
			{
				sceneSetupCheckResult = "";

				var sceneSaveables = WorldSaver.GetSaveablesFromScene();

				foreach (var saveable in sceneSaveables)
					if (saveable.IsSceneObject && saveable.SceneUniqueId < 0)
					{
						sceneSetupCheckResult += $"Object {saveable.name} have no scene ID!\n";

						if (sceneSetupCheckResult.Length > 1000)
						{
							sceneSetupCheckResult += "...";
							break;
						}
					}

				if (string.IsNullOrEmpty(sceneSetupCheckResult))
					sceneSetupCheckResult = "Scene Saveable objects are setup correctly.";
			}

			if (!string.IsNullOrEmpty(sceneSetupCheckResult))
			{
				if (!sceneSetupCheckResult.Contains("correctly"))
					msgType = MessageType.Warning;
				
				EditorGUILayout.HelpBox($"Scene check result:\n{sceneSetupCheckResult}", msgType);
			}

			if (GUILayout.Button("Set scene ids"))
				MarkSceneSaveables();
		}

		void DrawActualSave()
		{
			var saves = WorldSaver.GetActualSave();

			GUILayout.Label("Actual save", EditorStyles.boldLabel);
			
			if (saves.Count <= 0)
			{
				EditorGUILayout.HelpBox("When game in runtime and some save was made, there will be shown its data.",
					MessageType.None);

				GUILayout.Space(10);
				return;
			}
			
			scrollPos = GUILayout.BeginScrollView(scrollPos);

			foreach (var save in saves)
			{
				var text = $"Is Scene Object: {save.IsSceneObject}";
				
				if (save.IsSceneObject)
					text += $", SceneId: {save.SceneInstanceId}";
				else 
					text += $", PrefabHash: {save.MappedPrefabId}";
				
				text += $"\nSave data: {save.SerializedData}";
				
				EditorGUILayout.HelpBox(text, MessageType.None);
			}

			GUILayout.EndScrollView();
			
			GUILayout.Space(10);
		}

		void MarkSceneSaveables()
		{
			var sceneSaveables = WorldSaver.GetSaveablesFromScene();

			saveablesWithNoId.Clear();

			foreach (var saveable in sceneSaveables)
				if (saveable.IsSceneObject && saveable.SceneUniqueId < 0)
					saveablesWithNoId.Add(saveable);

			var saveBeh = SceneSaveHolder.Get();
			foreach (var saveable in saveablesWithNoId)
				saveBeh.SceneSaveables.Add(saveable);
			
			EditorUtility.SetDirty(saveBeh);
			
			sceneSetupCheckResult = "";
			
			Utility.Log("All scene saveables were marked.");
		}
	}
}