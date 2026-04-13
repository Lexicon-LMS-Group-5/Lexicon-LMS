using LMS.Blazor.Client.Services;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages;

public partial class UserList
{
    [Inject]
    private IApiService ApiService { get; set; } = default!;

    private bool IsLoading;
    private string? Error;

    private List<UserReadDto>? _allUsers;

    private List<UserReadDto>? _filteredUsers;

    private List<UserReadDto>? _pagedUsers;

    private string _sortColumn = "Roles";

    private bool _sortAscending = true;

    private string _searchTerm = "";

    private int _currentPage = 1;

    private const int _pageSize = 15;
    private int TotalPages => (int)Math.Ceiling((double)(_filteredUsers?.Count ?? 0) / _pageSize);

    private enum ModalType
    {
        None,
        EditUser,
        CreateUser,
        CreateStudent,
        DeleteUser,
    }

    private ModalType _activeModal = ModalType.None;

    private string _selectedUserId = "";
    private string _selectedUserName = "";
    private string? _currentUserId;

    private const int _courseIdTemp = 3;

    private void OpenEditModal(string userId)
    {
        _selectedUserId = userId;
        _activeModal = ModalType.EditUser;
    }

    private void OpenCreateModal()
    {
        _activeModal = ModalType.CreateUser;
    }

    private void OpenTempModal()
    {
        _activeModal = ModalType.CreateStudent;
    }

    private void OpenDeleteModal(string userId, string userName)
    {
        _selectedUserId = userId;
        _selectedUserName = userName;
        _activeModal = ModalType.DeleteUser;
    }

    private void CloseModal()
    {
        _activeModal = ModalType.None;
    }


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

        var me = await ApiService.GetAsync<UserReadDto>("api/users/me");
        _currentUserId = me?.Id;

        try
        {
            _allUsers = await ApiService.GetAsync<List<UserReadDto>>("api/users");
            ApplyFilterAndSort();
            if (_filteredUsers != null && _filteredUsers.Count != 0)
            {
                _filteredUsers = _filteredUsers.OrderBy(u => u.Roles.FirstOrDefault()).ThenBy(u => u.FirstName).ThenBy(u => u.LastName).ToList();
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
        if (_allUsers == null)
        {
            _filteredUsers = null;
            _pagedUsers = null;
            return;
        }

        IEnumerable<UserReadDto> query = _allUsers;

        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.CourseName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
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

        _filteredUsers = query.ToList();

        if (_currentPage > TotalPages)
        {
            _currentPage = TotalPages == 0 ? 1 : TotalPages;
        }

        _pagedUsers = _filteredUsers
            .Skip((_currentPage - 1) * _pageSize)
            .Take(_pageSize)
            .ToList();
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
        _currentPage = 1;
        ApplyFilterAndSort();
    }

    private void GoToPage(int page)
    {
        if (page < 1) page = 1;
        if (page > TotalPages) page = TotalPages;

        _currentPage = page;
        ApplyFilterAndSort();
    }

    private void NextPage() => GoToPage(_currentPage + 1);
    private void PreviousPage() => GoToPage(_currentPage - 1);

    private async Task HandleUserSaved()
    {
        _activeModal = ModalType.None;

        _allUsers = await ApiService.GetAsync<List<UserReadDto>>("api/users");
        ApplyFilterAndSort();
    }

    private bool IsCurrentUser(string userId)
    {
        return _currentUserId == userId;
    }
}
