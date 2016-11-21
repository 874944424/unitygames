using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMenuManager : MonoBehaviour
{
    public static LoadMenuManager _instance;

    public GameObject logo;
    public GameObject menu;


    void Awake()
    {
        _instance = this;
    }

    void Start()
    {

    }

    public void OnJoinSingleButtonClick()
    {
        PlayerPrefs.SetInt("Thebolder", 9);
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnJoinMulitButtonClick()
    {
        SceneManager.LoadScene(2);
    }
    public void OnJoinMulitSeverButtonClick()
    {
        SceneManager.LoadScene(3);
    }
}
