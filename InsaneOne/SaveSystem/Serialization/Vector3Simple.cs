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

namespace InsaneOne.SaveSystem
{
	[Serializable]
	public struct Vector3Simple
	{
		public float X, Y, Z;

		public Vector3Simple(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		
		public static implicit operator Vector3(Vector3Simple v) => new (v.X, v.Y, v.Z);
		public static implicit operator Vector3Simple(Vector3 v) => new (v.x, v.y, v.z);
	}
}