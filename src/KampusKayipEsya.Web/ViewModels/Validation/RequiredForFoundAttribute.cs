using System.ComponentModel.DataAnnotations;
using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.ViewModels.Validation;

public class RequiredForFoundAttribute(string typePropertyName) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var turAlani = validationContext.ObjectType.GetProperty(typePropertyName);
        var tur = turAlani?.GetValue(validationContext.ObjectInstance);

        if (tur is ItemType.Found && string.IsNullOrWhiteSpace(value as string))
            return new ValidationResult(ErrorMessage, [validationContext.MemberName!]);

        return ValidationResult.Success;
    }
}
