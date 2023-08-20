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
using UnityEngine;

namespace InsaneOne.SaveSystem
{
	/// <summary> Used for editor purposes - stores info about scene save setup. </summary>
	public sealed class SceneSaveHolder : MonoBehaviour
	{
		static SceneSaveHolder instance;
		
		/// <summary> Every new saveable index ONLY increased. If something removed - this will be nulls, but indexes still taken!. </summary>
		public List<Saveable> SceneSaveables = new List<Saveable>();

		public static SceneSaveHolder Get()
		{
			if (instance)
				return instance;
			
			var foundInstance = FindObjectOfType<SceneSaveHolder>();

			if (foundInstance)
			{
				instance = foundInstance;
			}
			else
			{
				var go = new GameObject("SceneSave_DO_NOT_REMOVE");
				instance = go.AddComponent<SceneSaveHolder>();
			}

			return instance;
		}

		public int GetSaveableSceneId(Saveable saveable)
		{
			if (!saveable.IsSceneObject)
				return -1;
			
			for (var id = 0; id < SceneSaveables.Count; id++)
			{
				
				var sceneSaveable = SceneSaveables[id];

				if (sceneSaveable && saveable == sceneSaveable)
					return id;
			}

			return -1;
		}
	}
}