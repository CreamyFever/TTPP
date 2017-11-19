using UnityEngine;
using UnityEngine.SceneManagement;

public class CAppManager : MonoBehaviour
{
    bool isLoadingPrevScene = false;

    private void OnDestroy()
    {
        Debug.Log("Destroy Scene");
        // Gameシーンはスタックに格納しない。
        if (PlayerPrefs.GetString(Constants.KEY_CURRENT_SCENE) == Constants.SCENE_GAME)
            return;

        // 前のシーンへ移るならここから抜ける。
        if(isLoadingPrevScene)
        {
            isLoadingPrevScene = false;
            return;
        }

        string name = SceneManager.GetActiveScene().name;
        CGlobalManager.PushScene(name);
    }


    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LoadPrevScene();
            }
        }
    }

    public void LoadPrevScene()
    {
        if (SceneTransition.Instance.Fadestate != FadeState.None)
            return;

        isLoadingPrevScene = true;
        string scene = CGlobalManager.PopScene();

        if (string.IsNullOrEmpty(scene))
        {
            Application.Quit();
        }
        else
        {
            SceneTransition.Instance.StartFadeOut(scene);
        }
    }

    public void OnPrevSceneButtonClick()
    {
        LoadPrevScene();
    }

    void OnQuitPopupYesButtonClick()
    {
        Application.Quit();
    }
}

