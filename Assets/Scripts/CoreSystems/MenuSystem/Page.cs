using System.Collections;
using UnityEngine;

namespace CoreSystems.MenuSystem
{
    /// <summary>
    /// Represents the pages the PageController in going to be making use of
    /// </summary>
    /// <seealso cref="PageController"/>
    public class Page: MonoBehaviour
    {
        //TODO: convert into enumerator
        private static readonly string FLAG_ON = "On";
        private static readonly string FLAG_OFF = "Off";
        public static readonly string FLAG_NONE = "None";

        /// <summary>
        /// Marks whether the logs are exposed or not
        /// </summary>
        public bool debug = false;
        public PageType type;
        public string targetState { get; private set; }
        public bool useAnimation = false;

        
        /// <remarks>
        /// The animator requires to have 3 states and a 1 parameter to work properly:
        /// The 3 states needed are as follows:  rest state (when the page is not animating)
        /// off state (when the page is being turned off) on state (when the page is being turned on)
        /// The parameter that needs to be set is a boolean type called "on".
        /// </remarks>>
        private Animator _animator;
        public bool isOn { get; private set; }

        #region Unity functions
        private void OnEnable()
        {
            CheckAnimatorIntegrity();
        }

        #endregion
        
        #region Public functions

        /// <summary>
        /// Handles the petition to turn on or off the page. If the page uses no animation
        /// it will simple set the object to active/inactive
        /// </summary>
        /// <param name="on">Turn on or off</param>
        public void Animate(bool on)
        {
            if (useAnimation)
            {
                _animator.SetBool("on", on);
                
                StopCoroutine("AwaitAnimation");
                StartCoroutine(AwaitAnimation(on));
            }
            else
            {
                if (!on)
                {
                    isOn = false;
                    gameObject.SetActive(false);
                }
                else
                {
                    isOn = true;
                }
            }
        }
        
    #endregion
        
    #region Private functions

        /// <summary>
        /// Waits for the animation to finish before running any other logic
        /// </summary>
        /// <param name="on">Turn on or off</param>
        /// <returns>A coroutine</returns>
        private IEnumerator AwaitAnimation(bool on)
        {
            targetState = on ? FLAG_ON : FLAG_OFF;
            
            //wait for the animator to reach the desired animation state
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(targetState))
            {
                yield return null;
            }
            
            //wait for the animation to finish playing
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }

            Log("Page ["+type+"] finished transitioning to: " + (on ? "on" : "off"));
            
            targetState = FLAG_NONE;

            if (!on)
            {
                isOn = false;
                gameObject.SetActive(false);
            }
            else
            {
                isOn = true;
            }
        }

        /// <summary>
        /// Checks if the page has an animator component set up if the page uses animations
        /// </summary>
        private void CheckAnimatorIntegrity()
        {
            if (!useAnimation) return;

            _animator = GetComponent<Animator>();
            if (!_animator)
            {
                LogWarning("The page: " + gameObject.name + " of type [" + type +
                           "] is trying to use animations but has no 'Animator' component attached");
            }
        }
        
        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log("[Page]: " + msg);
        }

        private void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning("[Page]: " + msg);  
        } 
        
    #endregion
    }
}