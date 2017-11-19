/// <summary>
/// オートコンプリートを便利に用いるためにかきました。 ex)Constants.XXX
/// </summary>
public struct Constants
{
    public const string RSC_FOLDER = "Resource/";
    
    public const string DATA_RSC_FOLDER = "Data";

    public const string LEVEL_DATA_RES_PATH = DATA_RSC_FOLDER + "/LevelData";


    #region SceneName
    public const string SCENE_TITLE = "Title";
    public const string SCENE_LOBBY = "Lobby";
    public const string SCENE_LOAD = "Load";
    public const string SCENE_GAME = "Game";
    #endregion

    public const string KEY_PLAY_MODE = "PLAY_MODE";
    public const string KEY_PLAY_LEVEL = "PLAY_LEVEL";
    public const string KEY_LEVEL_STATUS = "LEVEL_STATUS";

    public const string KEY_LEVEL_DATA = "LEVEL_DATA";
    public const string KEY_NEXT_SCENE = "NEXT_SCENE";
    public const string KEY_CURRENT_SCENE = "CURRENT_SCENE";

    public const int LEVEL_STATUS_LOCKED = -1;
    public const int LEVEL_STATUS_UNLOCKED = 0;

    #region UIBottomPanel
    public const string UI_CANVAS = "UICanvas";
    public const string UI_BOTTOM_PANEL = UI_CANVAS + "/BottomPanel";

    public const string UI_SHIELD_POINT_TEXT = UI_BOTTOM_PANEL + "/ShieldText";
    public const string UI_HEALTH_POINT_TEXT = UI_BOTTOM_PANEL + "/HPText";
    public const string UI_ATTACK_POINT_TEXT = UI_BOTTOM_PANEL + "/AttackPointValText";
    public const string UI_TOUCHED_PANEL_COUNT_TEXT = UI_BOTTOM_PANEL + "/TouchedPanelCountText";

    public const string UI_SHIELD_BAR = UI_BOTTOM_PANEL + "/ShieldBar";
    public const string UI_HEALTH_BAR = UI_BOTTOM_PANEL + "/HPBar";

    public const string UI_HEALTH_BAR_BACK = UI_BOTTOM_PANEL + "/HPBarBack";
    #endregion

    #region UIComboPanel
    public const string UI_COMBO_PANEL = UI_CANVAS + "/ComboPanel";
    public const string UI_COMBO_BOX = UI_COMBO_PANEL + "/Combo";
    public const string UI_COMBO_COUNT_TEXT = UI_COMBO_PANEL + "/ComboCountText";
    #endregion

    #region UITopPanel
    public const string UI_TOP_PANEL = UI_CANVAS + "/TopPanel";
    public const string UI_LEFT_PANELS_COUNT_TEXT = UI_TOP_PANEL + "/GenLeftPanelText";
    #endregion

    #region UICenter
    public const string UI_WARNING_IMAGE = UI_CANVAS + "/WarningImage";

    public const string UI_STAGE_CLEAR_PANEL = UI_CANVAS + "/StageClearPanel";
    public const string UI_STAGE_CLEAR_OK_BUTTON = UI_STAGE_CLEAR_PANEL + "/GoStageSelectButton";

    public const string UI_GAME_OVER_PANEL = UI_CANVAS + "/GameOverPanel";
    public const string UI_CONTINUE_BUTTON = UI_GAME_OVER_PANEL + "/ContinueButton";
    public const string UI_GO_STAGE_SELECT_BUTTON = UI_GAME_OVER_PANEL + "/GoStageSelectButton";
    #endregion

    #region UIEnemyPanel
    public const string UI_ENEMY_PANEL = UI_CANVAS + "/EnemyUIBack";
    public const int UI_ENEMY_SHIELD_BAR = 0;
    public const int UI_ENEMY_HEALTH_BAR = 1;
    public const int UI_ENEMY_LEFT_TURN_TEXT = 3;
    #endregion

    #region Sounds
    public const int SOUND_BGM_TITLE = 0;
    public const int SOUND_BGM_LOBBY = 1;
    public const int SOUND_BGM_GAME_FIELD = 2;
    public const int SOUND_BGM_BOSS = 3;
    public const int SOUND_BGM_RESULT = 4;

    public const int SOUND_ID_PANEL_TOUCH = 0;
    public const int SOUND_ID_PLAYER_ATTACK = 1;
    public const int SOUND_ID_ENEMY_ATTACK = 2;
    public const int SOUND_ID_ENEMY_DESTROYED = 3;
    public const int SOUND_ID_PANEL_COLLIDE = 4;
    public const int SOUND_ID_CLEAR = 5;
    public const int SOUND_ID_GAME_OVER = 6;
    public const int SOUND_ID_UI_BUTTON = 7;
    public const int SOUND_ID_BOSS_APPROACH = 8;
    public const int SOUND_ID_GOT_COMBO = 9;
    #endregion

}