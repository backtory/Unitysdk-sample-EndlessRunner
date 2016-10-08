﻿using System.Collections.Generic;
using UnityEngine;

// taken from http://wiki.unity3d.com/index.php/Singleton
namespace Assets.BacktorySDK.core
{
	/// <summary>
	/// Be aware this will not prevent a non singleton constructor
	///   such as `T myT = new T();`
	/// To prevent that, add `protected T () {}` to your singleton class.
	/// 
	/// As a note, this is made as MonoBehaviour because we need Coroutines.
	/// </summary>
	public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;

		private static object _lock = new object();

		public static T Instance
		{
			get
			{
				if (classCount <= 0) {
					Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
						"' already destroyed on application quit." +
						" Won't create again - returning null.");
					return null;
				}

				lock(_lock)
				{
					if (_instance == null)
					{
						_instance = (T) FindObjectOfType(typeof(T));

						if ( FindObjectsOfType(typeof(T)).Length > 1 )
						{
							Debug.LogError("[Singleton] Something went really wrong " +
								" - there should never be more than 1 singleton!" +
								" Reopening the scene might fix it.");
							return _instance;
						}

						if (_instance == null)
						{
							GameObject singleton = new GameObject();
							_instance = singleton.AddComponent<T>();
							singleton.name = "(singleton) "+ typeof(T).ToString();

							DontDestroyOnLoad(singleton);

							Debug.Log("[Singleton] An instance of " + typeof(T) + 
								" is needed in the scene, so '" + singleton +
								"' was created with DontDestroyOnLoad.");
						} else {
							DontDestroyOnLoad (_instance.gameObject);
							Debug.Log("[Singleton] Using instance already created: " +
								_instance.gameObject.name);
						}
					}

					return _instance;
				}
			}
		}

		/// <summary>
		/// The game object associated with this singleton belongs to a scene.
		/// When this scene is loaded for the second time, 2 instances of a singleton game object
		///   are created, which only one of them is ``dontDestroyOnLoad'' child. For Solving 
		///   this issue, ``classCount'' attirbute is created. It controlls the number of 
		///   singleton instances in the scene!! (Absolutely a Unity bug!)
		/// </summary>
		private static int classCount = 0;

		void Awake() {
			classCount++;
			Debug.Log ("[Singleton] Singleton.Awake()--> count = " + classCount);
		}

		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		void OnDestroy () {
			classCount--;
			Debug.Log ("[Singleton] Singleton.OnDestroy()--> count = " + classCount);
		}
	}

	// method extension pattern
	static public class MethodExtensionForMonoBehaviourTransform
	{
		/// <summary>
		/// Gets or add a component. Usage example:
		/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
		/// </summary>
		static public T GetOrAddComponent<T>(this Component child) where T : Component
		{
			T result = child.GetComponent<T>();
			if (result == null)
			{
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}
	}
}
