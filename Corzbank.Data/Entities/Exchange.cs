﻿using Corzbank.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corzbank.Data.Entities
{
    public class Exchange: KeyEntity
    {
        public CurrencyEnum ExchangeCurrency { get; set; }
        public CurrencyEnum BaseCurrency { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
    }
}