using System.Collections;
using UnityEngine;

namespace CoreSystems.MenuSystem
{
    /// <summary>
    /// Handles the pages in the scene
    /// </summary>
    public class PageController : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PageController instance;

        public bool debug;
        /// <summary>
        /// The initial page to be displayed
        /// </summary>
        public PageType entryPage; //The default page to be loaded on entering the game
        /// <summary>
        /// Array of the pages to be handled in the scene
        /// </summary>
        public Page[] pages;

        /// <summary>
        /// Hashtable containing a relation between PageTypes and pages
        /// </summary>
        private Hashtable _pagesTable;
        
        #region Unity functions
        private void Awake()
        {
            //If there is already a page controller
            if (instance)
            {
                LogWarning("There is already an instance of the controller. Destroying: " + gameObject.name);
                Destroy(gameObject);
                return;
            } 
            
            instance = this;
            _pagesTable = new Hashtable();
            RegisterAllPages();
                
            if(entryPage != PageType.NONE)
                TurnPageOn(entryPage);
            
            //I cannot see this system working properly with a "DontDestroy" object 
            //DontDestroyOnLoad(gameObject);
        }

    #endregion
        
        #region Public functions

        /// <summary>
        /// Turn the specified page on (with animation)
        /// </summary>
        /// <param name="type">The page type</param>
        public void TurnPageOn(PageType type)
        {
            //Sentinels
            if(type == PageType.NONE) return;
            if (!PageExists(type))
            {
                LogWarning("The page of type: [" +  type + "] trying to be turned on has not been registered");
                return;
            }

            //Logic
            Page page = GetPage(type);
            page.gameObject.SetActive(true);
            page.Animate(true);
        }

        /// <summary>
        /// Turns the specified page off and optionally turns another page on
        /// </summary>
        /// <param name="off">The page to turn off</param>
        /// <param name="on">The page to be turned on</param>
        /// <param name="waitForExit">Specified whether to wait for the page to be turned off before turning on the page or to do both animation at the same time</param>
        public void TurnPageOff(PageType off, PageType on = PageType.NONE, bool waitForExit = false)
        {
            //Sentinels
            if (off == PageType.NONE) return;
            if (!PageExists(off))
            {
                LogWarning("The page of type: [" +  off + "] trying to be turned off has not been registered");
                return;
            }
            
            //Logic
            Page offPage = GetPage(off);
            if (offPage.gameObject.activeSelf)
            {
                offPage.Animate(false);
            }

            //If no page has been set to be turned on we return
            if (on == PageType.NONE) return;
            
            if (waitForExit)
            {
                Page onPage = GetPage(on);  
                StartCoroutine(WaitForPageExit(onPage, offPage));
            }
            else
            {
                TurnPageOn(on);
            }
        }
        
        /// <summary>
        /// Returns is a page is on
        /// </summary>
        /// <param name="type">The page type to look for</param>
        /// <returns>If the page is on or not</returns>
        public bool PageIsOn(PageType type) {
            if (!PageExists(type)) {
                LogWarning("You are trying to detect if a page is on ["+type+"], but it has not been registered.");
                return false;
            }
            
            return GetPage(type).isOn;
        } 
        
        #endregion
        
        #region Private functions

        /// <summary>
        /// Waits for the page to be turned off before turning on the other page
        /// </summary>
        /// <param name="on">The page to turn on</param>
        /// <param name="off">The page to turn off</param>
        /// <returns>A Coroutine</returns>
        private IEnumerator WaitForPageExit(Page on, Page off)
        {
            //While the page animating to on or off wait
            while (off.targetState != Page.FLAG_NONE)
            {
                yield return null;
            }
            
            TurnPageOn(on.type);
        }

        /// <summary>
        /// Populates the hashtable with the specified pages
        /// </summary>
        private void RegisterAllPages()
        {
            foreach (var page in pages)
            {
                RegisterPage(page);
            }     
        }

        /// <summary>
        /// Inserts a page into the pages hashtable
        /// </summary>
        /// <param name="page"></param>
        private void RegisterPage(Page page)
        {
            if (PageExists(page.type)){ //We don't want duplicated pages 
                LogWarning("The page of type: [" + page.type + "] is already registered. Page: " + page.gameObject.name);
                return; 
            }
            
            _pagesTable.Add(page.type, page);
            Log("Registered new page of type: [" + page.type + "]");
        }
        
        /// <summary>
        /// Return a page stored in the pages hashtable
        /// </summary>
        /// <param name="type">The page type</param>
        /// <returns>The page or null if it was not found</returns>
        private Page GetPage(PageType type)
        {
            if (!PageExists(type))
            {
                LogWarning("The page of type: ["+ type +"] trying to be accessed has not been registered");
                return null;
            }

            return (Page)_pagesTable[type];
        }

        /// <summary>
        /// Checks if a page exists inside the pages hashtable
        /// </summary>
        /// <param name="type">The page type to look for</param>
        /// <returns>Whether the hashtable contains the page or not</returns>
        private bool PageExists(PageType type)
        {
            return _pagesTable.ContainsKey(type);
        }

        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log("[PageController]: " + msg);
        }

        private void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning("[PageController]: " + msg);  
        }

    #endregion
    } 
}

