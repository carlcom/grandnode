﻿using System;

namespace Grand.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a auction bid
    /// </summary>
    public partial class Bid : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// Gets or sets the bid date
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets the customer id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// Gets or sets Amount
        /// </summary>
        public decimal Amount { get; set; }
    }
}
