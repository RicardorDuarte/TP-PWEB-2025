using SharedEntities.BD.Data;
namespace RCLGeral.Services.Auth;

public class AuthStateService
{
    public event Action? OnChange;
    private bool _isAuthenticated = false;

    public bool IsAuthenticated {
        get => _isAuthenticated;
        set {
            if (_isAuthenticated != value){
                _isAuthenticated = value;
                NotifyStateChanged();
            }
        }
    }

    private ApplicationUser? _user;

    public ApplicationUser User
    {
        get => _user!;
        set{
            if (_user != value){
                _user = value;
                NotifyStateChanged();
            }
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}