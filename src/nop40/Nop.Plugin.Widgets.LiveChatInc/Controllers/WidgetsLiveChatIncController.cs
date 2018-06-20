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
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public WidgetsLiveChatIncController(IWorkContext workContext,
             IStoreContext storeContext,
             IStoreService storeService,
             ISettingService settingService,
             IOrderService orderService,
             ICategoryService categoryService,
             IProductAttributeParser productAttributeParser,
             ILocalizationService localizationService,
             IPermissionService permissionService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._categoryService = categoryService;
            this._productAttributeParser = productAttributeParser;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        [Area(AreaNames.Admin)]
        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);
            var model = new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeScope,
                CartUpdateInterval = settings.CartUpdateInterval,
                DisableSoundsForVisitor = settings.DisableSoundsForVisitor,
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
                model.DisableSoundsForVisitor_OverrideForStore = _settingService.SettingExists(settings, x => x.DisableSoundsForVisitor, storeScope);
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
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<LiveChatIncSettings>(storeScope);

            settings.HideOnMobile = model.HideOnMobile;
            settings.CartUpdateInterval = model.CartUpdateInterval;
            settings.DisableSoundsForVisitor = model.DisableSoundsForVisitor;

            if (model.HideOnMobile_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.HideOnMobile, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.HideOnMobile, storeScope);

            if (model.CartUpdateInterval_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.CartUpdateInterval, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.CartUpdateInterval, storeScope);

            if (model.DisableSoundsForVisitor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.DisableSoundsForVisitor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.DisableSoundsForVisitor, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
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

            List<DiscountForCaching> orderSubTotalAppliedDiscounts;
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
    }
}