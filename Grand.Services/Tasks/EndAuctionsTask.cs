﻿using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Tasks;
using Grand.Services.Catalog;
using Grand.Services.Customers;
using Grand.Services.Logging;
using Grand.Services.Messages;
using Grand.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Services.Tasks
{
    /// <summary>
    /// Represents a task end auctions
    /// </summary>
    public partial class EndAuctionsTask : ScheduleTask, IScheduleTask
    {
        private readonly IProductService _productService;
        private readonly IAuctionService _auctionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;

        public EndAuctionsTask(IProductService productService, IAuctionService auctionService, IQueuedEmailService queuedEmailService,
            IWorkflowMessageService workflowMessageService, LocalizationSettings localizationService, IShoppingCartService shoppingCartService,
            ICustomerService customerService)
        {
            this._productService = productService;
            this._auctionService = auctionService;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationService;
            this._shoppingCartService = shoppingCartService;
            this._customerService = customerService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var auctionsToEnd = _auctionService.GetAuctionsToEnd();
            foreach (var auctionToEnd in auctionsToEnd)
            {
                var bid = _auctionService.GetBidsByProductId(auctionToEnd.Id).OrderByDescending(x => x.Amount).FirstOrDefault();
                if (bid == null)
                    throw new ArgumentNullException("bid");

                _workflowMessageService.SendAuctionEndedCustomerNotification(auctionToEnd, null, bid);
                _workflowMessageService.SendAuctionEndedStoreOwnerNotification(auctionToEnd, _localizationSettings.DefaultAdminLanguageId, bid);

                var warnings = _shoppingCartService.AddToCart(_customerService.GetCustomerById(bid.CustomerId), bid.ProductId, Core.Domain.Orders.ShoppingCartType.Auctions,
                    bid.StoreId, customerEnteredPrice: bid.Amount);

                _auctionService.UpdateAuctionEnded(auctionToEnd, true);
            }
        }
    }
}
