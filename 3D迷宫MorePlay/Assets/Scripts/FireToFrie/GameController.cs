using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController _instance;
    public Text ip_address;

    public int connections = 10;    //最大连接数
    public int listenPort = 8899;   //监听端口
    public Transform pos1;
    public Transform pos2;
    public GameObject soldierPrefab;

    public GameObject logo;
    public GameObject menu;

    public AudioSource bgPlayMusic;
    public AudioSource bgMenuMusic;

    public Text gameover_showtext;
    public GameObject GameoverPanel;

    public GameObject stopPanel;

    public List<GameObject> soilder_objects = new List<GameObject>();        //保存玩家物体

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        GameoverPanel.SetActive(false);
        stopPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && soilder_objects.Count > 0)
        {
            stopPanel.SetActive(true);
            UnityEngine.Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //GameObject.FindGameObjectWithTag("Player").GetComponent<OwnerPlayer>().bullerGenertor.gameObject.GetComponent<CSBulletGenertor>().enabled = false;
            foreach (GameObject soilder in soilder_objects)
            {
                soilder.SetActive(false);
            }
        }
    }
    public void OnButtonCreateServerClick()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            Network.incomingPassword = "csgame1234";
            NetworkConnectionError connect_error = Network.InitializeServer(connections, listenPort);
            if (connect_error != NetworkConnectionError.NoError)
            {
                print(connect_error);
                return;
            }
            logo.SetActive(false);
            menu.SetActive(false);
            PlayMusic();
        }
    }
    void PlayMusic()
    {
        bgMenuMusic.Stop();
        bgPlayMusic.Play();
        this.GetComponent<AudioListener>().enabled = false;
    }
    public void OnButtonConnectServerClick()
    {
        string password = "csgame1234";
        NetworkConnectionError connect_error = Network.Connect(ip_address.text, listenPort, password);
        if (connect_error != NetworkConnectionError.NoError)
        {
            print(connect_error);
            return;
        }
        logo.SetActive(false);
        menu.SetActive(false);
        PlayMusic();
    }
    void OnServerInitialized()
    {
        CreateSoldier(pos1);
    }
    void OnConnectedToServer()
    {
        CreateSoldier(pos2);
    }
    void CreateSoldier(Transform pos)
    {
        NetworkPlayer player = Network.player;
        int group = int.Parse(player + "");
        GameObject go = Network.Instantiate(soldierPrefab, pos.position, Quaternion.identity, group) as GameObject;
        go.GetComponent<NetworkView>().RPC("SetOwnerPlayer", RPCMode.AllBuffered, Network.player);
        soilder_objects.Add(go);
    }
    public void OnQuitButtonClick()
    {
        UnityEngine.Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
    public void ShowGameOver(bool isWin)
    {
        UnityEngine.Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //显示游戏结束界面
        GameoverPanel.SetActive(true);
        
        if (!isWin)
        {
            gameover_showtext.text = "很遗憾你输掉了比赛！";
        }
        else
        {
            gameover_showtext.text = "恭喜你获得游戏胜利！";
        }
    }
    public void GobackGame()
    {
        UnityEngine.Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        stopPanel.SetActive(false);
        foreach (GameObject soilder in soilder_objects)
        {
            soilder.SetActive(true);
        }
        //GameObject.FindGameObjectWithTag("Player").GetComponent<OwnerPlayer>().bullerGenertor.gameObject.GetComponent<CSBulletGenertor>().enabled = true;
    }
}