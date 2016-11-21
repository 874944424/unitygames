using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIScale : MonoBehaviour
{
    public static Vector2 NativeResolution = new Vector2(1024, 768);

    private static float _guiScaleFactor = -1.0f;
    static List<Matrix4x4> stack = new List<Matrix4x4>();

    //未找到变量
    public bool _didResizeUI;

    #region Canvas中需要自适应的按钮类
    public RectTransform blood;
    public RectTransform armor;
    public RectTransform attack;
    public RectTransform armorTitle;
    public RectTransform attackTile;

    #endregion

    void Awake()
    {
        //设置选择控制显示对象
        //blood = GameObject.FindGameObjectWithTag(Tags.blood).GetComponent<RectTransform>();
    }

    void OnGUI()
    {
        BeginUIResizing();
    }

    public void BeginUIResizing()
    {
        Vector2 nativeSize = NativeResolution;
        _didResizeUI = true;
        stack.Add(GUI.matrix);
        Matrix4x4 m = new Matrix4x4();
        var w = (float)Screen.width;
        var h = (float)Screen.height;
        var aspect = w / h;
        var offset = Vector3.zero;
        if (aspect < (nativeSize.x / nativeSize.y))
        {
            //screen is taller 
            _guiScaleFactor = (Screen.width / nativeSize.x);
            offset.y += (Screen.height - (nativeSize.y * _guiScaleFactor)) * 0.5f;
        }
        else
        {
            //screen is wider
            _guiScaleFactor = (Screen.height / nativeSize.y);
            offset.x += (Screen.width - (nativeSize.x * _guiScaleFactor)) * 0.5f;
        }
        m.SetTRS(offset, Quaternion.identity, Vector3.one * _guiScaleFactor);
        GUI.matrix *= m;

        //TODo使得屏幕中的Quit按钮，Zoom按钮自适应
        // _guiScaleFactor 为目前缩放比例
        blood.localScale = _guiScaleFactor * Vector3.one;
        armor.localScale = _guiScaleFactor * Vector3.one;
        attack.localScale = _guiScaleFactor * Vector3.one;
        attackTile.localScale = _guiScaleFactor * Vector3.one;
        armorTitle.localScale = _guiScaleFactor * Vector3.one;
        if (GameObject.Find("Gift_Text") != null)
        {
            GameObject.Find("Gift_Text").transform.localScale = _guiScaleFactor * Vector3.one;
        }
    }

}
