using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using serverapi.Entity;

namespace serverapi.Validator
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("Name is required.")
                .Length(1, 100)
                .WithMessage("Name must be between 1 and 100 characters.");

            RuleFor(x => x.Price)
                .NotNull()
                .WithMessage("Price is required.")
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Quantity)
                .NotNull()
                .WithMessage("Quantity is required.")
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.IdBrand)
                .NotNull()
                .WithMessage("IdBrand is required.");

            RuleFor(x => x.IdCategory)
                .NotNull()
                .WithMessage("IdCategory is required.");
        }
    }
}