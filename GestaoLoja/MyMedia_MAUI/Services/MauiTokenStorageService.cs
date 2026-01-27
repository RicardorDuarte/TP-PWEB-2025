using RCLGeral.Models;
using RCLGeral.Services;
using System.Text.Json;

namespace MyMedia_MAUI.Services;

/// <summary>
/// Implementação do ITokenStorageService para MAUI usando SecureStorage
/// </summary>
public class MauiTokenStorageService : ITokenStorageService
{
    private const string TokenKey = "auth_token";
    private const string UserKey = "user_info";

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
        catch (Exception)
        {
            // SecureStorage pode falhar em alguns dispositivos
            return Preferences.Get(TokenKey, null);
        }
    }

    public async Task SetTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }
        catch (Exception)
        {
            // Fallback para Preferences se SecureStorage falhar
            Preferences.Set(TokenKey, token);
        }
    }

    public Task RemoveTokenAsync()
    {
        try
        {
            SecureStorage.Remove(TokenKey);
        }
        catch (Exception)
        {
            Preferences.Remove(TokenKey);
        }
        return Task.CompletedTask;
    }

    public async Task<UserInfo?> GetUserAsync()
    {
        try
        {
            var json = await SecureStorage.GetAsync(UserKey);
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<UserInfo>(json);
        }
        catch (Exception)
        {
            var json = Preferences.Get(UserKey, null);
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<UserInfo>(json);
        }
    }

    public async Task SetUserAsync(UserInfo user)
    {
        var json = JsonSerializer.Serialize(user);
        try
        {
            await SecureStorage.SetAsync(UserKey, json);
        }
        catch (Exception)
        {
            Preferences.Set(UserKey, json);
        }
    }

    public Task RemoveUserAsync()
    {
        try
        {
            SecureStorage.Remove(UserKey);
        }
        catch (Exception)
        {
            Preferences.Remove(UserKey);
        }
        return Task.CompletedTask;
    }
}
