using UnityEngine;

namespace InsaneOne.SaveSystem
{
	public interface ISaveSpawner
	{
		public GameObject SaveSpawn(GameObject prefab);
	}
}