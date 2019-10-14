using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.LiveChatInc
{
    /// <summary>
    /// Google Analytic plugin
    /// </summary>
    public class LiveChatIncPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly LiveChatIncSettings _liveChatIncSettings;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        public LiveChatIncPlugin(IWebHelper webHelper, ISettingService settingService, 
            LiveChatIncSettings liveChatIncSettings, ILocalizationService localizationService)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._liveChatIncSettings = liveChatIncSettings;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "body_end_html_tag_before" };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsLiveChatInc/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version", "Version");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version.Hint", "Version");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.License","License");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.License.Hint", "License");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login", "Login");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login.Hint", "Login");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval", "Track your customers' carts");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval.Hint", "Cart's details will be visible during a chat in your LiveChat application.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile", "Hide chat window on mobile");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile.Hint", "Hide chat window for the mobile version of your website");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.ChangeAccount", "If you would like to change LiveChat account you can");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.ResetSettings", "reset settings.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.UseOurWebApp", "Use our WebApp");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.OrCheckOutApps", "or check out our apps for");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CheckOutApps", "Check out our apps for");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.DesktopOrMobile", "desktop or mobile!");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CurrentlyUsedAccount", "Currently you are using your");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.LiveChatAccount", "LiveChat account.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Or", "or");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Saving", "Saving...");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CreateAccount", "create an account");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.SuccessfullyAdded", "LiveChat added to your website");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.SettingsReset", "LiveChat settings has been reset");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<LiveChatIncSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.License");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.License.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile.Hint");

            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.ChangeAccount");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.ResetSettings");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.UseOurWebApp");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.OrCheckOutApps");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CheckOutApps");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.DesktopOrMobile");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CurrentlyUsedAccount");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.LiveChatAccount");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Or");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Saving");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CreateAccount");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.SuccessfullyAdded");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.SettingsReset");

            base.Uninstall();
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsLiveChatInc";
        }
    }
}