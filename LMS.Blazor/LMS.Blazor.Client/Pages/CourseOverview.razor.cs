using System;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class CourseOverview
{
    [Parameter]
    public string CourseId { get; set; } = string.Empty;
}
