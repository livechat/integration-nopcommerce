using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.LiveChatInc.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.LiveChatInc.Controllers
{
    public class WidgetsLiveChatIncController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;

        public WidgetsLiveChatIncController(IWorkContext workContext,
             IStoreContext storeContext,
             ISettingService settingService,
             ILocalizationService localizationService,
            INotificationService notificationService,
             IPermissionService permissionService,
             IShoppingCartService shoppingCartService,
             IProductService productService)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
        }

        [Area(AreaNames.Admin)]
        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);
            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                CartUpdateInterval = settings.CartUpdateInterval,
                HideOnMobile = settings.HideOnMobile,
                TrackingCartOptions = new List<SelectListItem>
                {
                    new SelectListItem{ Text="Don't track", Value="0"},
                    new SelectListItem{ Text="Track every 10s", Value="10000"},
                    new SelectListItem{ Text="Track every 30s", Value="30000"},
                },
                License = new ConfigurationModel.LicenseInformation
                {
                    License = settings.License,
                    Login = settings.Login,
                    Version = settings.Version
                }
            };

            if (storeScope > 0)
            {
                model.CartUpdateInterval_OverrideForStore = _settingService.SettingExists(settings, x => x.CartUpdateInterval, storeScope);
                model.HideOnMobile_OverrideForStore = _settingService.SettingExists(settings, x => x.HideOnMobile, storeScope);
                model.License.Login_OverrideForStore = _settingService.SettingExists(settings, x => x.Login, storeScope);
                model.License.License_OverrideForStore = _settingService.SettingExists(settings, x => x.License, storeScope);
                model.License.Version_OverrideForStore = _settingService.SettingExists(settings, x => x.Version, storeScope);
            }

            return View("~/Plugins/Widgets.LiveChatInc/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);

            settings.HideOnMobile = model.HideOnMobile;
            settings.CartUpdateInterval = model.CartUpdateInterval;


            _settingService.SaveSettingOverridablePerStore(settings, x => x.HideOnMobile, model.HideOnMobile_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.CartUpdateInterval, model.CartUpdateInterval_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public ActionResult SetLicenseInformation(ConfigurationModel model)
        {
            var license = model.License;

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);
            settings.Login = license.Login;
            settings.License = license.License;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(settings, x => x.Login, license.Login_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.License, license.License_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            return Json(new { success = true, login = settings.Login, license = settings.License });
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public ActionResult ResetSettings()
        {
            //load settings for a chosen store scope
            _settingService.DeleteSetting<LiveChatIncSettings>();
            //now clear settings cache
            _settingService.ClearCache();

            return Json(new { success = true });
        }

        public ActionResult GetCustomerCart()
        {
            var customer = _workContext.CurrentCustomer;
            if (!customer.HasShoppingCartItems)
                return Json(null);

            var cart = _shoppingCartService.GetShoppingCart(customer, shoppingCartType: ShoppingCartType.ShoppingCart, storeId: _storeContext.CurrentStore.Id);
            if (cart.Count == 0)
                return Json(null);

            var productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
            var priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
            var taxService = EngineContext.Current.Resolve<ITaxService>();
            var taxSettings = EngineContext.Current.Resolve<TaxSettings>();
            var priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
            var orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
            var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();


            //subtotal
            decimal orderSubTotalDiscountAmountBase;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;

            List<Core.Domain.Discounts.Discount> orderSubTotalAppliedDiscounts;
            var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !taxSettings.ForceTaxExclusionFromOrderSubtotal;
            orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            decimal subtotalBase = subTotalWithoutDiscountBase;
            decimal subtotal = currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);


            var data = new
            {
                subtotal = priceFormatter.FormatPrice(subtotal, false, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax),
                products = cart.Select(sci =>
                {
                    var product = _productService.GetProductById(sci.ProductId);

                    return new
                    {
                        Quantity = sci.Quantity,
                        ProductName = _localizationService.GetLocalized(product, x => x.Name),
                        Link = webHelper.GetStoreLocation() + Url.Action("ProductDetails", "Product", new { productId = sci.ProductId })
                    };
                }).ToList()
            };

            return Json(data);
        }
    }
}