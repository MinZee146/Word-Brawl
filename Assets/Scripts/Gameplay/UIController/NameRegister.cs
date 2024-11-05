using DG.Tweening;
using TMPro;
using UnityEngine;

public class NameRegister : SingletonPersistent<NameRegister>
{
    [SerializeField] private GameObject _nameRegisterPanel;
    [SerializeField] private TMP_InputField _usernameInput;

    public void Initialize()
    {
        if (!PlayerPrefs.HasKey("Username"))
        {
            _nameRegisterPanel.SetActive(true);
            _nameRegisterPanel.transform.GetChild(0).GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack);
        }
    }

    public void ConfirmUsername()
    {
        if (!string.IsNullOrEmpty(_usernameInput.text))
        {
            PlayerPrefs.SetString("Username", _usernameInput.text);
            PlayerPrefs.Save();

            _nameRegisterPanel.SetActive(false);
        }
    }
}
