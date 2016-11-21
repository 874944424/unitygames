using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetProducePoints : MonoBehaviour
{
    private int widthLimit;
    private int heightLimit;
    private RandCreatePrim randcreateprim;
    private node[,] map;    //存放当前地图信息
    private List<string> passpoints = new List<string>();   //存放当前地图中的可通过的点
    private List<string> producepoints = new List<string>();    //存放当前地图中的生成点
    private bool isRead = false;            //是否读取了完整的地图信息

    public List<GameObject> producePres = new List<GameObject>();   //随机生成物模型
    private GameObject produceparent;   //随机生成物的父物体方便管理

    void Start()
    {
        randcreateprim = this.transform.GetComponent<RandCreatePrim>();
        AtStartSetValue();
        produceparent = GameObject.Find("produce_parent");
    }

    void FixedUpdate()
    {
        if (randcreateprim.map_dict.Count >= widthLimit * heightLimit && !isRead)
        {
            isRead = true;
            ReadMaps();
            FilterPoints();
            CreateProduce();
            ClearAfterUse();
        }
    }

    //变量初始化
    void AtStartSetValue()
    {
        widthLimit = randcreateprim.widthLimit;
        heightLimit = randcreateprim.heightLimit;
    }
    //地图信息读取(正确)
    void ReadMaps()
    {
        map = new node[widthLimit * 2 + 1, heightLimit * 2 + 1];
        for (int i = 0; i < widthLimit * 2 + 1; i++)
            for (int j = 0; j < heightLimit * 2 + 1; j++)
            {
                randcreateprim.map_dict.TryGetValue(i + "," + j, out map[i, j]);
                if (map[i, j] != node.wall)
                {
                    passpoints.Add(i + "," + j);
                }
            }
    }
    //筛选坐标留下可生成的位置点
    void FilterPoints()
    {
        for (int point_i = 0; point_i < passpoints.Count; point_i++)
        {
            int[] temp_pointXY = ReadpointToMap(passpoints[point_i]);
            //边界取消检测
            if (temp_pointXY[0] == widthLimit * 2 || temp_pointXY[1] == heightLimit * 2 || (temp_pointXY[0] == randcreateprim.startXY[0] && temp_pointXY[1] == randcreateprim.startXY[1]))
                continue;
            //附近三面墙着为生成点
            int point_count = 3;    //记录附近不为空的点的位子
            if (map[(temp_pointXY[0] - 1), temp_pointXY[1]] != node.wall)
            {
                point_count--;
            }
            if (map[(temp_pointXY[0] + 1), temp_pointXY[1]] != node.wall)
            {
                point_count--;
            }
            if (map[(temp_pointXY[0]), (temp_pointXY[1] - 1)] != node.wall)
            {
                point_count--;
            }
            if (map[(temp_pointXY[0]), (temp_pointXY[1] + 1)] != node.wall)
            {
                point_count--;
            }

            if (point_count == 2)    //存在三面墙的点
            {
                producepoints.Add(passpoints[point_i]);
                Debug.Log("GetProducePoints :" + passpoints[point_i]);
                passpoints.RemoveAt(point_i);
                //TODO随机产生怪物
                point_i--;
            }
        }
    }
    //把特定的List中的值转换为map上坐标位子信息
    private int[] ReadpointToMap(string pointXY)
    {
        int index_pass = pointXY.IndexOf(",", 0);
        int pointX = int.Parse(pointXY.Substring(0, index_pass));
        int pointY = int.Parse(pointXY.Substring(index_pass + 1));

        int[] result = new int[2];
        result[0] = pointX;
        result[1] = pointY;

        return result;
    }
    //随机生成物品或怪物
    private void CreateProduce()
    {
        int index_produce;
        int[] producepointXY;
        foreach (string point in producepoints)
        {
            producepointXY = ReadpointToMap(point);
            index_produce = Random.Range(0, producePres.Count); //选着随机物品
            GameObject produce_obj = Instantiate(producePres[index_produce], new Vector3(producepointXY[0] * 10, 0, producepointXY[1] * 10), Quaternion.identity) as GameObject;
            produce_obj.transform.parent = produceparent.transform;
        }
    }
    //清理当前无用数据
    private void ClearAfterUse()
    {
        passpoints.Clear();
        producepoints.Clear();
        passpoints = null;
        producepoints = null;

        transform.GetComponent<GetProducePoints>().enabled = false;
    }
}
