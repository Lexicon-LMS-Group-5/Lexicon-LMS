using System;

namespace LMS.Blazor.Client.Layout;

public partial class NavMenu
{
    private string MyCourseRouteUrl { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        // ToDo: Get course ID from ApplicationUser
        MyCourseRouteUrl = $"/courses/{1}";
    }
}
