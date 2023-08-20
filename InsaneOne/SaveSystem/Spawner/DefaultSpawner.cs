using UnityEngine;

namespace InsaneOne.SaveSystem
{
	public sealed class DefaultSpawner : ISaveSpawner
	{
		public GameObject SaveSpawn(GameObject prefab) => GameObject.Instantiate(prefab);
	}
}