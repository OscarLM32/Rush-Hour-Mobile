using System;
using CoreSystems.AudioSystem;
using CoreSystems.SceneSystem;
using Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = CoreSystems.AudioSystem.AudioType;

namespace Menus
{
    /// <summary>
    /// Logic of the log in scene
    /// </summary>
    public class LogInManager : MonoBehaviour
    {
        public TMP_InputField email;
        public TMP_InputField password;

        public TextMeshProUGUI errorMsg;

        private TouchScreenKeyboard _touchScreenKeyboard;

        private void Start()
        {
            AudioController.instance.PlayAudio(AudioType.ST_MAIN_THEME);
        }

        /// <summary>
        /// Opens the virtual keyboard in mobile devices
        /// </summary>
        public void OpenKeyboard()
        {
            _touchScreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
            TouchScreenKeyboard.hideInput = true;
        }

        /// <summary>
        /// Closes the virtual keyboard
        /// </summary>
        public void CloseKeyboard()
        {
            _touchScreenKeyboard.active = false;
        }
        
        /// <summary>
        /// Signs the user in
        /// </summary>
        public void SignIn()
        {
            DatabaseManager.Instance.SignInWithEmailPassword(
                email.text, 
                password.text, 
                OnSignInSuccess,
                OnSignInFailure);
            
            //DatabaseManager.Instance.auth.SignInWithEmailAndPasswordAsync(email.text, password.text); 
        }

        /// <summary>
        /// Sings up a user
        /// </summary>
        public void SignUpUser()
        {
            DatabaseManager.Instance.CreateEmailPasswordAccount(
                email.text, 
                password.text, 
                OnSignUpSuccess,
                OnSignUpFailure);
        }

        /// <summary>
        /// Action to be executed when the signin is successful
        /// </summary>
        private void OnSignInSuccess()
        {
            Debug.Log("Successfully logged");
            SceneController.instance.Load(SceneType.MainMenu/*, SetUpUser*/);
            Debug.Log("MainMenu loaded");
        }
        
        private void SetUpUser()
        {
            User.Instance.SetUpUser(DatabaseManager.Instance.auth.CurrentUser);
        }

        /// <summary>
        /// Action to be executed when the signin fails
        /// </summary>
        private void OnSignInFailure()
        {
            Debug.Log("Error signing in");
        }

        /// <summary>
        /// Action to be executed when the sign up is successful 
        /// </summary>
        private void OnSignUpSuccess()
        {
            
        }

        /// <summary>
        /// Action to be executed when the sign up fails
        /// </summary>
        private void OnSignUpFailure()
        {
            
        }
        
    }
}