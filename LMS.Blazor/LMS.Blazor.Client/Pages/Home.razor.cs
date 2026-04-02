
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LMS.Blazor.Client.Pages;

public partial class Home
{
    [CascadingParameter]
    private Task<AuthenticationState> authenticationStateTask { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await authenticationStateTask;

        if (!authenticationState.User.IsInRole("Teacher"))
        {
            Navigation.NavigateTo("my-course");
        }
    }
}