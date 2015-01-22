﻿using UnityEngine;
using System.Collections;
using StatusClass;

public sealed class EnemyStatusManager : MonoBehaviour
{
    /// <summary>
    /// ステータスクラス
    /// </summary>
    private Status status;
    /// <summary>
    /// 敵のタイプの列挙体
    /// </summary>
    public enum Type
    {
        enemy = 0,
        boss,
        hime,
    }
    /// <summary>
    /// 敵のタイプ
    /// </summary>
    public Type type;
    /// <summary>
    /// レベル
    /// </summary>
    [SerializeField]
    private int lv;
    /// <summary>
    /// 体力
    /// </summary>
//    [SerializeField]
//    private int hp;
    /// <summary>
    /// 魔力
    /// </summary>
//    [SerializeField]
//    private int mp;
    /// <summary>
    /// 経験値
    /// </summary>
//    [SerializeField]
//    private int exp;
    /// <summary>
    /// 死んだかどうか
    /// </summary>
    private bool isDead = false;

    /// <summary>
    /// 初期化
    /// </summary>
    void Awake()
    {
        switch (type)
        {
            case Type.enemy:
						status = new Status(lv, "Assets/EnemyTable.csv");
                break;
            case Type.boss:
						status = new Status(lv, "Assets/RastBassTable.csv");
                break;
            case Type.hime:
						status = new Status(lv, "Assets/RastBassTable.csv");
                break;
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="value">ダメージ量</param>
    public void Damage(int value)
    {
        this.status.HP -= value;
        if (this.status.HP <= 0)
        {
            isDead = true;
            this.status.HP = 0;
        }
    }

    /// <summary>
    /// 設定されたステータスを返す
    /// </summary>
    /// <returns></returns>
    public Status getStatus()
    {
        return status;
    }

    /// <summary>
    /// 死んだかどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool getIsDead()
    {
        return isDead;
    }
}