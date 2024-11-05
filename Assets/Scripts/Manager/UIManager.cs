using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private TMP_InputField _replaceLetter;

    public void LoadGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ToggleGameOverScreen()
    {
        _gameOverScreen.SetActive(!_gameOverScreen.activeSelf);
    }

    public void ToUpper()
    {
        _replaceLetter.text = _replaceLetter.text.ToUpper();
    }
}
