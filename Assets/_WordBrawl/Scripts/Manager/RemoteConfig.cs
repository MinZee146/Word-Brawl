using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;

public class RemoteConfig : SingletonPersistent<RemoteConfig>
{
    public int InitialCoins = 200;
    public int InitialHints = 5;
    public int RewardCoins = 20;
    public float AIDifficulty = 0.3f;

    struct UserAttributes { }
    struct AppAttributes { }

    async Task Initialize()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async Task FetchData()
    {
        await Initialize();

        var fetchCompletion = new TaskCompletionSource<bool>();

        RemoteConfigService.Instance.FetchCompleted += configResponse =>
        {
            ApplyRemoteSettings(configResponse);
            fetchCompletion.SetResult(true);
        };

        RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());

        await fetchCompletion.Task;
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        InitialCoins = RemoteConfigService.Instance.appConfig.GetInt("InitialCoins");
        InitialHints = RemoteConfigService.Instance.appConfig.GetInt("InitialHints");
        RewardCoins = RemoteConfigService.Instance.appConfig.GetInt("RewardCoins");
        AIDifficulty = RemoteConfigService.Instance.appConfig.GetFloat("AIDifficulty");

        UnityEngine.Debug.Log($"InitialCoins: {InitialCoins}, InitialHints: {InitialHints}, RewardCoins: {RewardCoins}, AIDifficulty: {AIDifficulty}");
    }
}
