using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Widgets.LiveChatInc.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public int CartUpdateInterval { get; set; }
        public bool HideOnMobile { get; set; }
        public bool DisableSoundsForVisitor { get; set; }
        public string License { get; set; }
        public bool IsRegisteredCustomer { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
    }
}