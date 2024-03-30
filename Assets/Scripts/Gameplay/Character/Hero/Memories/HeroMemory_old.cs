// using UnityEngine;

// public class HeroMemory_old<T> : HeroMemoryBase where T : MonoBehaviour
// {
// 	public T Object { get; set; }
// 	public Vector3 LastKnownLocation { get; set; }

// 	public HeroMemory_old(T obj, Vector3 lastKnownLocation)
// 	{
// 		Object = obj;
// 		LastKnownLocation = lastKnownLocation;
// 	}
// }
// public class HeroMemoryBase
// {
// 	public MonoBehaviour Object { get; set; }
// 	public Vector3 LastKnownLocation { get; set; }
// }
// public class AngelMemory : HeroMemory<Treasure>
// {
// 	public AngelMemory(Treasure treasure, Vector3 lastKnownLocation) : base(treasure, lastKnownLocation) { }
// }
