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
            PlayerPrefs.DeleteAll();
            _nameRegisterPanel.SetActive(true);
            _nameRegisterPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack);
        }
        else
        {
            Debug.Log("hehe");
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

    public void OnValueChanged()
    {
        if (_usernameInput.text.Length > 9)
        {
            _usernameInput.text = _usernameInput.text.Substring(0, 8);
        }
    }
}
