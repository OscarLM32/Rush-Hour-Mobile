using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Menus.MainMenu
{
    public class LocalizationManager : MonoBehaviour
    {
        public void SetLanguageEnglish()
        {
            LoadLocale(0);
        }

        public void SetLanguageSpanish()
        {
            LoadLocale(1);
        }

        private void LoadLocale(int id)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
        }  
    }
}