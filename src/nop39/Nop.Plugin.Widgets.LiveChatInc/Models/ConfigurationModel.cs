using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.LiveChatInc.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.LiveChatInc.CartUpdateInterval")]
        public int CartUpdateInterval { get; set; }
        public bool CartUpdateInterval_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.LiveChatInc.HideOnMobile")]
        public bool HideOnMobile { get; set; }
        public bool HideOnMobile_OverrideForStore { get; set; }
        public LicenseInformation License { get; set; }
        public List<SelectListItem> TrackingCartOptions { get; internal set; }

        public class LicenseInformation : BaseNopModel
        {
            [NopResourceDisplayName("Plugins.Widgets.LiveChatInc.Version")]
            public string Version { get; set; }
            public bool Version_OverrideForStore { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.LiveChatInc.License")]
            public string License { get; set; }
            public bool License_OverrideForStore { get; set; }
            [NopResourceDisplayName("Plugins.Widgets.LiveChatInc.Login")]
            public string Login { get; set; }
            public bool Login_OverrideForStore { get; set; }
        }
    }
}