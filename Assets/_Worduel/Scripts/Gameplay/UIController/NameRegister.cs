using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

public class NameRegister : SingletonPersistent<NameRegister>
{
    [SerializeField] private GameObject _nameRegisterPanel, _nameRegisterMenu, _closeButton;
    [SerializeField] private TMP_InputField _usernameInput;

    public void Initialize()
    {
        Timing.RunCoroutine(CheckHaveRegister());
    }

    private IEnumerator<float> CheckHaveRegister()
    {
        if (!PlayerPrefs.HasKey(GameConstants.PLAYERPREFS_USERNAME))
        {
            UIManager.Instance.UICanvasGroup.blocksRaycasts = false;
            yield return Timing.WaitForSeconds(0.5f);

            ToggleNameRegisterPanel(true);
        }
    }

    public void ConfirmUsername()
    {
        if (!string.IsNullOrEmpty(_usernameInput.text))
        {
            PlayerPrefs.SetString(GameConstants.PLAYERPREFS_USERNAME, _usernameInput.text);
            PlayerPrefs.Save();

            ToggleNameRegisterPanel(false);
        }
    }

    public void ToggleNameRegisterPanel(bool setActive)
    {
        UIManager.Instance.PanelAnimation(_nameRegisterMenu, _nameRegisterPanel, setActive,
        onOpen: () =>
        {
            _closeButton.SetActive(PlayerPrefs.HasKey(GameConstants.PLAYERPREFS_USERNAME));
        },
        onClose: () =>
        {
            _usernameInput.text = string.Empty;
            LoadStats.Instance.UpdateName();
        });
    }
}
