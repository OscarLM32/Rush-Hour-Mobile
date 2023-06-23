using UnityEngine;

namespace CoreSystems.MenuSystem
{
    /// <summary>
    /// This is a custom button script in which various parameters can be configured for a Unity button
    /// to handle, since Unity button cannot handle functions with various  and non-basic parameters 
    /// </summary>
    public class CustomPageControllerButton : MonoBehaviour
    {
        [SerializeField]private ButtonAction _action;
        
        /// <summary>
        /// The page to turn on 
        /// </summary>
        public PageType turnOn;
        
        /// <summary>
        /// The page to turn off
        /// </summary>
        public PageType turnOff;
        public bool waitExitTime;

        /// <summary>
        /// Handles the petition based on the parameters specified in the inspector
        /// </summary>
        public void HandlePetition()
        {
            switch (_action)
            {
                case ButtonAction.TURN_PAGE_ON:
                    PageController.instance.TurnPageOn(turnOn);
                    break;
                case ButtonAction.TURN_PAGE_OFF:
                    PageController.instance.TurnPageOff(turnOff, turnOn, waitExitTime);
                    break;
                default:
                    Debug.LogError("[CustomButton]: the action assigned to this button is not a proper action");
                    break;
            }
        }

        /// <summary>
        /// The action the button is going to handle
        /// </summary>
        private enum ButtonAction
        {
            TURN_PAGE_ON,
            TURN_PAGE_OFF
        }
    }
}