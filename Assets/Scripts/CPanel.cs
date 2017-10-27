using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTPP;

public class CPanel : MonoBehaviour
{
    public PanelInfo panelInfo;
    private PanelWave wave;
    public float accelerator;

    CPlayer player;

    public bool IsCrashed { get; private set; }


    /// <summary>
    /// オブジェクトプールにあるオブジェクトの使い回しなので、アクティブ化の際に呼び出すようにする。
    /// </summary>
    private void OnEnable()
    {
        IsCrashed = false;
        InitializePanel();
        accelerator = 0.0f;
        ChangePanelColor(panelInfo.subWaveIndex, wave);
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<CPlayer>();
        //StartCoroutine(PanelMove(panelInfo.subWaveIndex, wave));
    }

    private void Update()
    {
        PanelMoveMethod(panelInfo.subWaveIndex, wave);
    }

    void InitializePanel()
    {
        int level = CGameManager.Instance.PlayLevel;
        int currentWave = CGameManager.Instance.CurrentWave;
        wave = CGameManager.Instance.LevelDataList[level].waveList[currentWave];
    }

    /// <summary>
    /// パネルが地面に当たったら呼び出す。
    /// </summary>
    public void OnCrash()
    {
        IsCrashed = true;

        gameObject.SetActive(false);

        CSoundManager.Instance.PlayEffectSound(Constants.SOUND_ID_PANEL_COLLIDE);

        player.OnDamaged(panelInfo.panelVolume);
        player.wavePanelCount++;
    }
    
    void PanelMoveMethod(int index, PanelWave wave)
    {
        PanelSubWave subWave = wave.subWaveList[index];

        DecideMovePattern(subWave);
    }

    void DecideMovePattern(PanelSubWave subWave)
    {
        switch(subWave.info.moveType)
        {
            case MoveType.Square:
                ConstantSpeed(subWave.info.moveSpeed);
                break;

            case MoveType.Triangle:
                AccelerateSpeed(subWave.info.moveSpeed, 10.0f);
                break;

            case MoveType.Leaf:
                WaveMovement(subWave.info.moveSpeed, 3.0f, subWave.info.generatePos);
                break;

            case MoveType.Ring:
                StartCoroutine(RingPanelMove(subWave.info.moveSpeed));
                break;

            case MoveType.Shuriken:
                ConstantSpeed(subWave.info.moveSpeed);
                break;

            default:
                ConstantSpeed(subWave.info.moveSpeed);
                break;
        }
    }

    /// <summary>
    /// 普通に同じスピードで降ってくる。
    /// </summary>
    /// <param name="speed"></param>
    void ConstantSpeed(float speed)
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    /// <summary>
    /// どんどん加速度がついて降ってくる。
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="_maxSpeed"></param>
    void AccelerateSpeed(float speed, float _maxSpeed)
    {
        float maxSpeed = _maxSpeed;

        if(accelerator < maxSpeed)
        {
            accelerator += 0.1f;
        }

        transform.Translate(Vector3.down * speed * accelerator * Time.deltaTime);
    }

    /// <summary>
    /// 波形を描きながら降ってくる。
    /// </summary>
    /// <param name="speed">スピード</param>
    /// <param name="amplitude">振幅</param>
    /// <param name="offset">パネル生成座標の補正値</param>
    void WaveMovement(float speed, float amplitude, Vector2 offset)
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Sin(Time.time * amplitude * 1.5f) + offset.x;

        transform.position = pos;

        transform.Translate(Vector3.down * speed * amplitude * Time.deltaTime);
    }

    /// <summary>
    /// 動いたり止まったりするパネル
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    IEnumerator RingPanelMove(float speed)
    {
        float time = 0.0f;
        float timeLimit = 0.1f;

        while(true)
        {
            if (!gameObject.activeSelf)     // プレイヤーにタッチされたり、床についたらコルーチン終了
                yield break;
            
            if(time < timeLimit)
            {
                time += Time.deltaTime;
                ConstantSpeed(speed);
            }

            time = 0.0f;
            yield return new WaitForSeconds(2.0f);

            if (time < timeLimit)
            {
                time += Time.deltaTime;
                ConstantSpeed(-speed);
            }

            time = 0.0f;
            yield return new WaitForSeconds(2.0f);
        }
    }
    

    void ChangePanelColor(int index, PanelWave wave)
    {
        PanelSubWave subWave = wave.subWaveList[index];
        
        int spriteIndex = (int)subWave.info.moveType * 6 + (int)subWave.info.panelColor;

        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
            = CPanelManager.Instance.panelSprites[spriteIndex];
    }
}
