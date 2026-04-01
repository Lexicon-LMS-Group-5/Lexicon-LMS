using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LMS.Blazor.Client.Pages
{
    public partial class EditUser
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;

        private UserUpsertDto Model { get; set; } = new();
        private bool IsLoading { get; set; }
        private string? Error { get; set; }
        private string RolesInput { get; set; } = string.Empty;


        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                var user = await ApiService.GetAsync<UserReadDto>($"api/users/{Id}");

                if (user == null)
                {
                    Error = "User not found";
                    return;
                }

                RolesInput = string.Join(",", user.Roles ?? new List<string>());
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

        private async Task HandleValidSubmit() 
        { 
            try 
            { 
                Model.Roles = RolesInput.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToList(); 
                var result = await ApiService.PutAsync<UserUpsertDto, UserReadDto>($"api/users/edit/{Id}", Model); 
                if (result == null) 
                { 
                    Error = "Update failed"; 
                    return; 
                } 
                Navigation.NavigateTo("/users"); 
            } catch (Exception ex) { Error = ex.Message; } 
        }

        private void GoBack()
        {
            Navigation.NavigateTo("/users");
        }
    }
}
