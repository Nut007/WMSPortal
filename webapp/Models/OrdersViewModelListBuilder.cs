using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;

namespace WMSPortal.Models
{
    public class OrdersViewModelListBuilder : IOrdersViewModelListBuilder
    {
        ICodelkupRepository _lookupRepository;
        public OrdersViewModelListBuilder(ICodelkupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }
        public void BuildListsOrdersViewModel(ViewModels.OrdersViewModel orderViewModel)
        {
            orderViewModel.PriorityList = _lookupRepository.GetLookupListByType(LookupType.DropLoc);
        }
    }
}