using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.FacebookShop
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.LiveChatInc.SetLicenseInformation",
                 "plugins/livechatinc/setlicenseinformation",
                 new { controller = "WidgetsLiveChatInc", action = "SetLicenseInformation" },
                 new[] { "Nop.Plugin.Widgets.LiveChatInc.Controllers" }
            );
            routes.MapRoute("Plugin.Widgets.LiveChatInc.ResetSettings",
               "plugins/livechatinc/resetsettings",
               new { controller = "WidgetsLiveChatInc", action = "ResetSettings" },
               new[] { "Nop.Plugin.Widgets.LiveChatInc.Controllers" }
          );
            routes.MapRoute("Plugin.Widgets.LiveChatInc.GetCustomerCart",
              "plugins/livechatinc/getcustomercart",
              new { controller = "WidgetsLiveChatInc", action = "GetCustomerCart" },
              new[] { "Nop.Plugin.Widgets.LiveChatInc.Controllers" }
         );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
