using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum node       //创建迷宫时的格子类型
{
    unpass = 0,     //未处理的点
    pass = 1,       //为通路的点
    prepass = 3,    //支路打通时经过的点
    wall = 2,       //为墙的的点
    truepass = 4,   //正确的道路
    produce = 5,    //为怪物或物品产生的点
    truewayproduce = 6, //主路经上的支路死路
}
public class RandCreatePrim : MonoBehaviour
{
    /*
    * 自动生成迷宫 要点迷宫的格子全被墙隔断，
    * 随机选着其中一个进行通路打墙，
    * 如果选中的格子已经用过就中断当前通路
    * 如果没有使用的格子加入当前通路并加入列表打通墙体
    */

    [HideInInspector]
    public Dictionary<string, node> map_dict = new Dictionary<string, node>();         //地图信息
    public GameObject wallPrefabs;      //创建墙壁的模型
    public GameObject groundPrefabs;       //创建地面的模型
    public GameObject parentobj;        //地面和墙面选择父物体.
    public int[] startXY = { 1, 1};
    public int[] endXY = { 6, 6 };
    public int widthLimit = 6;
    public int heightLimit = 6;

    public List<GameObject> producePres = new List<GameObject>();   //随机生成物模型
    private GameObject produceparent;   //随机生成物的父物体方便管理

    void Start()
    {
        parentobj = GameObject.Find("WallsGrounds_parent");
        produceparent = GameObject.Find("produce_parent");
        CreatePrefabMap();
    }

    ///<summary>
    ///普利姆迷宫生成
    /// </summary>
    /// <param name = "startX">起始点X坐标</param>
    /// <param name = "startY">起始点Y坐标</param>
    /// <param name = "widthLimit">迷宫宽度</param>
    /// <param name = "heightLimit">迷宫高度</param>
    /// <param name="haveBorder">迷宫是否含有墙</param>


    //生成全是墙封闭的墙面的格子地图
    private node[,] CreateEmptyMazemap(int widthLimit, int heitLimit)
    {
        node[,] mazemap = new node[widthLimit * 2 + 1, heitLimit * 2 + 1];
        //个数扩张一倍作为墙体添加
        for (int column = 0; column < widthLimit * 2; column ++)    //列
        {
            for (int row = 0; row < heitLimit * 2; row++)           //行
            {
                //分配墙还是通路
                if (column % 2 == 0 || row % 2 == 0)
                {
                    mazemap[row, column] = node.wall;
                }
                else
                {
                    mazemap[row, column] = node.unpass;
                }
            }
        }
        //补全围墙
        for (int column = 0; column <= widthLimit * 2; column++)
        {
            mazemap[widthLimit * 2, column] = node.wall;
        }
        for (int row = 0; row <= heitLimit * 2; row++)
        {
            mazemap[row, heitLimit * 2] = node.wall;
        }

        return mazemap;
    }
    //打通连通的墙壁
    private node[,] MakeMazemap(int startX, int startY, int endX, int endY, int widthLimit, int heightLimit, node[,] empty_mazemap)
    {
        //存放未连通格子
        List<string> unpassnode = new List<string>();

        for (int row = 0; row < heightLimit; row ++)
        {
            for (int column = 0; column < widthLimit; column++)
            {                
                string nodexy = (row * 2 + 1).ToString() + "," + (column * 2 + 1).ToString();
                unpassnode.Add(nodexy);
            }
        }

        List<string> way = new List<string>();

        int targetX = startX * 2 - 1;
        int targetY = startY * 2 - 1;
   
        unpassnode.Remove(targetX+","+targetY);
        unpassnode.Remove((endX * 2 - 1) + "," + (endY*2 -1));

        empty_mazemap[targetX, targetY] = node.pass;
        int count = 4; //方向选择判断次数
        int dection = RandomInt();  //方向
        while (targetX != endX * 2 - 1 || targetY != endY * 2 - 1)
        {
            //打通到迷宫出口的一条路
            if (dection == 0)   //上
            {
                if (targetX - 2 < 1 || empty_mazemap[targetX - 2, targetY] == node.pass || empty_mazemap[targetX - 2, targetY] == node.truewayproduce)
                {
                    dection++;
                    count--;
                }
                else
                { 
                    targetX -= 2;
                    empty_mazemap[targetX, targetY] = node.pass;
                    empty_mazemap[targetX + 1, targetY] = node.pass;
                    way.Add(targetX + "," + targetY);
                    unpassnode.Remove(targetX + "," + targetY);
                    count = 4;
                    dection = RandomInt();  //方向
                }
            }
            if (dection == 3) //右
            {
                if (targetY + 1 >= widthLimit * 2 || empty_mazemap[targetX, targetY + 2] == node.pass || empty_mazemap[targetX, targetY + 2] == node.truewayproduce)
                {
                    dection = 0;
                    count--;
                }
                else
                {
                    targetY += 2;
                    empty_mazemap[targetX, targetY] = node.pass;
                    empty_mazemap[targetX, targetY - 1] = node.pass;
                    way.Add(targetX + "," + targetY);
                    unpassnode.Remove(targetX + "," + targetY);
                    count = 4;
                    dection = RandomInt();  //方向
                }
            }
            if (dection == 2)    //下
            {
                if (targetX + 1 >= heightLimit * 2 || empty_mazemap[targetX + 2, targetY] == node.pass || empty_mazemap[targetX + 2, targetY] == node.truewayproduce)
                {
                    dection++;
                    count--;
                }
                else
                {
                    targetX += 2;
                    empty_mazemap[targetX, targetY] = node.pass;
                    empty_mazemap[targetX - 1, targetY] = node.pass;
                    way.Add(targetX + "," + targetY);
                    unpassnode.Remove(targetX + "," + targetY);
                    count = 4;
                    dection = RandomInt();  //方向
                }
            }
            if (dection == 1)   //左
            {
                if (targetY - 1 <= 0 || empty_mazemap[targetX, targetY - 2] == node.pass || empty_mazemap[targetX, targetY - 2] == node.truewayproduce)
                {
                    dection ++;
                    count--;
                }
                else
                {
                    targetY -= 2;
                    empty_mazemap[targetX, targetY] = node.pass;
                    empty_mazemap[targetX, targetY + 1] = node.pass;
                    way.Add(targetX + "," + targetY);
                    unpassnode.Remove(targetX + "," + targetY);
                    count = 4;
                    dection = RandomInt();  //方向
                }
            }
            /////四个方向都检查完毕后
            if (count < 0)
            {
                if (way.Count == 0)
                {
                    Debug.Log("一定是出错了，进入死路但是已经无路可退了");
                    break;
                }
                //判断后退节点是否为死路点
                CheckPoint(targetX, targetY, ref empty_mazemap);
                //Debug.Log("trueway produce point" + targetX + targetY + empty_mazemap[targetX, targetY]);

                //倒退到前一步
                way.RemoveAt(way.Count - 1);
                string nodeXY = way[way.Count - 1];
                int nodeindex = nodeXY.IndexOf(",", 0);
                targetX = int.Parse(nodeXY.Substring(0, nodeindex));
                targetY = int.Parse(nodeXY.Substring(nodeindex + 1));
                count = 4;
            }
        }
        empty_mazemap[(endX * 2 - 1), (endY * 2 - 1)] = node.pass;

        //打通其余墙面
        MakeThroughtMap(unpassnode, ref empty_mazemap);

        return empty_mazemap;
    }
    private int RandomInt(int min = 0, int max = 4)
    {
        int result = UnityEngine.Random.Range(min, max * 3);
        result = (result + 1) % max;
        return result;
    }
    //创建实体迷宫
    private void CreatePrefabMap()
    {
        //检测保证输入的值是否有效
        if (widthLimit < 1)
        {
            widthLimit = 1;
        }
        if (heightLimit < 1)
        {
            heightLimit = 1;
        }
        if (startXY[0] < 1 || startXY[0] > widthLimit)
        {
            startXY[0] = UnityEngine.Random.Range(1, widthLimit);   //输出的为整数
        }
        if (startXY[1] < 1 || startXY[1] > heightLimit)
        {
            startXY[1] = UnityEngine.Random.Range(1, heightLimit);
        }
        if (endXY[0] < 1 || endXY[0] > widthLimit)
        {
            endXY[0] = widthLimit;
        }
        if (endXY[1] < 1 || endXY[1] > heightLimit)
        {
            endXY[1] = heightLimit;
        }

        //生成全是墙封闭的墙面的格子地图
        //N个格子需要打通N个墙面(连同所有的格子)
        //初始点是未格子中的任意一个格子
        //终点是连通格子（第一次默认是开始位置到终点）
        node[,] map = MakeMazemap(startXY[0], startXY[1], endXY[0], endXY[1], widthLimit, heightLimit, CreateEmptyMazemap(widthLimit, heightLimit));
       //初始墙体 和 结束位子墙体
        //map[1, 0] = node.pass;
        map[widthLimit * 2 - 1, heightLimit * 2] = node.pass;
        for (int i = 0; i < widthLimit * 2 + 1; i++)
        {
            for (int j = 0; j < heightLimit * 2 + 1; j++)
            {
                //Debug.Log(i + " " + j + " " + map[i, j].ToString());
                if (map[i, j] == node.wall)
                {
                    GameObject obj_node = Instantiate(wallPrefabs, new Vector3((float)i, 0, (float)j), Quaternion.identity) as GameObject;
                    obj_node.transform.parent = parentobj.transform;
                }
                //生成随机物
                if(map[i, j] == node.truewayproduce || map[i, j] == node.produce)
                     CreateProduce(i, j);
                //生成地面
                GameObject obj_ground = Instantiate(groundPrefabs, new Vector3((float)i, -1, (float)j), Quaternion.identity) as GameObject;
                obj_ground.transform.parent = parentobj.transform;

                //保存地图信息
                map_dict.Add(i + "," + j, map[i, j]);              
            }
        }

        parentobj.transform.localScale = new Vector3(10, 10, 10);
    }
    //检测后退的点处地图信息
    void CheckPoint(int x, int y, ref node[,] map, node node = node.truewayproduce)
    {
        //附近三面墙着为生成点
        int point_count = 3;    //记录附近不为空的点的位子
        if (map[(x - 1), y] != node.wall)
        {
            point_count--;
        }
        if (map[(x + 1), y] != node.wall)
        {
            point_count--;
        }
        if (map[x, (y - 1)] != node.wall)
        {
            point_count--;
        }
        if (map[x, (y + 1)] != node.wall)
        {
            point_count--;
        }

        if (point_count == 2)    //存在三面墙的点
        {
            map[x, y] = node;
        }
    }
    //生成--TODO生成更多的怪物和物品
    void CreateProduce(int x, int y)
    {
        int index_produce = UnityEngine.Random.Range(0, producePres.Count); //选着随机物品
        GameObject produce_obj = Instantiate(producePres[index_produce], new Vector3(x * 10, 0, y * 10), Quaternion.identity) as GameObject;
        produce_obj.transform.parent = produceparent.transform;
    }
    //打通剩余的网格通路
    void MakeThroughtMap(List<string> unpassnode, ref node[,] empty_mazemap)
    {
        //打通其余的格子  
        List<string> findway = new List<string>();
        while (unpassnode.Count > 0)   //判断格子是否全部连通
        {
            int index = RandomInt(0, unpassnode.Count);
            int node_index = unpassnode[index].IndexOf(",", 0);
            string nodeX = unpassnode[index].Substring(0, node_index);
            string nodeY = unpassnode[index].Substring(node_index + 1);
            int targetX = int.Parse(nodeX);
            int targetY = int.Parse(nodeY);
            findway.Clear();
            findway.Add(targetX + "," + targetY);

            int count = 4;
            int count_passnode = 4;  //降低passnode的优先级
            int dection = RandomInt();  //方向

            unpassnode.Remove((targetX) + "," + targetY);

            //注意判断的时候是跟随dection进行，可能上个节点成功后，接下的节点没有进行循环判断就跳到下一个dection

            while (empty_mazemap[targetX, targetY] != node.pass || empty_mazemap[targetX, targetY] != node.truewayproduce)
            {
                empty_mazemap[targetX, targetY] = node.prepass;
                unpassnode.Remove((targetX) + "," + targetY);
                if (dection == 0)   //上
                {
                    if (targetX - 1 <= 0 || empty_mazemap[targetX - 2, targetY] == node.prepass
                        || empty_mazemap[targetX - 2, targetY] == node.produce)
                    {
                        dection++;
                        count--;
                        count_passnode--;
                    }
                    else if (count_passnode > 0 && (empty_mazemap[targetX - 2, targetY] == node.pass || empty_mazemap[targetX - 2, targetY] == node.truewayproduce))
                    {
                        dection++;
                        count_passnode--;
                    }
                    else
                    {
                        empty_mazemap[targetX - 1, targetY] = node.prepass;
                        targetX = targetX - 2;
                        findway.Add(targetX + "," + targetY);
                        count = 4;
                        count_passnode = 4;
                        dection = RandomInt();  //方向

                        unpassnode.Remove((targetX) + "," + targetY);
                        if (empty_mazemap[targetX, targetY] == node.pass || empty_mazemap[targetX, targetY] == node.truewayproduce)
                            break;
                        else
                            empty_mazemap[targetX, targetY] = node.prepass;
                    }
                }
                if (dection == 1) //右
                {
                    if (targetY + 1 >= widthLimit * 2 || empty_mazemap[targetX, targetY + 2] == node.prepass
                        || empty_mazemap[targetX, targetY + 2] == node.produce)
                    {
                        dection++;
                        count--;
                        count_passnode--;
                    }
                    else if (count_passnode > 0 && (empty_mazemap[targetX, targetY + 2] == node.pass || empty_mazemap[targetX, targetY + 2] == node.truewayproduce))
                    {
                        dection++;
                        count_passnode--;
                    }
                    else
                    {
                        empty_mazemap[targetX, targetY + 1] = node.prepass;
                        targetY = targetY + 2;
                        findway.Add(targetX + "," + targetY);
                        count = 4;
                        dection = RandomInt();  //方向

                        unpassnode.Remove((targetX) + "," + targetY);
                        if (empty_mazemap[targetX, targetY] == node.pass || empty_mazemap[targetX, targetY] == node.truewayproduce)
                            break;
                        else
                            empty_mazemap[targetX, targetY] = node.prepass;
                    }
                }
                if (dection == 2)    //下
                {
                    if (targetX + 1 >= heightLimit * 2 || empty_mazemap[targetX + 2, targetY] == node.prepass
                        || empty_mazemap[targetX + 2, targetY] == node.produce)
                    {
                        dection++;
                        count--;
                        count_passnode--;
                    }
                    else if (count_passnode > 0 && (empty_mazemap[targetX + 2, targetY] == node.pass || empty_mazemap[targetX + 2, targetY] == node.truewayproduce))
                    {
                        dection++;
                        count_passnode--;
                    }
                    else
                    {
                        empty_mazemap[targetX + 1, targetY] = node.prepass;
                        targetX = targetX + 2;
                        findway.Add(targetX + "," + targetY);
                        count = 4;
                        dection = RandomInt();  //方向

                        unpassnode.Remove((targetX) + "," + targetY);
                        if (empty_mazemap[targetX, targetY] == node.pass || empty_mazemap[targetX, targetY] == node.truewayproduce)
                            break;
                        else
                            empty_mazemap[targetX, targetY] = node.prepass;
                    }
                }
                if (dection == 3)   //左
                {
                    if (targetY - 1 <= 0 || empty_mazemap[targetX, targetY - 2] == node.prepass
                        || empty_mazemap[targetX, targetY - 2] == node.produce)
                    {
                        dection = 0;
                        count--;
                        count_passnode--;
                    }
                    else if (count_passnode > 0 && (empty_mazemap[targetX, targetY - 2] == node.pass || empty_mazemap[targetX, targetY - 2] == node.truewayproduce))
                    {
                        dection = 0;
                        count_passnode--;
                    }
                    else
                    {
                        empty_mazemap[targetX, targetY - 1] = node.prepass;
                        targetY = targetY - 2;
                        findway.Add(targetX + "," + targetY);
                        count = 4;
                        dection = RandomInt();  //方向

                        unpassnode.Remove((targetX) + "," + targetY);
                        if (empty_mazemap[targetX, targetY] == node.pass || empty_mazemap[targetX, targetY] == node.truewayproduce)
                            break;
                        else
                            empty_mazemap[targetX, targetY] = node.prepass;
                    }
                }

                /////四个方向都检查完毕后
                if (count <= 0)
                {
                    //后退就为生成物品或怪物的点
                    CheckPoint(targetX, targetY, ref empty_mazemap, node.produce);
                    //Debug.Log("MAP produce point" + targetX + targetY + empty_mazemap[targetX, targetY]);
                    //倒退到前一步
                    findway.RemoveAt(findway.Count - 1);
                    if (findway.Count == 0)
                    {

                        Debug.Log("一定是出错了，进入死路但是已经无路可退了");
                        break;
                    }
                    string nodeXY = findway[findway.Count - 1];
                    int nodeindex = nodeXY.IndexOf(",", 0);
                    targetX = int.Parse(nodeXY.Substring(0, nodeindex));
                    targetY = int.Parse(nodeXY.Substring(nodeindex + 1));
                    count = 4;
                }
            }
            //通路换算成已通过
            for (int i = 0; i < findway.Count; i++)
            {
                string nodepassXY = findway[i];
                int index_pass = nodepassXY.IndexOf(",", 0);
                int nodepassX = int.Parse(nodepassXY.Substring(0, index_pass));
                int nodepassY = int.Parse(nodepassXY.Substring(index_pass + 1));

                empty_mazemap[nodepassX, nodepassY] = node.pass;
            }
            //出发点就为生成物品或怪物的点
            empty_mazemap[int.Parse(nodeX), int.Parse(nodeY)] = node.produce;

            //Debug.Log("MAP produce point" + nodeX + nodeY);
        }
    }

}
