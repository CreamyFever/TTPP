using UnityEngine;

public class CAppManager : MonoBehaviour
{

    private void Update()
    {
        QuitGame();   
    }

    void QuitGame()
    {
        if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}

