using UnityEngine;
using System.Collections;

public class MyNetwork : MonoBehaviour
{
    public int connections = 10;
    public int listenPort = 8899;
    public bool useNat = false;
    public string ip = "127.0.0.1";
    public GameObject cube_prefab;
    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUILayout.Button("创建服务器"))
            {
                //进行创建服务器的操作
                Network.incomingPassword = "test1234";
                NetworkConnectionError connect_error = Network.InitializeServer(connections, listenPort, useNat);
                print(connect_error);
            }
            if (GUILayout.Button("连接服务器"))
            {
                //连接服务器
                string password = "test1234";
                NetworkConnectionError connect_error = Network.Connect(ip, listenPort, password);
                print(connect_error);
            }
        }
        else if (Network.peerType == NetworkPeerType.Server)
        {
            GUILayout.Label("服务器创建完成");
        }
        else if (Network.peerType == NetworkPeerType.Client)
        {
            GUILayout.Label("客户端已经接入");
        }
    }

    //注意：这两个方法只在服务器调用
    void OnServerInitialized()
    {
        print("Server 完成初始化!");
        int group = int.Parse(Network.player+""); //直接访问Network.player会返回得到当前客户端,是唯一的索引
        Network.Instantiate(cube_prefab, new Vector3(0,10,0), Quaternion.identity, group);
    }
    void OnPlayerConnected(NetworkPlayer playerclient)
    {
        print("一个客户端连接过来，Index Number:"+ playerclient);
    }

    //注意：这两个方法只在客户端器调用
    void OnConnectedToServer()
    {
        int group = int.Parse(Network.player + ""); //直接访问Network.player会返回得到当前客户端,是唯一的索引
        Network.Instantiate(cube_prefab, new Vector3(0, 10, 0), Quaternion.identity, group);
        print("我成功连接到了服务器！");
    }
    //network view 组间用来在局域网之内同步一个游戏的组件属性
    //network view 会把创建出来它的客户端作为主人,主客户端.
}

