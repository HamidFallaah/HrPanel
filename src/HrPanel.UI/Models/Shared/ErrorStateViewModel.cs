namespace HrPanel.UI.Models.Shared;

public sealed record ErrorStateViewModel(string Title,string Message,bool CanRetry = true);
