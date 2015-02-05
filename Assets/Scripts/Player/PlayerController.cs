﻿using UnityEngine;
using System.Collections;
using StatusClass;
using SkillClass;
using CSV;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// メインカメラ
    /// </summary>
    private GameObject mainCamera;
    /// <summary>
    /// ボス戦かどうか
    /// </summary>
    [SerializeField]
    private bool isBossBattle = false;
    /// <summary>
    /// ターゲットオブジェクト
    /// </summary>
    private GameObject TargetObject;
    /// <summary>
    /// 道中スピード
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private float RoadSpeed = 0;
    /// <summary>
    /// 通常移動スピード
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private float NormalSpeed = 0;
    /// <summary>
    /// 攻撃移動スピード
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private float AttackSpeed = 0;
    /// <summary>
    /// 死ねるかどうか（デバッグ用）
    /// </summary>
    [SerializeField]
    private bool canDead;
    /// <summary>
    /// 魔法ボール
    /// </summary>
    private GameObject MagicBallObject;
    /// <summary>
    /// 矢
    /// </summary>
    private GameObject ArrowObject;
    /// <summary>
    /// 魔法/弓発射位置
    /// </summary>
    [SerializeField]
    private GameObject ShotPoint;
    /// <summary>
    /// アニメーションコントローラ
    /// </summary>
    private Animator animator;
    /// <summary>
    /// 魔法を撃っているかどうか
    /// </summary>
    private bool isShotMagic = false;
    /// <summary>
    /// 魔法を一発打ったか(短時間に連続して出さないようにする)
    /// </summary>
    private bool isOneShotMagic = false;
    /// <summary>
    /// 矢を撃っているかどうか
    /// </summary>
    private bool isShotArrow = false;
    /// <summary>
    /// 矢を一発打ったか(短時間に連続して出さないようにする)
    /// </summary>
    private bool isOneShotArrow = false;
    /// <summary>
    /// 剣を振っているかどうか
    /// </summary>
    private bool isAttackSword = false;
    /// <summary>
    /// 剣スキルを一発打ったか(短時間に連続して出さないようにする)
    /// </summary>
    private bool isOneShotSword = false;
    /// <summary>
    /// 移動しているかどうか
    /// </summary>
    private bool isMove = false;
    /// <summary>
    /// base layerで使われる、アニメーターの現在の状態の参照
    /// </summary>
    private AnimatorStateInfo currentBaseState;
    //各アニメーションへのステート
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int runState = Animator.StringToHash("Base Layer.Run");
    static int swordState = Animator.StringToHash("Base Layer.Sword");
    static int magicState = Animator.StringToHash("Base Layer.Magic");
    static int arrowState = Animator.StringToHash("Base Layer.Arrow");
    static int DamageState = Animator.StringToHash("Base Layer.Damage");
    static int DeadState = Animator.StringToHash("Base Layer.Dead_01");
    /// <summary>
    /// Statusクラス
    /// </summary>
    public Status status;
    /// <summary>
    /// 死亡フラグ
    /// </summary>
    private bool deadFlag = false;
    /// <summary>
    /// 死亡フラグを建てたかどうか
    /// </summary>
    private bool isDeadFlag = false;
    /// <summary>
    /// ダメージを受けたかの判定
    /// </summary>
    private bool isDamage = false;
    /// <summary>
    /// LvUPのためのフラグ
    /// </summary>
    private bool LvUp = false;
    /// <summary>
    /// スキルクラス
    /// </summary>
    private Skill skill;

    /// <summary>
    /// 武器/剣
    /// </summary>
    private GameObject Weapon_Sword;
    /// <summary>
    /// 武器/杖
    /// </summary>
    private GameObject Weapon_Rod;
    /// <summary>
    /// 武器/弓
    /// </summary>
    private GameObject Weapon_Bow;
    /// <summary>
    /// ヒットエフェクト
    /// </summary>
    private GameObject HitEffect;
    /// <summary>
    /// 歩行エフェクト
    /// </summary>
    private GameObject RunSmokeEffect;
    /// <summary>
    /// マウスボタンの列挙体
    /// </summary>
    private enum MouseButtonEnum
    {
        LEFT_BUTTON = 0,
        RIGHT_BUTTON,
        CENTER_BUTTON,
    }
    /// <summary>
    /// マウスボタンの構造体
    /// </summary>
    private struct MouseButtonStruct
    {
        public bool right;
        public bool left;
        public bool center;
        public bool rightDown;
        public bool leftDown;
        public bool centerDown;
    }
    /// <summary>
    /// マウスボタン
    /// </summary>
    private MouseButtonStruct mouseButton;
    /// <summary>
    /// 武器の列挙体
    /// </summary>
    private enum WeaponEnum
    {
        SWORD = 0,
        MAGIC,
        BOW,
    }
    /// <summary>
    /// 移動ボタンの構造体
    /// </summary>
    private struct IsMoveButton
    {
        public bool forwerd;
        public bool back;
        public bool right;
        public bool left;
        public bool jump;
    }
    /// <summary>
    /// 移動ボタンが押されているか
    /// </summary>
    private IsMoveButton isMoveButton;
    /// <summary>
    /// 現在選択中の武器
    /// </summary>
    private int nowWeapon = 0;
    /// <summary>
    /// 現在当たり続けているオブジェクト接触点の法線
    /// </summary>
    private Vector3 nowCollissionStayNormalVec = Vector3.up;
    /// <summary>
    /// ジャンプ可能か
    /// </summary>
    private bool canJump = false;
    /// <summary>
    /// マネージャーオブジェクト
    /// </summary>
    private GameObject manager;
    /// <summary>
    /// 剣振り効果音
    /// </summary>
    public AudioClip SwordSe;
    /// <summary>
    /// 魔法効果音
    /// </summary>
    public AudioClip MagicSe;
    /// <summary>
    /// 弓効果音
    /// </summary>
	public AudioClip BowSe;
    /// <summary>
    /// 選択効果音
    /// </summary>
    public AudioClip SelectSe;
    /// <summary>
    /// 魔法がでないときの効果音
    /// </summary>
    public AudioClip NonMagicSe;
	/// <summary>
	/// 魔法オブジェクトインスタンス
	/// </summary>
	private GameObject magicInstance;
	/// <summary>
	/// 弓オブジェクトインスタンス
	/// </summary>
	private GameObject arrowInstance;
    /// <summary>
    /// 剣による攻撃が与えられるか
    /// </summary>
    private bool canSwordDamage = false;

    /// <summary>
    /// 剣の攻撃エフェクト
    /// </summary>
    private GameObject sword_trail;


	void Awake()
	{
		if (GameObject.FindGameObjectWithTag("Boss") != null)
        {
            TargetObject = GameObject.FindGameObjectWithTag("Boss");
        }
        else if (GameObject.FindGameObjectWithTag("Hime") != null)
        {
            TargetObject = GameObject.FindGameObjectWithTag("Hime");
        }
        MagicBallObject = Resources.Load("Prefab/MagicBall") as GameObject;
        ArrowObject = Resources.Load("Prefab/Arrow") as GameObject;
        animator = this.gameObject.GetComponent<Animator>();

        Weapon_Sword = GameObject.FindGameObjectWithTag("Weapon_Sword");
        Weapon_Rod = GameObject.FindGameObjectWithTag("Weapon_Rod");
        Weapon_Bow = GameObject.FindGameObjectWithTag("Weapon_Bow");

        HitEffect = Resources.Load("Prefab/HitEffect") as GameObject;

        RunSmokeEffect = Resources.Load("Prefab/RunSmoke") as GameObject;

        mainCamera = GameObject.Find("CameraControllPoint");

        manager = GameObject.Find("Manager");

        sword_trail = GameObject.Find ("Sword_Tral");
	
    }

    // Use this for initialization
    void Start()
    {
        StatusManager statusManager = new StatusManager();
        status = new Status(
            statusManager.getLoadStatus().LV,
            statusManager.getLoadStatus().EXP,
            statusManager.getLoadStatus().HP,
            statusManager.getLoadStatus().MP,
			statusManager.getLoadStatus().NAME, "Assets/LvTable.csv");
        Weapon_Sword.renderer.enabled = true;
        Weapon_Rod.renderer.enabled = false;
        Weapon_Bow.renderer.enabled = false;
    }

    void Update()
    {
        //マウスイベント
        MouseEvent();
        //武器切り替え
        WeaponChange();
        //アニメーション管理
        AnimationCheck();
        //ボタンイベント
        ButtonEvent();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //移動
        Move();
        //攻撃
        Attack();
    }

    /// <summary>
    /// 移動
    /// </summary>
    void Move()
    {
        if (!isDeadFlag &&
            !isAttackSword &&
            !isShotMagic &&
            !isShotArrow)
        {
            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");
            animator.SetFloat("Horizontal", inputH);
            isMove = false;
            Vector3 forward = mainCamera.GetComponent<CameraController>().getCameraDirection(Vector3.forward).normalized;
            Vector3 back = mainCamera.GetComponent<CameraController>().getCameraDirection(Vector3.back).normalized;
            Vector3 right = mainCamera.GetComponent<CameraController>().getCameraDirection(Vector3.right).normalized;
            Vector3 left = mainCamera.GetComponent<CameraController>().getCameraDirection(Vector3.left).normalized;
            Vector3 down = mainCamera.GetComponent<CameraController>().getCameraDirection(Vector3.down).normalized;
            float rotSpeed = 0.2f;
            //前
            if (isMoveButton.forwerd)
            {
                isMove = true;
                rigidbody.AddForce(forward * NormalSpeed, ForceMode.VelocityChange);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), rotSpeed);
            }
            //後ろ
            else if (isMoveButton.back)
            {
                isMove = true;
                rigidbody.AddForce(back * NormalSpeed, ForceMode.VelocityChange);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(back), rotSpeed);
            }
            //左
            if (isMoveButton.left)
            {
                isMove = true;
                rigidbody.AddForce(left * NormalSpeed, ForceMode.VelocityChange);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(left), rotSpeed);
            }
            //右
            else if (isMoveButton.right)
            {
                isMove = true;
                rigidbody.AddForce(right * NormalSpeed, ForceMode.VelocityChange);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(right), rotSpeed);
            }

            //ジャンプ
            if (isMoveButton.jump && canJump)
            {
                float Power = 150f;
                //法線に上方向ベクトル加算
                nowCollissionStayNormalVec += Vector3.up;
                rigidbody.AddForce(nowCollissionStayNormalVec.normalized * Power, ForceMode.VelocityChange);
                //ベクトル初期化
                nowCollissionStayNormalVec = Vector3.up;
                canJump = false;
            }

            //常に下方向に力をかける
            rigidbody.AddForce(Vector3.down * 3.5f, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    void Attack()
    {
        //skill = new Skill(ShotPoint.transform.position, this.transform.rotation, "Boss");
        currentBaseState = this.animator.GetCurrentAnimatorStateInfo(0);
		if (!isDeadFlag &&
		    currentBaseState.nameHash != DamageState &&
            mouseButton.left)
        {
            switch (nowWeapon)
            {
                case (int)WeaponEnum.SWORD:
                    isAttackSword = true;
                    break;
                case (int)WeaponEnum.MAGIC:
                    isShotMagic = true;
                    break;
                case (int)WeaponEnum.BOW:
                    isShotArrow = true;
                    break;
            }
        }
    }

    /// <summary>
    /// アニメーション管理
    /// </summary>
    void AnimationCheck()
    {
        if (LvUp == true)
        {
            AudioSource[] audioSource = GetComponents<AudioSource>();
            audio.PlayOneShot(audioSource[audioSource.Length - 1].clip);
            var particle = GetComponent<ParticleSystem>();
            particle.Play();
        }

        // 参照用のステート変数にBase Layer (0)の現在のステートを設定する
        currentBaseState = this.animator.GetCurrentAnimatorStateInfo(0);

        //立ちモーションの時
        if (currentBaseState.nameHash == idleState)
        {
            //LvUpフラグを初期化
            LvUp = false;
        }
        //走りモーションの時
        else if (currentBaseState.nameHash == runState)
        {

        }
        //剣モーションの時
        else if (currentBaseState.nameHash == swordState)
        {
            //攻撃フラグを初期化
            isAttackSword = false;
        }
        //魔法モーションの時
        else if (currentBaseState.nameHash == magicState)
        {
            //攻撃フラグを初期化
            isShotMagic = false;
        }
        //弓モーションの時
        else if (currentBaseState.nameHash == arrowState)
        {
            //攻撃フラグを初期化
            isShotArrow = false;
        }
        //ダメージモーションの時
        if (currentBaseState.nameHash == DamageState)
        {
            //ダメージフラグを初期化
            isDamage = false;
        }
        //死亡モーションの時
        if (currentBaseState.nameHash == DeadState)
        {
            deadFlag = false;
        }
        animator.SetBool("isMove", isMove);
        animator.SetBool("isAttackSword", isAttackSword);
        animator.SetBool("isShotMagic", isShotMagic);
        animator.SetBool("isShotArrow", isShotArrow);
        animator.SetBool("DeadFlag", deadFlag);
        animator.SetBool("isDamage", isDamage);
    }

    /// <summary>
    /// 走りイベント
    /// </summary>
    void RunSeTiming()
    {
        //Debug.Log("Se Play");
        Instantiate(RunSmokeEffect, this.transform.position, this.transform.rotation);
    }

    /// <summary>
    /// 剣攻撃開始イベント
    /// </summary>
    void SwordAttack_StartEvent()
    {
        sword_trail.GetComponent<TrailRenderer> ().enabled = true;
		audio.PlayOneShot(SwordSe);
        canSwordDamage = true;
    }

    /// <summary>
    /// 剣攻撃終了イベント
    /// </summary>
    void SwordAttack_EndEvent()
    {
        sword_trail.GetComponent<TrailRenderer> ().enabled = false;
        canSwordDamage = false;
    }

    /// <summary>
    /// 魔法攻撃イベント
    /// </summary>
    void MagicAttackEvent()
    {
        if (this.status.MP > 0)
        {
            this.status.MP -= 10;
            audio.PlayOneShot(MagicSe);
            isOneShotMagic = true;
            //ロックオンしていたら追従
            if (manager.GetComponent<AimCursorManager>().getLockOnObject() != null)
            {
                magicInstance = Instantiate(MagicBallObject, ShotPoint.transform.position, Quaternion.LookRotation(manager.GetComponent<AimCursorManager>().getLockOnObject().transform.position - this.transform.position)) as GameObject;
                magicInstance.GetComponent<MagicController>().setTargetObject(manager.GetComponent<AimCursorManager>().getLockOnObject());
            }
            else
            {
                Instantiate(MagicBallObject, ShotPoint.transform.position, Camera.main.transform.rotation);
            }
            MagicController.EnemyDamage = this.status.Magic_Power;
        }
        else
        {
            audio.PlayOneShot(NonMagicSe);
        }
        //Debug.Log(MagicController.EnemyDamage);
    }

    /// <summary>
    /// 弓攻撃イベント
    /// </summary>
    void BowAttackEvent()
    {
		audio.PlayOneShot(BowSe);
		isOneShotArrow = true;
        //ロックオンしていたら追従
		if (manager.GetComponent<AimCursorManager>().getLockOnObject() != null)
		{
			arrowInstance = Instantiate(ArrowObject, ShotPoint.transform.position, Quaternion.LookRotation(manager.GetComponent<AimCursorManager>().getLockOnObject().transform.position - this.transform.position)) as GameObject;
			arrowInstance.GetComponent<BowController>().setTargetObject(manager.GetComponent<AimCursorManager>().getLockOnObject());
		}
		else
		{
            Instantiate(ArrowObject, ShotPoint.transform.position, Camera.main.transform.rotation);
		}
		BowController.EnemyDamage = this.status.BOW_POW;
        //Debug.Log(MagicController.EnemyDamage);
    }

    /// <summary>
    /// マウスイベント
    /// </summary>
    void MouseEvent()
    {
        mouseButton.right = Input.GetMouseButton((int)MouseButtonEnum.RIGHT_BUTTON);
        mouseButton.left = Input.GetMouseButton((int)MouseButtonEnum.LEFT_BUTTON);
        mouseButton.center = Input.GetMouseButton((int)MouseButtonEnum.CENTER_BUTTON);
        mouseButton.rightDown = Input.GetMouseButtonDown((int)MouseButtonEnum.RIGHT_BUTTON);
        mouseButton.leftDown = Input.GetMouseButtonDown((int)MouseButtonEnum.LEFT_BUTTON);
        mouseButton.centerDown = Input.GetMouseButtonDown((int)MouseButtonEnum.CENTER_BUTTON);
    }

    /// <summary>
    /// ボタンイベント
    /// </summary>
    void ButtonEvent()
    {
        bool anyKey = Input.anyKey;
        //前
        if (Input.GetKey(KeyCode.W))
        {
            isMoveButton.forwerd = true;
            isMoveButton.back = false;
            isMoveButton.right = false;
            isMoveButton.left = false;
            isMoveButton.jump = false;
        }
        //後ろ
        else if (Input.GetKey(KeyCode.S))
        {
            isMoveButton.forwerd = false;
            isMoveButton.back = true;
            isMoveButton.right = false;
            isMoveButton.left = false;
            isMoveButton.jump = false;
        }
        else
        {
            isMoveButton.forwerd = false;
            isMoveButton.back = false;
            isMoveButton.right = false;
            isMoveButton.left = false;
            isMoveButton.jump = false;
        }
        //左
        if (Input.GetKey(KeyCode.A))
        {
            isMoveButton.forwerd = false;
            isMoveButton.back = false;
            isMoveButton.right = false;
            isMoveButton.left = true;
            isMoveButton.jump = false;
        }
        //右
        else if (Input.GetKey(KeyCode.D))
        {
            isMoveButton.forwerd = false;
            isMoveButton.back = false;
            isMoveButton.right = true;
            isMoveButton.left = false;
            isMoveButton.jump = false;
        }
        
        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMoveButton.forwerd = false;
            isMoveButton.back = false;
            isMoveButton.right = false;
            isMoveButton.left = false;
            isMoveButton.jump = true;
        }

        //for (int key = 0; key < 429; key++)
        //{
        //    if (Input.GetKey((KeyCode)key))
        //    {
        //        Debug.Log((KeyCode)key);
        //    }
        //}
    }

    /// <summary>
    /// 武器切り替え
    /// </summary>
    void WeaponChange()
    {


		if (Input.GetKeyDown(KeyCode.Alpha1) || nowWeapon == 0)
		{
			nowWeapon = (int)WeaponEnum.SWORD;
            //武器表示を設定
            Weapon_Sword.renderer.enabled = true;
            Weapon_Rod.renderer.enabled = false;
            Weapon_Bow.renderer.enabled = false;
        }
		if (Input.GetKeyDown(KeyCode.Alpha2) || nowWeapon == 1)
		{
			nowWeapon = (int)WeaponEnum.MAGIC;
            //武器表示を設定
            Weapon_Sword.renderer.enabled = false;
            Weapon_Rod.renderer.enabled = true;
            Weapon_Bow.renderer.enabled = false;
        }
		if (Input.GetKeyDown(KeyCode.Alpha3) || nowWeapon == 2)
		{
			nowWeapon = (int)WeaponEnum.BOW;
            //武器表示を設定
            Weapon_Sword.renderer.enabled = false;
            Weapon_Rod.renderer.enabled = false;
            Weapon_Bow.renderer.enabled = true;
        }
        if (mouseButton.rightDown) //Input.GetMouseButtonDown (1)
		{
            audio.PlayOneShot(SelectSe);
            Method.Selecting(ref nowWeapon, 3, "up");

		}
        //Debug.Log(nowWeapon);
    }

    /// <summary>
    /// 何かに当たったら（コリジョン）
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider.name);
        if (collision.collider.name == "toBossEventWall")
        {
            Application.LoadLevel("Boss");
        }
    }

    /// <summary>
    /// 何かに当たり続けたら(トリガー)
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerStay(Collider collider)
    { 
        if (collider.gameObject.CompareTag("Hime") && canSwordDamage)
        {
            //Instantiate(HitEffect, collider.transform.position, this.transform.rotation);
            var hime = collider.gameObject.GetComponent<RastBossController>();
            hime.GetComponent<EnemyStatusManager>().Damage(this.status.Sword_Power);
            canSwordDamage = false;
            //Debug.Log("Hit");
        }
        else if ((collider.gameObject.CompareTag("Enemy") ||
                collider.gameObject.CompareTag("Boss")) && 
                canSwordDamage)
        {
            //Instantiate(HitEffect, collider.transform.position, this.transform.rotation);
            var status = collider.gameObject.GetComponent<EnemyStatusManager>();
            status.Damage(this.status.Sword_Power);
            canSwordDamage = false;
            Debug.Log("Hit");
        }
    }

    /// <summary>
    /// 何かに当たり続けたら（コリジョン）
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionStay(Collision collision)
    {
        // 接触点の情報を取得
        foreach (ContactPoint contact in collision.contacts)
        {
            // 接触点の法線取得
            nowCollissionStayNormalVec = contact.normal;
            canJump = true;
        }
    }

    /// <summary>
    /// 外部から経験値取得を呼び出す関数
    /// </summary>
    /// <param name="exp">経験値</param>
    public void GetExp(int exp)
    {
        this.status.EXP += exp;
        if (this.status.EXP >= this.status.ExpLimit)
        {

            status.LevUp();
            LvUp = true;
            isMove = false;
            //攻撃フラグを初期化
            isAttackSword = false;
            isShotMagic = false;
            isShotArrow = false;
        }
    }

    /// <summary>
    /// 外部からダメージを呼び出す関数
    /// </summary>
    /// <param name="val">ダメージ値</param>
    public void Damage(int val)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        this.status.HP -= val;
        if (this.status.HP < 0)
        {
            this.status.HP = 0;
        }
        //HPが無くなったら死亡フラグを立てる
        if (this.status.HP <= 0 && !isDeadFlag && canDead)
        {
            this.deadFlag = true;
            isDeadFlag = true;
            Application.LoadLevelAdditive("GameOver");
            Time.timeScale = 0.3f;
        }
        //HPが残っていたらダメージフラグ
        else if (deadFlag != false)
        {
            this.isDamage = true;
        }
        Debug.Log(this.status.HP);
    }

    /// <summary>
    /// プレイヤーのステータス値を得る
    /// </summary>
    /// <returns></returns>
    public Status getStatus()
    {
        return status;
    }

    /// <summary>
    /// 現在使用中の武器を得る
    /// </summary>
    /// <returns></returns>
    public int getNowWeapon()
    {
        return nowWeapon;
    }
}