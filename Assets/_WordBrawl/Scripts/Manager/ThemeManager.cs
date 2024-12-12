using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ThemeManager : SingletonPersistent<ThemeManager>
{
    public Theme CurrentTheme;
    public List<Theme> AllThemes { get; private set; } = new List<Theme>();
    public event Action OnThemeUnlocked;

    public void NotifyThemeUnlocked()
    {
        OnThemeUnlocked?.Invoke();
    }

    public async void Initialize()
    {
        PlayerPrefs.SetInt("Default Unlocked", 1);
        var currentTheme = PlayerPrefs.GetString(GameConstants.PLAYERPREFS_CURRENT_THEME, "Default");
        await LoadAllThemes();

        CurrentTheme = AllThemes.FirstOrDefault(theme => theme.Name == currentTheme);
        if (CurrentTheme != null)
        {
            Debug.Log($"Current theme set to: {CurrentTheme.Name}");
        }
        else
        {
            Debug.LogError("Failed to find the current theme. Default theme will be used.");
        }
    }

    public async Task LoadAllThemes()
    {
        try
        {
            var handle = Addressables.LoadAssetsAsync<Theme>("Theme", null);
            var themes = await handle.Task;

            if (themes != null && themes.Count > 0)
            {
                AllThemes = themes.OrderByDescending(theme => theme.name == "Default").ToList();

                Debug.Log($"Successfully loaded {AllThemes.Count} themes.");
            }
            else
            {
                Debug.LogError("Failed to load theme list or no themes found.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading themes: {ex.Message}");
        }
    }

    public void ChangeTheme()
    {
        CurrentTheme = ThemeSelector.Instance.GetCurrentTheme();
        ThemeSelector.Instance.UpdateStatus();
        PlayerPrefs.SetString(GameConstants.PLAYERPREFS_CURRENT_THEME, CurrentTheme.Name);
        UIManager.Instance.ToggleThemePanel(false);

        Debug.Log($"Current theme set to {CurrentTheme.Name}");
    }
}
