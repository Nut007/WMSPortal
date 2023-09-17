using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;

namespace WMSPortal.Models
{
    public class LoadingViewModelListBuilder : ILoadingViewModelListBuilder
    {
        ICodelkupRepository _lookupRepository;
        public LoadingViewModelListBuilder(ICodelkupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }
        public void BuildListsLoadingViewModel(ViewModels.LoadingViewModel viewModel)
        {
            //orderViewModel.PriorityList = _lookupRepository.GetLookupListByType(LookupType.DropLoc);
        }
    }
}