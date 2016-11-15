using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Gamemanager : MonoBehaviour
{
    [HideInInspector]
    public Gamemanager _instance;

    public GameObject PlayerPrefabs;
    private GameObject player;
    private bool isShowMinimap; //判断小地图是否显示
    private GameObject minimap; //小地图

    private GameObject points_parent;    //节点的父物体模型
    public bool isCanfindway = false;  //判断玩家是否拥有访问路径权限
    private bool isFinding = true;     //是否正在查询路径
    private float time;                 //计时查询路径权限

    public float time_monsters = 11f;        //计时怪物潮

    void Awake()
    {
        minimap = GameObject.Find("minimapRawImage");
        _instance = this;
    }
    void Start()
    {
        //终点生成提示
        player = Instantiate(PlayerPrefabs, new Vector3(1, 0, 1)*10, Quaternion.identity) as GameObject;
        minimap.SetActive(false);
        isShowMinimap = false;
        //每段时间产生怪物
        //StartCoroutine(MonstersComing());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShoworHideMinimap();
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ShoworHideMinimap();
        }
        if (Input.GetKeyDown(KeyCode.BackQuote) && isCanfindway)
        {
            isFinding = !isFinding;
        }
    }
    void FixedUpdate()
    {
        StartCoroutine(FindwayOut());
    }


    //小地图出现的时候玩家不可移动消失后才可以 移动。
    void ShoworHideMinimap()
    {
        minimap.SetActive(!minimap.activeSelf);
        isShowMinimap = !isShowMinimap;

        if (isShowMinimap)
        {
            player.GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            player.GetComponent<FirstPersonController>().enabled = true;
        }
    }

    //实时计算的自动寻路功能
    IEnumerator FindwayOut()
    {
        while (isFinding && isCanfindway)
        {
            //int.parse()该方式对于浮点数会做无条件舍去，失去精确度。
            Vector3 startpoint = player.transform.position;
            int startpointx = (int)((startpoint.x + 5) / 10f);
            int startpointy = (int)((startpoint.z + 5) / 10f);

            //startpoint = new Vector3(((int)((startpoint.x + 5) / 10f)), 0, ((int)((startpoint.z + 5) / 10f)));
            //Debug.Log(startpointx + " " + startpointy);

            transform.GetComponent<AutoPathfinding>().AutoFindWay(startpointx, startpointy);
            yield return new WaitForSeconds(1f);
            points_parent = GameObject.FindGameObjectWithTag("point");
            Destroy(points_parent);
            yield return new WaitForFixedUpdate();
        }
    }

    //怪物潮生成
    IEnumerator MonstersComing()
    {
        while (true)
        {
            yield return new WaitForSeconds(time_monsters);
            this.GetComponent<RandCreatePrim>().CreateProduce((int)(player.transform.position.x / 10), (int)(player.transform.position.z / 10));
            if (time_monsters > 7f)
                time_monsters--;
            yield return new WaitForFixedUpdate();
        }
    }
}
