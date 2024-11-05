using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameRegister : MonoBehaviour
{
    [SerializeField] private GameObject _nameRegisterPanel;
    [SerializeField] private TMP_InputField _usernameInput;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Username"))
        {
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
