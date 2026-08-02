namespace HrPanel.UI.Models.Shared;

public sealed record EmptyStateViewModel(string Title,string Message,string Icon = "bi-inbox",string? ActionText = null,string? ActionUrl = null);
