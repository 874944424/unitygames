using UnityEngine;
using System.Collections;

public class SoildAnimation : MonoBehaviour
{
    private SoildPlayer player;
    private bool havePlayDieAnimation = false;  //是否已经播放过死亡动画
    private CharacterController characterController;
    private Animation anim;
    void Awake()
    {
        player = this.GetComponent<SoildPlayer>();
        anim = this.GetComponent<Animation>();
        characterController = this.GetComponent<CharacterController>();
    }
    void Start()
    {

    }

    void Update()
    {
        if (player.hp <= 0)
        {
            if (!havePlayDieAnimation)
            {            
                havePlayDieAnimation = true;
                PlayState(AnimsNames.soldierDie_animName);
            }
        }
        else
        {
            if (characterController.isGrounded == false)
            {
                //播放下的动画
                PlayState(AnimsNames.soldierFalling_animName);
                
            }
            else
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                if (Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f)
                {
                    PlayState(AnimsNames.soldierRun_animName);                 
                }
                else
                {
                    PlayState(AnimsNames.soldierIdle_animName);
                }
            }
        }
    }

    void PlayState(string animName)
    {
        anim.CrossFade(animName, 0.2f);//会有0.2秒的缓冲时间，0.2秒内渐变到animName
        //anim.Play(animName); //不管当前正在播放的动画，终止然后播放animName       
    }

    public void PlayerOnHurt(float damage)
    {
        player.TakeDamage(damage);
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.5f, player.transform.position.z);
        //Debug.Log("受到伤害");
    }
}
