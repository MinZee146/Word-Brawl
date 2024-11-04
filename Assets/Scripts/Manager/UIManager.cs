using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonPersistent<UIManager>
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
