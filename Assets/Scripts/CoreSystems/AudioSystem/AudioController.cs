using System.Collections;
using UnityEngine;

    
    namespace CoreSystems.AudioSystem {

        /// <summary>
        /// Controller of all the ingame sounds
        /// </summary>
        public class AudioController : MonoBehaviour
        {
            public static AudioController instance;

            public bool debug;
            public AudioTrack[] tracks;

            /// <summary>
            /// Relation between audio types and tracks
            /// </summary>
            private Hashtable _audioTable; // relationship of audio types (key) and tracks (value)
            
            /// <summary>
            /// Relation between audio types and jobs
            /// </summary>
            private Hashtable _jobTable;   // relationship between audio types (key) and jobs (value)

            private enum AudioAction {
                START,
                STOP,
                RESTART
            }

            /// <summary>
            /// Information needed to run a job
            /// </summary>
            private class AudioJob {
                public AudioAction action;
                public AudioType type;
                public bool fade;
                public WaitForSeconds delay;

                public AudioJob(AudioAction action, AudioType type, bool fade, float delay) {
                    this.action = action;
                    this.type = type;
                    this.fade = fade;
                    this.delay = delay > 0f ? new WaitForSeconds(delay) : null;
                }
            }

#region Unity Functions
            private void Awake() {
                if (!instance) {
                    Configure();
                }
            }

            private void OnDisable() {
                Dispose();
            }
            #endregion

            #region Public Functions
            /// <summary>
            /// Creates a job to play an audio
            /// </summary>
            /// <param name="type">The type</param>
            /// <param name="fade">Has fading</param>
            /// <param name="delay">Job delay</param>
            public void PlayAudio(AudioType type, bool fade=false, float delay=0.0F) {
                AddJob(new AudioJob(AudioAction.START, type, fade, delay));
            }

            /// <summary>
            /// Creates a job to stop an audio
            /// </summary>
            /// <param name="type">The type</param>
            /// <param name="fade">Has fading</param>
            /// <param name="delay">Job delay</param> 
            public void StopAudio(AudioType type, bool fade=false, float delay=0.0F) {
                AddJob(new AudioJob(AudioAction.STOP, type, fade, delay));
            }

            /// <summary>
            /// Creates a job to restart an audio
            /// </summary>
            /// <param name="type">The type</param>
            /// <param name="fade">Has fading</param>
            /// <param name="delay">Job delay</param>  
            public void RestartAudio(AudioType type, bool fade=false, float delay=0.0F) {
                AddJob(new AudioJob(AudioAction.RESTART, type, fade, delay));
            }
            #endregion

            #region Private Functions
            
            /// <summary>
            /// Configures an initial instance
            /// </summary>
            private void Configure() {
                instance = this;
                _audioTable = new Hashtable();
                _jobTable = new Hashtable();
                GenerateAudioTable();
                DontDestroyOnLoad(gameObject);
            }

            /// <summary>
            /// Disposes of all the jobs being run
            /// </summary>
            private void Dispose() {
                // cancel all jobs in progress
                foreach(DictionaryEntry _kvp in _jobTable) {
                    Coroutine _job = (Coroutine)_kvp.Value;
                    if(_job!=null)StopCoroutine(_job);
                }
            }

            /// <summary>
            /// Adds a job the job table
            /// </summary>
            /// <param name="job">The job tu run</param>
            private void AddJob(AudioJob job) {
                // cancel any job that might be using this job's audio source
                RemoveConflictingJobs(job.type);

                Coroutine _jobRunner = StartCoroutine(RunAudioJob(job));
                _jobTable.Add(job.type, _jobRunner);
                Log("Starting job on ["+job.type+"] with operation: "+job.action);
            }

            /// <summary>
            /// Removes a job from the table
            /// </summary>
            /// <param name="type">The audio type</param>
            private void RemoveJob(AudioType type) {
                if (!_jobTable.ContainsKey(type)) {
                    Log("Trying to stop a job ["+type+"] that is not running.");
                    return;
                }
                Coroutine _runningJob = (Coroutine)_jobTable[type];
                if(_runningJob != null) StopCoroutine(_runningJob);
                _jobTable.Remove(type);
            }

            /// <summary>
            /// Removes the conflicting job from the jobstable
            /// </summary>
            /// <param name="type">The audio type to check conflicts with</param>
            private void RemoveConflictingJobs(AudioType type) {
                // cancel the job if one exists with the same type
                if (_jobTable.ContainsKey(type)) {
                    RemoveJob(type);
                }

                // cancel jobs that share the same audio track
                AudioType _conflictAudio = AudioType.NONE;
                AudioTrack _audioTrackNeeded = GetAudioTrack(type, "Get Audio Track Needed");
                foreach (DictionaryEntry _entry in _jobTable) {
                    AudioType _audioType = (AudioType)_entry.Key;
                    AudioTrack _audioTrackInUse = GetAudioTrack(_audioType, "Get Audio Track In Use");
                    if (_audioTrackInUse.source == _audioTrackNeeded.source) {
                        _conflictAudio = _audioType;
                        break;
                    }
                }
                if (_conflictAudio != AudioType.NONE) {
                    RemoveJob(_conflictAudio);
                }
            }

            /// <summary>
            /// Runs the action of different jobs
            /// </summary>
            /// <param name="job">The job to run</param>
            /// <returns>A Coroutine</returns>
            private IEnumerator RunAudioJob(AudioJob job) {
                if (job.delay != null) yield return job.delay;

                AudioTrack _track = GetAudioTrack(job.type); // track existence should be verified by now
                _track.source.clip = GetAudioClipFromAudioTrack(job.type, _track);

                float _initial = 0f;
                float _target = 1f;
                switch (job.action) {
                    case AudioAction.START:
                        _track.source.Play();
                    break;
                    case AudioAction.STOP when !job.fade:
                        _track.source.Stop();
                    break;
                    case AudioAction.STOP:
                        _initial = 1f;
                        _target = 0f;
                    break;
                    case AudioAction.RESTART:
                        _track.source.Stop();
                        _track.source.Play();
                    break;
                }

                // fade volume
                if (job.fade) {
                    float _duration = 1.0f;
                    float _timer = 0.0f;

                    while (_timer <= _duration) {
                        _track.source.volume = Mathf.Lerp(_initial, _target, _timer / _duration);
                        _timer += Time.deltaTime;
                        yield return null;
                    }

                    // if _timer was 0.9999 and Time.deltaTime was 0.01 we would not have reached the target
                    // make sure the volume is set to the value we want
                    _track.source.volume = _target;

                    if (job.action == AudioAction.STOP) {
                        _track.source.Stop();
                    }
                }

                _jobTable.Remove(job.type);
                Log("Job count: "+_jobTable.Count);
            }

            /// <summary>
            /// Initializes the audio table
            /// </summary>
            private void GenerateAudioTable() {
                foreach(AudioTrack _track in tracks) {
                    foreach(AudioObject _obj in _track.audios) {
                        // do not duplicate keys
                        if (_audioTable.ContainsKey(_obj.type)) {
                            LogWarning("You are trying to register audio ["+_obj.type+"] that has already been registered.");
                        } else {
                            _audioTable.Add(_obj.type, _track);
                            Log("Registering audio ["+_obj.type+"]");
                        }
                    }
                }
            }

            /// <summary>
            /// Get track from audio type
            /// </summary>
            /// <param name="type">The audio type</param>
            /// <param name="job">the name of the job</param>
            /// <returns></returns>
            private AudioTrack GetAudioTrack(AudioType type, string job="") {
                if (!_audioTable.ContainsKey(type)) {
                    LogWarning("You are trying to <color=#fff>"+job+"</color> for ["+type+"] but no track was found supporting this audio type.");
                    return null;
                }
                return (AudioTrack)_audioTable[type];
            }

            private AudioClip GetAudioClipFromAudioTrack(AudioType _type, AudioTrack _track) {
                foreach (AudioObject _obj in _track.audios) {
                    if (_obj.type == _type) {
                        return _obj.clip;
                    }
                }
                return null;
            }

            private void Log(string msg) {
                if (!debug) return;
                Debug.Log("[Audio Controller]: "+msg);
            }
            
            private void LogWarning(string msg) {
                if (!debug) return;
                Debug.LogWarning("[Audio Controller]: "+msg);
            }
            #endregion
        }
    }
