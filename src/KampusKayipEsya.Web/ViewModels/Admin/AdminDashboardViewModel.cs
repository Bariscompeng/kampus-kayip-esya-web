namespace KampusKayipEsya.Web.ViewModels.Admin;

public record AdminDashboardViewModel(
    int CategoryCount,
    int LocationCount,
    int ItemCount,
    int PendingItemCount,
    int UserCount);
