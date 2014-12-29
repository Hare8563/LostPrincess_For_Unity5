﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;

public class StatusManager : MonoBehaviour {
    /// <summary>
    /// uGUIキャンバス
    /// </summary>
    public GameObject canvas;
    /// <summary>
    /// セーブ/ロードを行うクラス
    /// </summary>
    private PlayerPrefsEx prefs;
    /// <summary>
    /// ステータスの構造体
    /// </summary>
    public struct StatusStruct
    {
        public string NAME;
        public int HP;
        public int MP;
        public int LV;
        public int EXP;
        public int SWORD;
        public int BOW;
        public int MAGIC;
    };
    /// <summary>
    /// ステータスの構造体変数
    /// </summary>
    private StatusStruct statusStruct;

    /// <summary>
    /// プレイヤーオブジェクト
    /// </summary>
    private GameObject PlayerObject;
    
    /// <summary>
    /// 読み込み
    /// </summary>
    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
    }

	/// <summary>
	/// 初期化
	/// </summary>
	void Start () 
    {
        
	}
	
	/// <summary>
	/// 更新
	/// </summary>
	void Update () 
    {
        //GUI管理
        StatusGUIController();
	}

    /// <summary>
    /// ロード
    /// </summary>
    void Load()
    {
        //Debug.Log(System.Environment.CurrentDirectory);
        prefs = new PlayerPrefsEx();
        prefs.Load(System.Environment.CurrentDirectory + "/saveData.xml");
        statusStruct.NAME = prefs.GetString("NAME");// Debug.Log("NAME = " + statusStruct.NAME);
        statusStruct.HP = prefs.GetInt("HP"); //Debug.Log("HP = " + statusStruct.HP);
        statusStruct.MP = prefs.GetInt("MP"); //Debug.Log("MP = " + statusStruct.MP);
        statusStruct.LV = prefs.GetInt("LV"); //Debug.Log("LV = " + statusStruct.LV);
        statusStruct.EXP = prefs.GetInt("EXP"); //Debug.Log("EXP = " + statusStruct.EXP);
        statusStruct.SWORD = prefs.GetInt("Sword"); //Debug.Log("SWORD = " + statusStruct.SWORD);
        statusStruct.BOW = prefs.GetInt("Bow"); //Debug.Log("BOW = " + statusStruct.BOW);
        statusStruct.MAGIC = prefs.GetInt("Magic"); //Debug.Log("MAGIC = " + statusStruct.MAGIC);
    }

    /// <summary>
    /// ステータスウィンドウGUI
    /// </summary>
    void StatusGUIController()
    {
        PlayerController playerController = PlayerObject.GetComponent<PlayerController>();
        foreach (Transform child in canvas.transform)
        {
            //Debug.Log(child.name);
            if (child.name == "NAME")
            {
                child.gameObject.GetComponent<Text>().text = playerController.getPlayerStatus().NAME;//statusStruct.NAME;
            }
            else if (child.name == "HP")
            {
                child.gameObject.GetComponent<Text>().text = playerController.getPlayerStatus().HP.ToString();//statusStruct.HP.ToString();
            }
            else if (child.name == "MP")
            {
                child.gameObject.GetComponent<Text>().text = playerController.getPlayerStatus().MP.ToString();//statusStruct.MP.ToString();
            }
            else if (child.name == "LV")
            {
                child.gameObject.GetComponent<Text>().text = playerController.getPlayerStatus().LEV.ToString();//statusStruct.LV.ToString();
            }
        }
    }

    /// <summary>
    /// ロードしたステータスを得る
    /// </summary>
    /// <returns></returns>
    public StatusStruct getLoadStatus()
    {
        Load();
        return statusStruct;
    }
}