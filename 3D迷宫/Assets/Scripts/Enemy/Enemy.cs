using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    /* 所有敌人的父类包含共通的属性和动作 */

    protected float speed = 1f;    //移动速度
    protected int hp = 1;         //血量
    protected int attack;     //攻击力
    protected float attack_distance = 0;  //攻击距离
    protected float time_check = 0.005f;   //检测时间间隔
    protected float length;           //检测距离

    public GameObject player;      //保存发现玩家物体
    protected float Chase_time = 5f;         //倒计时追击时间
    protected float timer;                 //计时器

    protected GameObject checkobj;            //射线检测点
    protected GameObject checkorigin_obj;     //射线检测方向

    protected Animator anim;          //当前物体的动画状态

    protected bool IsCanAttack = false;       //是否可以进行攻击  
    protected bool IsCanWalk = true;         //是否可以走动防止滑步

    protected OwnerPlayer ownplayer;

    public Material on_hurtmaterial;                //红色材质
    [HideInInspector]
    public Material origin_material_enemy;            //没被打中时的怪物敌人的贴图
    public float movebackdistance = 1f;             //攻击击退距离

    public AudioClip dead_audio;                    //死亡音响
    private bool isPlayaudio = false;               //是否播放死亡音效    

    public int max_hp;

    public virtual void Start()
    {
        checkobj = transform.GetChild(0).gameObject;
        checkorigin_obj = transform.GetChild(1).gameObject;
        anim = this.GetComponent<Animator>();
        origin_material_enemy = transform.GetComponentInChildren<Renderer>().material;
        StartCoroutine(CheckFindAplayer());
        StartCoroutine(MoveToPlayer());

        max_hp = hp;
    }

    public virtual void FixedUpdate()
    {
        CheckDead();
        PlayerOnEye();
    }

    //检测是否发现主角,(设定一段时内检测一次)
    public virtual IEnumerator CheckFindAplayer()
    {      
        while (player == null)
        {
            Vector3 checkPos = checkobj.transform.position;
            Vector3 direction = checkPos - checkorigin_obj.transform.position;
            length = 30f;

            RaycastHit hitinfo;
            bool isCollider = Physics.Raycast(checkorigin_obj.transform.position, direction, out hitinfo, length);
            if (isCollider)
            {
                Debug.DrawLine(transform.position, hitinfo.point);
                if (hitinfo.transform.tag == Tags.palyer)
                {
                    //Debug.Log("check player");
                    //TODO开始追击玩家
                   
                    player = hitinfo.transform.gameObject;
                    ownplayer = player.GetComponent<OwnerPlayer>();
                }
                if (hitinfo.transform.tag != Tags.enemy)
                {
                    isCollider = false;
                }
            }

            //yield return new WaitForSeconds(time_check);
            yield return new WaitForFixedUpdate();
        }
    }
    //TODO向主角移动
    public virtual IEnumerator MoveToPlayer()
    {
        while (true)
        {
            if (player != null)     //timer > 0 && 
            {
                ////计时追击
                //timer -= Time.deltaTime;
                //先移动到玩家所在方格，再向玩家移动
                Vector3 player_pos = player.transform.position;
                //int.parse()该方式对于浮点数会做无条件舍去，失去精确度。
                int point_posx = (int)((player_pos.x + 5) / 10f) * 10;
                int point_posz = (int)((player_pos.z + 5) / 10f) * 10;
                Vector3 point_pos = new Vector3(point_posx, transform.position.y, point_posz);
                //进入攻击玩家距离后后取消追击
                if (Vector3.Distance(player.transform.position, transform.position) <= attack_distance)
                {
                    anim.SetBool(AnimValueNames.move, false);
                    transform.LookAt(player.transform.position);
                    IsCanAttack = true;
                    IsCanWalk = false;
                    yield return new WaitForSeconds(time_check);
                    continue;
                }
                //怪物不在攻击动画状态
                else if (IsCanWalk)
                {
                    if (Mathf.Abs(transform.position.x - point_pos.x) <= 5f || Mathf.Abs(transform.position.z - point_pos.z) <= 5f)
                        point_pos = new Vector3(player_pos.x, transform.position.y, player_pos.z);
                    else
                    {
                        //Debug.Log((point_posx / 10) + "," + (int)((transform.position.z + 5) / 10f));
                        node temp;
                        GameObject.Find("Gamemanager").GetComponent<RandCreatePrim>().map_dict.TryGetValue((point_posx / 10) + "," + (int)((transform.position.z + 5) / 10f), out temp);
                        if (temp == node.pass)
                        {
                            point_pos = new Vector3(point_pos.x, transform.position.y, transform.position.z);
                        }
                        else
                        {
                            point_pos = new Vector3(transform.position.x, transform.position.y, point_pos.z);
                        }
                    }
                    IsCanAttack = false;
                    Vector3 temp_pos = Vector3.Lerp(transform.position, point_pos, speed * Time.deltaTime);
                    transform.position = temp_pos;
                    transform.LookAt(point_pos);
                    anim.SetBool(AnimValueNames.move, true);
                }
                else
                {
                    IsCanAttack = false;
                }
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
                anim.SetBool(AnimValueNames.move, false);
            }
        }
    }
    //TODO在一定距离时向主角攻击
    protected virtual void MakeDamage()
    {
    }
    //TODO受到玩家的攻击伤害
    public virtual void OnHurt(int damage)
    {
        hp -= damage;
        transform.GetComponentInChildren<Renderer>().material = on_hurtmaterial;
        Invoke("ComeBackOriginMaterial", 0.05f);
        if (player != null)
        {
            Vector3 moveback = (player.transform.position - transform.position).normalized * movebackdistance;
            transform.position = new Vector3(transform.position.x - moveback.x, transform.position.y, transform.position.z - moveback.z);
        }
        Time.timeScale = 0.5f;
    }

    public virtual void ComeBackOriginMaterial()
    {
        Time.timeScale = 1f;
        transform.GetComponentInChildren<Renderer>().material = origin_material_enemy;
        //CancelInvoke();
    }

    //死亡检测
    void CheckDead()
    {
        if (hp <= 0)
        {
            if (!isPlayaudio)
            {
                transform.GetComponent<AudioSource>().PlayOneShot(dead_audio);
                isPlayaudio = true;
            }
            anim.SetBool(AnimValueNames.death, true);
            transform.tag = Tags.dead;
            player = null;
        }
    }

    //检测玩家是否被锁定中
    void PlayerOnEye()
    {
        //检测物环绕
        if (player != null)
        {
            transform.LookAt(player.transform.position);

            if (Vector3.Distance(player.transform.position, transform.position) > attack_distance + 0.5f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    checkobj.transform.RotateAround(transform.position, Vector3.up, 200 * Time.deltaTime);
                    checkorigin_obj.transform.RotateAround(transform.position, Vector3.up, 200 * Time.deltaTime);
                }
            }
            else
            {
                timer = Chase_time;
            }
            if (player.GetComponent<OwnerPlayer>().hp <= 0)
            {
                player = null;
            }
        }
        else
        {
            checkobj.transform.RotateAround(transform.position, Vector3.up, 200 * Time.deltaTime);
            checkorigin_obj.transform.RotateAround(transform.position, Vector3.up, 200 * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, transform.rotation.y, 0);
        }
    }
}
