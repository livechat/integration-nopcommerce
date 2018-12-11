using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.LiveChatInc.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;


namespace Nop.Plugin.Widgets.LiveChatInc.Controllers
{
    public class WidgetsLiveChatIncController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;

        public WidgetsLiveChatIncController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger,
            ICategoryService categoryService,
            IProductAttributeParser productAttributeParser,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
            this._categoryService = categoryService;
            this._productAttributeParser = productAttributeParser;
            this._localizationService = localizationService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
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

            return View("~/Plugins/Widgets.LiveChatInc/Views/WidgetsLiveChatInc/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);

            settings.HideOnMobile = model.HideOnMobile;
            settings.CartUpdateInterval = model.CartUpdateInterval;

            if (model.HideOnMobile_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.HideOnMobile, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.HideOnMobile, storeScope);

            if (model.CartUpdateInterval_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.CartUpdateInterval, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.CartUpdateInterval, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetLicenseInformation(ConfigurationModel model)
        {
            var license = model.License;

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);
            settings.Login = license.Login;
            settings.License = license.License;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (license.Login_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.Login, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.Login, storeScope);

            if (license.License_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.License, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.License, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            return Json(new { success = true, login = settings.Login, license = settings.License });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (!customer.HasShoppingCartItems) return Json(null);

            var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart).ToList();
            if (cart.Count == 0) return Json(null);

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

            List<Discount> orderSubTotalAppliedDiscounts;
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
                    // decimal taxRate;
                    //var store = _storeService.GetStoreById(sci.StoreId);
                    var sciModel = new
                    {
                        //  Store = store != null ? store.Name : "Unknown",
                        Quantity = sci.Quantity,
                        ProductName = sci.Product.Name,
                        //AttributeInfo = productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                        //Total = priceFormatter.FormatPrice(taxService.GetProductPrice(sci.Product, priceCalculationService.GetSubTotal(sci), out taxRate)),
                        Link = webHelper.GetStoreLocation() + Url.Action("ProductDetails", "Product", new { productId = sci.ProductId })
                    };
                    return sciModel;
                }).ToList()
            };

            return Json(data);
        }


        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);

            if (string.IsNullOrEmpty(settings.License))
                return Content(string.Empty);

            var model = new PublicInfoModel
            {
                CartUpdateInterval = settings.CartUpdateInterval,
                HideOnMobile = settings.HideOnMobile,
                License = settings.License,
                IsRegisteredCustomer = _workContext.CurrentCustomer.IsInCustomerRole(SystemCustomerRoleNames.Registered),
                CustomerName = _workContext.CurrentCustomer.GetFullName(),
                CustomerEmail = _workContext.CurrentCustomer.Email
            };

            return View("~/Plugins/Widgets.LiveChatInc/Views/WidgetsLiveChatInc/PublicInfo.cshtml", model);
        }
    }
}