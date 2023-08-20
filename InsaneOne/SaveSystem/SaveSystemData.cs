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

namespace InsaneOne.SaveSystem
{
	[CreateAssetMenu(menuName = "InsaneOne/Save System Data")]
	public sealed class SaveSystemData : ScriptableObject
	{
		static SaveSystemData instance;
		
		[Tooltip("It can not be edited manually. Use buttons below.")]
		public List<MappedPrefab> AvailablePrefabs = new List<MappedPrefab>();

		/// <summary> Loads settings instance from Resources folder if exist. </summary>
		public static SaveSystemData Load()
		{
			if (!instance)
			{
				var loaded = Resources.Load<SaveSystemData>(nameof(SaveSystemData));

				if (!loaded)
					throw new NullReferenceException("No SaveSystemData asset found in resources! You need to create it and place in the Resources folder.");
				
				instance = loaded;
			}

			return instance;
		}

		/// <summary> For internal usage only. </summary>
		public void AddPrefab(GameObject prefab)
		{
			var mapped = new MappedPrefab
			{
				Id = GetNewPrefabId(),
				Prefab = prefab
			};

			AvailablePrefabs.Add(mapped);
		}
		
		/// <summary> For internal usage only. </summary>
		public ushort GetNewPrefabId()
		{
			ushort curId = 0;
			
			foreach (var mappedPrefab in AvailablePrefabs)
				curId = Math.Max(curId, mappedPrefab.Id);

			return (ushort) (curId + 1);
		}
	}
}