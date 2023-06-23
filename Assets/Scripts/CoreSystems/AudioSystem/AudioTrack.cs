using System;
using UnityEngine;

namespace CoreSystems.AudioSystem
{
    /// <summary>
    /// Relation between an AudioSource and different AudioObjects
    /// </summary>
    [Serializable]
    public class AudioTrack
    {
        public AudioSource source;
        public AudioObject[] audios;
    }
    
    /// <summary>
    /// Relation between an audio clip and an AudioType
    /// </summary>
    [Serializable]
    public class AudioObject
    {
        public AudioType type;
        public AudioClip clip;
    }
    
}