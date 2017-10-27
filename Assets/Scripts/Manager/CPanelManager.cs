using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonPattern;

using TTPP;

public class CPanelManager : CSingletonPattern<CPanelManager>
{
    public Sprite[] panelSprites;

    [SerializeField]
    GameObject m_panel;
    ObjectPool m_panelPool;

    [SerializeField]
    int m_panelCount;
    public int PanelCount
    {
        get { return m_panelCount; }
        set { m_panelCount = value; }
    }

    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(instance);

        panelSprites = Resources.LoadAll<Sprite>("Images/TapPanels");

        m_panelPool = gameObject.AddComponent<ObjectPool>();
        m_panelPool.InitPool(m_panel, m_panelCount);
    }

    
    public void GeneratePanels(PanelSubWave subwave)
    {
        GameObject panel;

        panel = m_panelPool.GetObject();

        if (panel == null)
            return;

        panel.transform.position = subwave.info.generatePos;

        SetPanelInfo(panel, subwave);
        panel.SetActive(true);
    }

    public void SetPanelInfo(GameObject panel, PanelSubWave subwave)
    {
        PanelInfo info = panel.GetComponent<CPanel>().panelInfo;
        info.subWaveIndex = subwave.index;
        info.panelColor = subwave.info.panelColor;
        info.moveType = subwave.info.moveType;
        info.panelType = subwave.info.panelType;
        info.moveSpeed = subwave.info.moveSpeed;
        info.score = subwave.info.score;
        info.scoreScale = subwave.info.scoreScale;
        info.panelVolume = subwave.info.panelVolume;
        info.generatePos = subwave.info.generatePos;
    }
}
