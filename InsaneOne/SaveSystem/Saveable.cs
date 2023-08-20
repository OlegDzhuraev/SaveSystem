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
using UnityEngine;
using InsaneOne.Core;

namespace InsaneOne.SaveSystem
{
	/// <summary> Derive your own save classes from this one. </summary>
	[DisallowMultipleComponent]
	public abstract class Saveable : MonoBehaviour
	{
		[Tooltip("Automatically generated unique Id if this object is prefab. \n NOT used for scene objects.")]
		[SerializeField, ReadOnly] int prefabUniqueHash = -1;
		[Tooltip("Check this if this object will be placed on scene, not spawned from code as prefab.")]
		[SerializeField] bool isSceneObject;
		
		public int PrefabHashId => prefabUniqueHash;
		public int SceneUniqueId => SceneSaveHolder.Get().GetSaveableSceneId(this);
		public bool IsSceneObject => isSceneObject;

		/// <summary> Serialize data to DTO and return it in this method. </summary>
		public abstract object GetSaveData();
		public abstract void LoadSaveData(object data);

		public abstract Type GetSerializedType();

		public SavedObject ConvertToDTO()
		{
			return new SavedObject
			{
				SceneInstanceId = SceneUniqueId,
				MappedPrefabId = PrefabHashId,
				IsSceneObject = IsSceneObject,
				SerializedData = WorldSaver.Serializer.Serialize(GetSaveData())
			};
		}

#if UNITY_EDITOR
		public void EditorSetHash(int hash) => prefabUniqueHash = hash;
#endif
	}
}