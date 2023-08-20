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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InsaneOne.SaveSystem
{
	public static class WorldSaver
	{
		/// <summary> You can provide your custom serializer here. </summary>
		public static ISaveSerializer Serializer { get; set; } = new JsonSaveSerializer();
		/// <summary> You can provide your custom object spawner here, if you need custom spawn logic. </summary>
		public static ISaveSpawner Spawner { get; set; }  = new DefaultSpawner();

		/// <summary> Fires when world loaded from save. You can initialize your game using this event. </summary>
		public static event Action WorldLoaded;
		
		/// <summary> List exist as a storage of a temporary save data (when loading or saving world state). </summary>
		static List<SavedObject> saved = new ();
		
		/// <summary> Returns current save data. I recommend to use it only for internal purposes. </summary>
		public static List<SavedObject> GetActualSave() => saved;
		
		/// <summary> Collects all objects from scene and converts it into serialization-friendly List. You can serialize and save it as you want. </summary>
		/// <returns></returns>
		public static List<SavedObject> Collect()
		{
			if (saved == null)
				saved = new List<SavedObject>();
			else 
				saved.Clear();

			var saveables = GetSaveablesFromScene();

			foreach (var saveable in saveables)
				saved.Add(saveable.ConvertToDTO());

			return saved;
		}

		public static void Load(List<SavedObject> save)
		{
			saved = save ?? throw new NullReferenceException("Passed save is null! Cancelled loading.");
			
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			
			ResetWorldState();
		}
		
		public static Saveable[] GetSaveablesFromScene() => GameObject.FindObjectsOfType<Saveable>(true);

		static void OnSceneLoaded(Scene scene, LoadSceneMode _)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			
			ResetWorldState();
			
			var saveSystemData = SaveSystemData.Load();
			var saveablesOnScene = GetSaveablesFromScene();
			
			foreach (var savedObject in saved)
			{
				if (savedObject.IsSceneObject)
					LoadSaveableAsSceneObject(savedObject, saveablesOnScene);
				else
					LoadSaveableAsPrefab(savedObject, saveSystemData.AvailablePrefabs);
			}
			
			WorldLoaded?.Invoke();
		}
		
		static void LoadSaveableAsPrefab(SavedObject savedGo, List<MappedPrefab> mappedPrefabs)
		{
			foreach (var prefab in mappedPrefabs)
			{
				if (prefab.Id == savedGo.MappedPrefabId)
				{
					var spawned = Spawner.SaveSpawn(prefab.Prefab);
					LoadSaveable(spawned.GetComponent<Saveable>(), savedGo.SerializedData);
				}
			}
		}

		static void LoadSaveableAsSceneObject(SavedObject savedGo, Saveable[] saveablesOnScene)
		{
			foreach (var sceneSaveable in saveablesOnScene)
			{
				if (sceneSaveable.SceneUniqueId == savedGo.SceneInstanceId)
					LoadSaveable(sceneSaveable, savedGo.SerializedData);
			}
		}
		
		static void LoadSaveable(Saveable saveable, string serializedData)
		{
			var serializedType = saveable.GetSerializedType();
			var dto = Serializer.Deserialize(serializedData, serializedType);
			saveable.LoadSaveData(dto);
		}
		
		static void ResetWorldState()
		{
			var prefabSaveables = GetSaveablesFromScene();
			foreach (var saveable in prefabSaveables)
			{
				if (!saveable.IsSceneObject)
					GameObject.Destroy(saveable.gameObject);
			}
		}
	}
}