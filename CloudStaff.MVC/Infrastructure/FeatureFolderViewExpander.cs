using Microsoft.AspNetCore.Mvc.Razor;

namespace CloudStaff.MVC.Infrastructure;

public class FeatureFolderViewExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context) { }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        return viewLocations.Concat(new[]
        {
            "/Features/{1}/{0}.cshtml",
            "/Features/{1}/Views/{0}.cshtml",
        });
    }
}