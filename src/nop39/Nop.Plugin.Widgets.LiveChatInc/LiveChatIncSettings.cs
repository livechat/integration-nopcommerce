
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.LiveChatInc
{
    public class LiveChatIncSettings : ISettings
    {
        public string Version { get; set; }
        public string License { get; set; }
        public string Login { get; set; }
        public int CartUpdateInterval { get; set; }
        public bool HideOnMobile { get; set; }
        public bool DisableSoundsForVisitor { get; set; }
    }
}