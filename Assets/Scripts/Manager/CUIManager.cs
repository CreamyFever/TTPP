using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TTPP;
using SingletonPattern;

public class CUIManager : CSingletonPattern<CUIManager>
{
    #region BottomPanel
    Text shieldPointText;
    Text healthPointText;
    Text attackPointText;

    Text touchedPanelCountText;

    Image shieldBarImage;
    Image healthBarImage;
    Image healthBarBackImage;
    #endregion

    #region ComboPanel
    const int COMBO_BOX_COUNT = 3;
    Image comboPanel;
    Image[] comboImage = new Image[COMBO_BOX_COUNT];
    Text comboCountText;
    PanelColor touchedPanelColor;
    #endregion

    #region TopPanel
    Text genLeftPanelsText;
    #endregion

    #region CenterUI
    Image warningImage;
    #endregion

    #region EnemyUI
    const int ENEMY_UI_COUNT = 4;
    Transform[] enemyUITrans = new Transform[ENEMY_UI_COUNT];

    Image[] enemyUIPanel = new Image[ENEMY_UI_COUNT];
    Image[] enemyShieldBar = new Image[ENEMY_UI_COUNT];
    Image[] enemyHealthBar = new Image[ENEMY_UI_COUNT];
    Text[] leftTurnText = new Text[ENEMY_UI_COUNT];
    #endregion

    private void Awake()
    {
        shieldPointText = GameObject.Find(Constants.UI_SHIELD_POINT_TEXT).GetComponent<Text>();
        healthPointText = GameObject.Find(Constants.UI_HEALTH_POINT_TEXT).GetComponent<Text>();
        attackPointText = GameObject.Find(Constants.UI_ATTACK_POINT_TEXT).GetComponent<Text>();
        touchedPanelCountText = GameObject.Find(Constants.UI_TOUCHED_PANEL_COUNT_TEXT).GetComponent<Text>();

        shieldBarImage = GameObject.Find(Constants.UI_SHIELD_BAR).GetComponent<Image>();
        healthBarImage = GameObject.Find(Constants.UI_HEALTH_BAR).GetComponent<Image>();
        healthBarBackImage = GameObject.Find(Constants.UI_HEALTH_BAR_BACK).GetComponent<Image>();

        comboPanel = GameObject.Find(Constants.UI_COMBO_PANEL).GetComponent<Image>();
        comboCountText = GameObject.Find(Constants.UI_COMBO_COUNT_TEXT).GetComponent<Text>();
        for(int i = 0; i < COMBO_BOX_COUNT; i++)
        {
            comboImage[i] = GameObject.Find(Constants.UI_COMBO_BOX + (i+1).ToString()).GetComponent<Image>();
        }

        genLeftPanelsText = GameObject.Find(Constants.UI_LEFT_PANELS_COUNT_TEXT).GetComponent<Text>();

        warningImage = GameObject.Find(Constants.UI_WARNING_IMAGE).GetComponent<Image>();
        warningImage.gameObject.SetActive(false);

        for (int i = 0; i < ENEMY_UI_COUNT; i++)
        {
            enemyUITrans[i] = GameObject.Find(Constants.UI_ENEMY_PANEL + (i + 1).ToString()).transform;

            enemyUIPanel[i] = enemyUITrans[i].GetComponent<Image>();
            enemyShieldBar[i] = enemyUITrans[i].GetChild(Constants.UI_ENEMY_SHIELD_BAR).GetComponent<Image>();
            enemyHealthBar[i] = enemyUITrans[i].GetChild(Constants.UI_ENEMY_HEALTH_BAR).GetComponent<Image>();
            leftTurnText[i] = enemyUITrans[i].GetChild(Constants.UI_ENEMY_LEFT_TURN_TEXT).GetComponent<Text>();
        }
    }

    public void ShowTopUI(int panelCount)
    {
        genLeftPanelsText.text = panelCount.ToString();
    }

    public void ShowEnemyUI(EnemyInfo info, Vector3 pos)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(pos);
        Vector3 resultPos = new Vector3(viewPos.x * Camera.main.pixelWidth, (viewPos.y-0.1f) * Camera.main.pixelHeight, 0);
        enemyUITrans[info.index].position = resultPos;

        enemyShieldBar[info.index].fillAmount = info.shieldPoint / info.maxShieldPoint;
        enemyHealthBar[info.index].fillAmount = info.healthPoint / info.maxHealthPoint;

        leftTurnText[info.index].text = info.leftTurn.ToString();

        if (info.healthPoint <= 0.0f)
            enemyUITrans[info.index].gameObject.SetActive(false);
        else
            enemyUITrans[info.index].gameObject.SetActive(true);
    }

    public void ShowComboUI(int combo, int needToCombo)
    {
        Color color = GetColor();   // タッチしたパネルの色

        // コンボが途切れたらオフにする。
        if (combo == 0 && needToCombo == 0)
        {
            comboPanel.gameObject.SetActive(false);
            return;
        }

        else comboPanel.gameObject.SetActive(true);

        comboCountText.text = combo.ToString();

        for(int i = 0; i < needToCombo; i++)
        {
            comboImage[i].color = color;
        }
        for(int i = 2; i >= needToCombo; i--)
        {
            comboImage[i].color = Color.white;
        }
    }

    public void GetTouchedPanelColor(PanelColor color)
    {
        touchedPanelColor = color;
    }

    public void ShowBottomUI(PlayerInfo info, int touchedPanelCount)
    {
        shieldPointText.text = ((int)info.shieldPoint).ToString();
        healthPointText.text = ((int)info.healthPoint).ToString();
        attackPointText.text = ((int)info.attackPoint).ToString();
        touchedPanelCountText.text = touchedPanelCount.ToString();

        ShowBarImage(info);
    }
	
    void ShowBarImage(PlayerInfo info)
    {
        shieldBarImage.fillAmount = info.shieldPoint / info.maxShieldPoint;
        healthBarImage.fillAmount = info.healthPoint / info.maxHealthPoint;

        if (healthBarImage.fillAmount < 0.3f)
        {
            healthBarImage.color = Color.red;
            healthBarBackImage.color = Color.red;
        }
        else
        {
            healthBarImage.color = Color.white;
            healthBarBackImage.color = Color.white;
        }
    }

    Color GetColor()
    {
        Color result;

        switch (touchedPanelColor)
        {
            case PanelColor.Red:
                result = Color.red;
                break;
            case PanelColor.Yellow:
                result = Color.yellow;
                break;
            case PanelColor.Green:
                result = Color.green;
                break;
            case PanelColor.Cyan:
                result = Color.cyan;
                break;
            case PanelColor.Blue:
                result = Color.blue;
                break;
            case PanelColor.Purple:
                result = Color.magenta;
                break;
            default:
                result = Color.white;
                break;
        }

        return result;
    }

    public void ShowWarningUI(bool active)
    {
        warningImage.gameObject.SetActive(active);
    }
}
