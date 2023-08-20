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
	/// <summary> Can be used to save basic GameObject data: position, local rotation and active state. </summary>
	public sealed class BasicObjectSaveable : Saveable
	{
		public override object GetSaveData()
		{
			var tr = transform;
			
			var dto = new BasicObjectDTO()
			{
				Active = gameObject.activeSelf,
				Position = tr.position,
				LocalEuler = tr.localEulerAngles
			};

			return dto;
		}

		public override void LoadSaveData(object data)
		{
			var dto = data as BasicObjectDTO;
			var tr = transform;

			gameObject.SetActive(dto.Active);
			tr.position = dto.Position;
			tr.localEulerAngles = dto.LocalEuler;
		}

		public override Type GetSerializedType() => typeof(BasicObjectDTO);
	}
}