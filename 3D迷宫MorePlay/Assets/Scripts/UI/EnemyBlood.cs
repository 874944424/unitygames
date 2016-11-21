using UnityEngine;
using System.Collections;

public class EnemyBlood : MonoBehaviour
{
    public float npcHeight;         //模型高度
    public Texture2D blood_red;     //红色血条贴图
    public Texture2D blood_black;   //黑色血条贴图
    private int HP = 100;

    void Start ()
    {
        //得到模型高度
        //得到模型原始高度
        float size_y = GetComponent<Collider>().bounds.size.y;
        //得到模型缩放的比例
        float scal_y = transform.localScale.y;
        //它们的乘积是高度
        npcHeight = (size_y * scal_y);
    }
	
	void Update ()
    {

    }



    void OnGUI()
    {

        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + npcHeight, transform.position.z);
        Vector2 position = GameObject.FindGameObjectWithTag(Tags.playercamera).GetComponent<Camera>().WorldToScreenPoint(worldPosition);

        position = new Vector2(position.x, Screen.height - position.y);
        Vector2 bloodSize = GUI.skin.label.CalcSize(new GUIContent(blood_red));


        int blood_width = blood_red.width * HP / 100;

        GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2), position.y - bloodSize.y, bloodSize.x, bloodSize.y), blood_black);
        GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2), position.y - bloodSize.y, blood_width, bloodSize.y), blood_red);
    }
}
