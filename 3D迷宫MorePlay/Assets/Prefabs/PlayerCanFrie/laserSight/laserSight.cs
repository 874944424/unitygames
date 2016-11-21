using UnityEngine;
using System.Collections;

public class laserSight : MonoBehaviour
{
    public GameObject laserLinePrefab;
    public GameObject laserPointPrefab;
    public bool on;
    public bool disableRootCollider = true;

    private Transform laserPoint;
    private Transform laserPointOrigin;
    private float laserLineRate = 2f;
    private float nextLaserLineTime = 2f;
    private float positionBuffer = 2.0f;    //在结束前

    public GameObject player;
    public Camera _camera;
    public GameObject bulletdection;    //默认开火方向

    void Start()
    {
        on = true;
        laserPoint = GameObject.Instantiate(laserPointPrefab).transform;
        laserPointOrigin = transform.GetChild(1);
        player = GameObject.FindGameObjectWithTag(Tags.palyer);
        if((_camera = Camera.main) == null)
            _camera = GameObject.FindGameObjectWithTag(Tags.playercamera).GetComponent<Camera>();
    }

    void Update()
    {
        RaycastHit hit;
        float maxLength = 20f;
        if (disableRootCollider)
        {
            transform.root.GetComponent<Collider>().enabled = false;
        }
        if (player.GetComponent<Animation>().IsPlaying(AnimsNames.soldierIdle_animName))
        {
            //得到当前物体的目标
            Vector3 point_center = _camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            //TODO如果在移动相机中心的 +（-0.2, 0, 0）的位置。
            point_center += _camera.transform.forward * 1000;
            transform.LookAt(point_center);
        }
        else
        {
            transform.LookAt(bulletdection.transform.position);
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit) && on)
        {
            laserPoint.position = hit.point + hit.normal * 0.03f;
            laserPoint.GetComponent<laserPoint>().on = true;
            maxLength = Mathf.Min(maxLength, Vector3.Distance(transform.position, hit.point));
        }
        else
        {
            laserPoint.GetComponent<laserPoint>().on = false;
        }
        if (disableRootCollider)
        {
            transform.root.GetComponent<Collider>().enabled = true;
        }

        if (Time.time > nextLaserLineTime && on)
        {
            nextLaserLineTime = Time.time + (laserLineRate);
            GameObject newLaserLine = Instantiate(laserLinePrefab, transform.position, Quaternion.identity) as GameObject;
            newLaserLine.name = "laserLine";
            newLaserLine.transform.parent = transform;
            newLaserLine.transform.localRotation = Quaternion.identity;
            newLaserLine.transform.eulerAngles = new Vector3(newLaserLine.transform.eulerAngles.x + 90, newLaserLine.transform.eulerAngles.y, newLaserLine.transform.eulerAngles.z);
            if (maxLength < positionBuffer * 2.0)
            {
                newLaserLine.transform.localPosition = new Vector3(newLaserLine.transform.localPosition.x, newLaserLine.transform.localPosition.y, positionBuffer);
            }
            else
            {
                newLaserLine.transform.localPosition = new Vector3(newLaserLine.transform.localPosition.x, newLaserLine.transform.localPosition.y, Random.Range(positionBuffer, maxLength - positionBuffer));
            }
        }
        if (on)
        {
            laserPointOrigin.GetComponent<laserPoint>().on = true;
        }
        else
        {
            laserPoint.GetComponent<laserPoint>().on = false;
            laserPointOrigin.GetComponent<laserPoint>().on = false;
        }
        //Delete laser lines further than ray cast hit.
        if (maxLength > positionBuffer * 2)
        {
            for (var m = 0; m < transform.childCount; m++)
            {
                Transform child = transform.GetChild(m);
                if (child.localPosition.z > maxLength && child.name == "laserLine")
                {
                    Destroy(child.gameObject);
                }
            }
        }

    }

}