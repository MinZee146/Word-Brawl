using UnityEngine;

public class UIController : SingletonPersistent<UIController>
{
    [SerializeField] private GameObject _hintButton, _confirmButton;

    public RectTransform ConfirmButtonRect()
    {
        return _confirmButton.GetComponent<RectTransform>();
    }

    public void ToggleHintAndConfirm(bool hintState = true, bool display = true)
    {
        if (!display)
        {
            _hintButton.SetActive(false);
            _confirmButton.SetActive(false);
            return;
        }

        if (true) //
        {
            _hintButton.SetActive(hintState);
            _confirmButton.SetActive(!hintState);
        }
        else
        {
            _hintButton.SetActive(false);
            _confirmButton.SetActive(false);
        }
    }

}