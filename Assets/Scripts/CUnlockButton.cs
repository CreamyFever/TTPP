using UnityEngine;
using UnityEngine.UI;

public class CUnlockButton : MonoBehaviour
{
    public void OnButtonClick(Text text)
    {
        CLevelManager.Instance.OnLevelButtonClick(text);
    }
}
