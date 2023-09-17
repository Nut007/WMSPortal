﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMSPortal.ViewModels;

namespace WMSPortal.Models
{
    public interface IOrdersViewModelListBuilder
    {
        void BuildListsOrdersViewModel(OrdersViewModel orderViewModel);
        
    }
}