using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.LiveChatInc.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.LiveChatInc.Components
{
    [ViewComponent(Name = "WidgetsLiveChatInc")]
    public class WidgetsLiveChatIncViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;

        public WidgetsLiveChatIncViewComponent(
            IWorkContext workContext,
            IStoreContext storeContext,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger,
            ICustomerService customerService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
            this._customerService = customerService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(_storeContext.CurrentStore.Id);

            if (string.IsNullOrEmpty(settings.License))
                return Content(string.Empty);

            var model = new PublicInfoModel
            {
                CartUpdateInterval = settings.CartUpdateInterval,
                HideOnMobile = settings.HideOnMobile,
                License = settings.License,
                IsRegisteredCustomer = _workContext.CurrentCustomer.IsInCustomerRole(NopCustomerDefaults.RegisteredRoleName),
                CustomerName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer),
                CustomerEmail = _workContext.CurrentCustomer.Email
            };
            return View("~/Plugins/Widgets.LiveChatInc/Views/PublicInfo.cshtml", model);
        }
    }
}