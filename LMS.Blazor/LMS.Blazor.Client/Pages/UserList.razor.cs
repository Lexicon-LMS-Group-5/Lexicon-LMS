using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class UserList
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private int? UserId { get; set; } = null;

    private bool IsLoading { get; set; }
    private string? Error { get; set; } = null;
    private List<UserReadDto>? Users { get; set; } = null;

    private string _sortColumn = "Roles";

    private bool _sortAscending = true;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        try
        {
            Users = await ApiService.GetAsync<List<UserReadDto>>("api/users");

            if (Users != null && Users.Count != 0)
            {
                Users = Users.OrderBy(u => u.Roles.FirstOrDefault()).ThenBy(u => u.FirstName).ThenBy(u => u.LastName).ToList();
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SortBy(string column)
    {
        if (_sortColumn == column)
        {
            _sortAscending = !_sortAscending;
        }
        else
        {
            _sortColumn = column;
            _sortAscending = true;
        }

        if (Users == null) return;

        Users = _sortColumn switch
        {
            "Name" => _sortAscending
                        ? Users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToList()
                        : Users.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName).ToList(),
            "Email" => _sortAscending
                        ? Users.OrderBy(u => u.Email).ToList()
                        : Users.OrderByDescending(u => u.Email).ToList(),
            "Roles" => _sortAscending
                        ? Users.OrderBy(u => u.Roles?.FirstOrDefault()).ToList()
                        : Users.OrderByDescending(u => u.Roles?.FirstOrDefault()).ToList(),
            _ => Users
        };
    }
}
