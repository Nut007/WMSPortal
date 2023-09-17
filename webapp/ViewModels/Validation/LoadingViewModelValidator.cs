using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels.Validation
{
    using FluentValidation;
    
    namespace FluentValidationApplication.Validation
    {
        public class LoadingViewModelValidator : AbstractValidator<LoadingViewModel>
        {
            public LoadingViewModelValidator()
            {
                RuleFor(x => x.LOADING_DATE).NotNull().WithMessage("*Required");
            }
        }
    }  
}