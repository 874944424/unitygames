using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager_Custom : NetworkManager
{
    public void Start()
    {
        SetupMenuSceneButtons();
    }

    public void StartupHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }
    void SetIPAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void OnLevelWasLoaded(int level)    //unity_function
    {
        if (level == 3)
        {
            SetupMenuSceneButtons();
        }
        else if(level == 4)
        {
            SetupPlaySceneButtons();
        }
    }

    void SetupMenuSceneButtons()
    {
        GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(StartupHost);

        GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.AddListener(JoinGame);
        GameObject.Find("ButtonBackMenu").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonBackMenu").GetComponent<Button>().onClick.AddListener(GoBackmenu);
    }

    void SetupPlaySceneButtons()
    {
        GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(GoBacklocalhost);
    }

    public void GoBacklocalhost()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene(3);
        NetworkManager.singleton.StopHost();
    }

    public void GoBackmenu()
    {
        UnityEngine.Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
