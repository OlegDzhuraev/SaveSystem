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

namespace InsaneOne.SaveSystem
{
	[Serializable]
	public struct SavedObject
	{
		/// <summary>Use only for always-on-scene objects. This value can not exist or be incorrect for objects which was spawned after scene load and saved. </summary>
		public int SceneInstanceId;
		public bool IsSceneObject;
		public int MappedPrefabId;
		public string SerializedData;
	}
}