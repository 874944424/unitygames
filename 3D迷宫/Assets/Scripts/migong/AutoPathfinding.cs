using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AutoPathfinding :MonoBehaviour
{
    private int[] endXY;
    private int widthLimit;
    private int heightLimit;

    [HideInInspector]
    public node[,] map;    //存放当前地图信息

    private RandCreatePrim randcreateprim;
    private bool isRead = false;            //是否读取了完整的地图信息

    public GameObject pointerPre;       //指示物的模型
    public GameObject points_parentPre;

    void Start()
    {
        randcreateprim = this.transform.GetComponent<RandCreatePrim>();
        AtStartSetValue();
    }

    void FixedUpdate()
    {
        if (randcreateprim.map_dict.Count >= widthLimit * heightLimit && !isRead)
        {
            isRead = true;
            ReadMaps();
        }
    }

    //开始寻路
    public void AutoFindWay(int startX, int startY)
    {
        //定义出发位子
        Point pa = new Point();
        pa.x = startX;
        pa.y = startY;

        //定义目的地
        Point pb = new Point();
        pb.x = widthLimit * 2 - 1;
        pb.y = heightLimit * 2;

        if (pa.x >= pb.x || pa.y >= pb.y)
        {
            return;
        }

        ClearWay();
        FindWay(pa, pb);
        Showtheway();
    }

    //清空上次寻路记忆
    void ClearWay()
    {
        for (int i = 0; i < widthLimit * 2 + 1; i++)
            for (int j = 0; j < heightLimit * 2 + 1; j++)
            {
                if (map[i, j] == node.truepass)
                {
                    map[i, j] = node.pass;
                }
            }
        open_List.Clear();
        close_List.Clear();
    }

    //变量初始化
    void AtStartSetValue()
    {
        endXY = randcreateprim.endXY;
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
            }
    }

    //TODOA星寻路
    //开启列表
    List<Point> open_List = new List<Point>();
    //关闭列表
    List<Point> close_List = new List<Point>();

    //从开启列表查找F值最小的节点
    private Point GetMinFFromOpenList()
    {
        Point pmin = null;
        foreach (Point p in open_List)
        {
            if (pmin == null || pmin.G + pmin.H >  p.G + p.H)
                pmin = p;
        }
        return pmin;
    }

    //判断一个点是否为障碍物
    private bool IsBar(Point p, node[,] map)
    {
        if (map[p.x, p.y] == node.wall)
            return true;
        return false;
    }

    //判断关闭列表是否包含一个坐标的点
    private bool IsInCloseList(int x, int y)
    {
        foreach (Point p in close_List)
        {
            if (p.x == x && p.y == y)
                return true;
        }
        return false;
    }

    //从关闭列表返回对应坐标的点
    private Point GetPointFromCloseList(int x, int y)
    {
        foreach (Point p in close_List)
            if (p.x == x && p.y == y)
                return p;
        return null;
    }

    //判断开启列表是否包含一个坐标的点
    private bool IsInOpenList(int x, int y)
    {
        foreach (Point p in open_List)
            if (p.x == x && p.y == y)
                return true;
        return false;
    }

    //从开启列表返回对应坐标的点
    private Point GetPointFromOpenList(int x, int y)
    {
        foreach (Point p in open_List)
            if (p.x == x && p.y == y)
                return p;
        return null;
    }

    //计算某个点的G值(距离出发点的权重)
    private int GetG(Point p)
    {
        if (p.father == null)   //出发点
            return 0;
        if (p.x == p.father.x || p.y == p.father.y)
        {
            return p.father.G + 10;
        }
        else
            return p.father.G + 14; //上个点的斜对角
    }

    //计算某个点的H值 （距离终点的权重）
    private int GetH(Point p, Point pb)
    {
        return Math.Abs(p.x - pb.x) + Math.Abs(p.y - pb.y);
    }

    //检查当前节点附近的节点
    private void CheckPointNear(Point p0, node[,] map, Point pa, ref Point pb)
    {
        for (int xt = p0.x - 1; xt <= p0.x + 1; xt++)
        {
            for (int yt = p0.y - 1; yt <= p0.y + 1; yt++)
            {
                //排除超过边界和等于自身的点
                if ((xt >= 0 && xt <= widthLimit*2 && yt >= 0 && yt <= heightLimit*2) 
                    && !(xt == p0.x && yt == p0.y))
                {
                    //排除超过边界和关闭列表的点
                    if (map[xt, yt] != node.wall && !IsInCloseList(xt, yt))
                    {
                        if (IsInOpenList(xt, yt))
                        {
                            Point pt = GetPointFromOpenList(xt, yt);
                            int G_new = 0;
                            if (p0.x == pt.x || p0.y == pt.y)
                                G_new = p0.G + 10;
                            else
                                G_new = p0.G + 14;
                            if (G_new < pt.G)
                            {
                                open_List.Remove(pt);
                                pt.father = p0;
                                pt.G = G_new;
                                open_List.Add(pt);
                            }
                        }
                        else
                        {
                            //不在开启列表中
                            Point pt = new Point();
                            pt.x = xt;
                            pt.y = yt;
                            pt.father = p0;
                            pt.G = GetG(pt);
                            pt.H = GetH(pt, pb);
                            open_List.Add(pt);
                        }
                    }
                }
            }
        }
    }

    //查询一条通路
    public void FindWay(Point pa, Point pb)
    {
        open_List.Add(pa);
        while (!(IsInOpenList(pb.x, pb.y) || open_List.Count == 0))
        {
            Point p0 = GetMinFFromOpenList();
            if (p0 == null) return;
            open_List.Remove(p0);
            close_List.Add(p0);
            CheckPointNear(p0, map, pa, ref pb);
        }

        //进行筛选把真确的道路标识出来
        Point p = GetPointFromOpenList(pb.x, pb.y);
        //Debug.Log(close_List.Count);
        while (p.father != null)
        {
            p = p.father;
            map[p.x, p.y] = node.truepass;
        }
        map[pb.x, pb.y] = node.truepass;
    }
    //保存通路
    public void SaveWay(Point pb)
    {
        Point p = pb;
        while (p.father != null)
        {
            p = p.father;
            map[p.x, p.y] = node.truepass;
        }
    }

    public void Showtheway()
    {
        GameObject points_parent = Instantiate(points_parentPre);
        for (int i = 0; i < widthLimit * 2 + 1; i++)
            for (int j = 0; j < heightLimit * 2 + 1; j++)
            {
                if (map[i, j] == node.truepass)
                {
                    //生成指示物
                    GameObject obj_point = Instantiate(pointerPre, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
                    obj_point.transform.parent = points_parent.transform;
                }
            }
        points_parent.transform.localScale = new Vector3(10, 10, 10);
        points_parent.transform.position = new Vector3(0, -5, 0);
        Destroy(points_parent, 2f);
    }
}

//A星算法的点信息（权重）
public class Point
{
    public int x;
    public int y;
    public int G;
    public int H;

    public Point father;    //组合路径上一节点

    public Point()
    { }

    public Point(int x0, int y0, int G0, int H0, Point F)
    {
        x = x0;
        y = y0;
        G = G0;
        H = H0;
        father = F;
    }
}
