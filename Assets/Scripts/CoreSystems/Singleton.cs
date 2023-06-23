using System;
using UnityEngine;

namespace CoreSystems
{
    
    /// <summary>
    /// Basic class from which every singleton will inherit 
    /// </summary>
    /// <typeparam name="T">Template</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            if (!Instance)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}