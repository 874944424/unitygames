using UnityEngine;
using System.Collections;

public class logoController : MonoBehaviour
{
    private GameObject menu_planel;

    void Awake()
    {
        menu_planel = transform.parent.GetChild(1).gameObject;
    }

    void Start()
    {
        menu_planel.SetActive(false);
    }

    public void OnLogoPalyOver()
    {
        menu_planel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
