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

    private List<UserReadDto>? AllUsers { get; set; }

    private string _sortColumn = "Roles";

    private bool _sortAscending = true;

    private string _searchTerm = "";

    private string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (_searchTerm != value)
            {
                _searchTerm = value;
                ApplyFilterAndSort();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        try
        {
            AllUsers = await ApiService.GetAsync<List<UserReadDto>>("api/users");
            ApplyFilterAndSort();
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

    private void ApplyFilterAndSort()
    {
        if (AllUsers == null)
        {
            Users = null;
            return;
        }

        IEnumerable<UserReadDto> query = AllUsers;

        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (u.Roles != null && u.Roles.Any(r =>
                    r.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))));
        }

        query = _sortColumn switch
        {
            "Name" => _sortAscending
                ? query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                : query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName),

            "Email" => _sortAscending
                ? query.OrderBy(u => u.Email)
                : query.OrderByDescending(u => u.Email),

            "Roles" => _sortAscending
                ? query.OrderBy(u => u.Roles?.FirstOrDefault())
                : query.OrderByDescending(u => u.Roles?.FirstOrDefault()),

            _ => query
        };

        Users = query.ToList();
    }

    private void SortBy(string column)
    {
        if (_sortColumn == column)
            _sortAscending = !_sortAscending;

        else
        {
            _sortColumn = column;
            _sortAscending = true;
        }

        ApplyFilterAndSort();
    }
}
