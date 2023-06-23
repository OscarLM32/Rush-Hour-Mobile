using System;
using UnityEngine;

namespace CoreSystems
{
    /// <summary>
    /// Basic class from which every "scene level" singleton will inherit  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonScene<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; protected set; }

        protected void Awake()
        {
            if (!Instance)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
    }
}