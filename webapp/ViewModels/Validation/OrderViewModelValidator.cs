using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels.Validation
{
    using FluentValidation;
    
    namespace FluentValidationApplication.Validation
    {
        public class OrderViewModelValidator : AbstractValidator<OrdersViewModel>
        {
            public OrderViewModelValidator()
            {
                RuleFor(x => x.StorerKey).NotNull().WithMessage("*Required");
                RuleFor(x => x.ExternOrderKey).NotNull().WithMessage("*Required");
                RuleFor(x => x.BuyerPO).NotEmpty().WithMessage("*Required");
                RuleFor(x => x.ConsigneeKey).NotEmpty().WithMessage("*Required");
                RuleFor(x => x.OrderDate).NotNull().WithMessage("*Required");
                RuleFor(x => x.DeliveryDate).NotNull().WithMessage("*Required");
            }
        }
    }  
}