﻿namespace Logistics.DriverApp.ViewModels;

public class LoginPageViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IApiClient _apiClient;

    public LoginPageViewModel(
        IAuthService authService, 
        IApiClient apiClient)
    {
        _authService = authService;
        _apiClient = apiClient;
        SignInCommand = new AsyncRelayCommand(LoginAsync, () => !IsBusy);
        OpenSignUpCommand = new AsyncRelayCommand(OpenSignUpUrl, () => !IsBusy);
        IsBusyChanged += (s, e) => SignInCommand.NotifyCanExecuteChanged();
    }

    public IAsyncRelayCommand SignInCommand { get; }
    public IAsyncRelayCommand OpenSignUpCommand { get; }

    public async Task LoginAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _authService.LoginAsync();

            if (!result.IsError)
            {
                _apiClient.AccessToken = result.AccessToken;
                await Shell.Current.GoToAsync("//DashboardPage", true);
            }
        }
        catch (Exception ex)
        {
            await PopupHelpers.ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task OpenSignUpUrl()
    {
        var url = $"{_authService.Options.Authority}/account/register";
        await Launcher.OpenAsync(url);
    }
}
