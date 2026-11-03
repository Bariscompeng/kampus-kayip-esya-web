using System.ComponentModel.DataAnnotations;

namespace KampusKayipEsya.Web.ViewModels.Validation;

public class NotInFutureAttribute : ValidationAttribute
{
    public NotInFutureAttribute() : base("{0} bugünden sonra olamaz.")
    {
    }

    public override bool IsValid(object? value) =>
        value is not DateTime tarih || tarih.Date <= DateTime.Today;
}
