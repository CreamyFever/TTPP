using System;
using System.Collections.Generic;
using UnityEngine;

namespace TTPP
{
    public enum MoveType
    {
        Leaf = 0,       // Wave
        Ring,           // StopNMove
        Shuriken,       // Random
        Square,         // Straight
        Triangle        // Diagonal
    }

    public enum PanelType
    {
        Normal = 0,
        NeedToTouch,
        Damage,
        Multiple
    }

    public enum PanelColor
    {
        Red = 0,
        Yellow,
        Green,
        Cyan,
        Blue,
        Purple
    }

    [Serializable]
    public class UnitInfo
    {
        public string name;
        public float healthPoint;
        public float attackPoint;
        public float shieldPoint;

        public float maxHealthPoint;
        public float maxAttackPoint;
        public float maxShieldPoint;
    }

    [Serializable]
    public class PlayerInfo : UnitInfo
    {
        public float score;

        public PlayerInfo() { }

        public PlayerInfo(string name, float healthPoint, float attackPoint, float shieldPoint)
        {
            this.name = name;
            this.maxHealthPoint = healthPoint;
            this.maxAttackPoint = attackPoint;
            this.maxShieldPoint = shieldPoint;

            this.healthPoint = maxHealthPoint;
            this.attackPoint = maxAttackPoint;
            this.shieldPoint = maxShieldPoint;

            score = 0.0f;
        }
    }

    [Serializable]
    public class EnemyInfo : UnitInfo
    {
        public int index;
        public Sprite sprite;
        public GameObject prefab;
        public int leftTurn;

        public EnemyInfo() { }

        public EnemyInfo(string name, float healthPoint, float attackPoint, float shieldPoint)
        {
            this.name = name;
            this.healthPoint = healthPoint;
            this.attackPoint = attackPoint;
            this.shieldPoint = shieldPoint;
        }
    }

    [Serializable]
    public class PanelInfo
    {
        public int subWaveIndex;

        public MoveType moveType;
        public PanelType panelType;
        public PanelColor panelColor;

        public float moveSpeed;
        public float score;
        public float scoreScale;
        public float panelVolume;

        public Vector2 generatePos;

        public PanelInfo() { }
    }

    [Serializable]
    public class LevelData
    {
        public int index;

        public LevelStatus status;

        public List<PanelWave> waveList = new List<PanelWave>();

    }

    [Serializable]
    public class PanelWave
    {
        public int waveIndex = -1;
        public float genTime = 15.0f;
        public List<PanelSubWave> subWaveList = new List<PanelSubWave>();

        public List<EnemyInfo> enemyList = new List<EnemyInfo>();
    }

    [Serializable]
    public class PanelSubWave
    {
        public int index = 0;
        public float delay = 0.0f;
        public int spawnCount = 1;
        public int spawnMax = 1;
        public float interval = 1.0f;

        public PanelInfo info = new PanelInfo();

        public PanelSubWave() { }
        public PanelSubWave(int index) { this.index = index; }
    }

    [Serializable]
    public class LevelStatus
    {
        public int index;
        public int clearCount;
        public int score;
        public int combo;
        public int bestScore;
        public int bestCombo;
    }
}
