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

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InsaneOne.SaveSystem.Dev
{
	[CustomEditor(typeof(SaveSystemData))]
	public sealed class SaveSystemDataEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var prevGuiEnabled = GUI.enabled;
			GUI.enabled = false;
			
			DrawDefaultInspector();
			
			GUI.enabled = prevGuiEnabled;
			
			var saveData = target as SaveSystemData;

			if (!saveData || saveData.AvailablePrefabs == null)
			{
				GUILayout.Label("Warning: select SaveSystemData asset or set available prefabs to continue.");
				return;
			}

			if (GUILayout.Button("Collect Saveable prefabs"))
				CollectPrefabs(saveData);
			
			DrawClearPrefabs(saveData);
			
			DrawNote(saveData);
		
			if (GUILayout.Button("Update hashes"))
				UpdateHashes(saveData);
		}

		void DrawClearPrefabs(SaveSystemData saveData)
		{
			GUILayout.Space(15);

			var prevColor = GUI.color;
			GUI.color = Color.red;
			
			if (GUILayout.Button("Clear empty prefabs"))
				ClearEmptyPrefabs(saveData);

			GUI.color = prevColor;
			
			GUILayout.Space(15);
		}

		void CollectPrefabs(SaveSystemData saveData)
		{
			ClearSceneObjectsFromPrefabs(saveData);
			
			var prefabsGuids = AssetDatabase.FindAssets("t:prefab");

			foreach (var guid in prefabsGuids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
					
				if (!prefab.TryGetComponent<Saveable>(out var saveable) || saveable.IsSceneObject)
					continue;
					
				var exist = saveData.AvailablePrefabs.Any(saveMappedPrefab => saveMappedPrefab.Prefab == prefab);

				if (!exist)
				{
					saveData.AddPrefab(prefab);
					Utility.Log($"Added prefab {prefab} to save system.");
				}
			}
		}

		void ClearSceneObjectsFromPrefabs(SaveSystemData saveData)
		{
			for (var q = saveData.AvailablePrefabs.Count - 1; q >= 0; q--)
			{
				var mappedPrefab = saveData.AvailablePrefabs[q];
				var prefab = mappedPrefab.Prefab;
				
				if (prefab && prefab.TryGetComponent<Saveable>(out var saveable) && saveable.IsSceneObject)
				{
					saveable.EditorSetHash(-1);
					saveData.AvailablePrefabs.Remove(mappedPrefab);
				}
			}
		}

		void ClearEmptyPrefabs(SaveSystemData saveData)
		{
			for (var q = saveData.AvailablePrefabs.Count - 1; q >= 0; q--)
			{
				var mappedPrefab = saveData.AvailablePrefabs[q];

				if (!mappedPrefab.Prefab)
					saveData.AvailablePrefabs.Remove(mappedPrefab);
			}
		}

		void DrawNote(SaveSystemData saveData)
		{
			var warningText = "";
			
			foreach (var mappedPrefab in saveData.AvailablePrefabs)
			{
				if (!mappedPrefab.Prefab)
				{
					warningText += $"No prefab set in one of the mapped prefabs \n";
					continue;
				}
				
				if (!mappedPrefab.Prefab.TryGetComponent<Saveable>(out var saveable))
				{
					warningText += $"Prefab {mappedPrefab.Prefab.name} have no Prefab Saveable component! It is required for the save system.\n";
					continue;
				}
				
				if (saveable.PrefabHashId != mappedPrefab.Id)
					warningText += $"Prefab {mappedPrefab.Prefab.name} has wrong hash set in the Prefab Saveable component! You need to Update Hashes.\n";
			}
			
			if (warningText.Length > 0)
				EditorGUILayout.HelpBox(warningText, MessageType.Warning);
		}

		void UpdateHashes(SaveSystemData ssd)
		{
			foreach (var mappedPrefab in ssd.AvailablePrefabs)
			{
				if (!mappedPrefab.Prefab)
					continue;
				
				if (!mappedPrefab.Prefab.TryGetComponent<Saveable>(out var saveable))
				{
					Utility.Log($"No Saveable component on {mappedPrefab.Prefab}!", LogType.Warning);
					continue;
				}

				saveable.EditorSetHash(mappedPrefab.Id);
				EditorUtility.SetDirty(mappedPrefab.Prefab);
			}
			
			Utility.Log("Hashes update complete!");
		}
	}
}