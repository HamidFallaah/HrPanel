using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace HrPanel.UI.Ui;

/// <summary>
/// Resolves views that live beside their presentation feature without forcing
/// controllers to return fragile, explicit view paths.
/// </summary>
internal sealed class FeatureViewLocationExpander : IViewLocationExpander
{
    private const string FeatureKey = "HrPanelFeature";
    private static readonly IReadOnlyDictionary<string, string> ControllerFeatures =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Assets"] = "Assets",
            ["Auth"] = "Authentication",
            ["Dashboard"] = "Dashboard",
            ["Employees"] = "Employees",
            ["Employments"] = "Employments",
            ["Errors"] = "Shared",
            ["Lookups"] = "ReferenceData",
            ["Organization"] = "Organization",
            ["Scheduling"] = "Scheduling"
        };

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        if (context.ActionContext.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return;
        }

        if (!ControllerFeatures.TryGetValue(descriptor.ControllerName, out var feature))
        {
            return;
        }

        context.Values[FeatureKey] = feature;
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,IEnumerable<string> viewLocations)
    {
        if (context.Values.TryGetValue(FeatureKey, out var feature) &&
            !string.IsNullOrWhiteSpace(feature))
        {
            yield return $"/Features/{feature}/Views/{{0}}.cshtml";
            yield return $"/Features/{feature}/Views/Shared/{{0}}.cshtml";
        }

        yield return "/Shared/Views/{0}.cshtml";

        foreach (var location in viewLocations)
        {
            yield return location;
        }
    }
}
