using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Plugins;
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
        private readonly LiveChatIncSettings _LiveChatIncSettings;


        public LiveChatIncPlugin(IWebHelper webHelper, ISettingService settingService, 
            LiveChatIncSettings LiveChatIncSettings)
        {
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._LiveChatIncSettings = LiveChatIncSettings;
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
        /// Gets a view component for displaying plugin in public store
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <param name="viewComponentName">View component name</param>
        public void GetPublicViewComponent(string widgetZone, out string viewComponentName)
        {
            viewComponentName = "WidgetsLiveChatInc";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version", "Version");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version.Hint", "Version");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.License","License");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.License.Hint", "License");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login", "Login");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login.Hint", "Login");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval", "Track your customers' carts");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval.Hint", "Cart's details will be visible during a chat in your LiveChat application.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile", "Hide chat window on mobile");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile.Hint", "Hide chat window for the mobile version of your website");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.DisableSoundsForVisitor", "Disable chat window sounds");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.DisableSoundsForVisitor.Hint", "Disable chat window sounds for your customer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.ChangeAccount", "If you would like to change LiveChat account you can");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.ResetSettings", "reset settings.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.UseOurWebApp", "Use our WebApp");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.OrCheckOutApps", "or check out our apps for");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CheckOutApps", "Check out our apps for");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.DesktopOrMobile", "desktop or mobile!");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CurrentlyUsedAccount", "Currently you are using your");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.LiveChatAccount", "LiveChat account.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Or", "or");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.Saving", "Saving...");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.CreateAccount", "create an account");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.SuccessfullyAdded", "LiveChat added to your website");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LiveChatInc.SettingsReset", "LiveChat settings has been reset");

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
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Version.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.License");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.License.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Login.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CartUpdateInterval.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.HideOnMobile.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.DisableSoundsForVisitor");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.DisableSoundsForVisitor.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.ChangeAccount");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.ResetSettings");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.UseOurWebApp");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.OrCheckOutApps");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CheckOutApps");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.DesktopOrMobile");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CurrentlyUsedAccount");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.LiveChatAccount");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Or");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.Saving");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.CreateAccount");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.SuccessfullyAdded");
            this.DeletePluginLocaleResource("Plugins.Widgets.LiveChatInc.SettingsReset");

            base.Uninstall();
        }
    }
}