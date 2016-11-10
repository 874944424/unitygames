using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
    private OwnerPlayer player;
    private bool havePlayDieAnimation = false;  //是否已经播放过死亡动画
    private CharacterController characterController;
    private Animation anim;
    private NetworkView networkview;
    void Awake()
    {
        player = this.GetComponent<OwnerPlayer>();
        anim = this.GetComponent<Animation>();
        characterController = this.GetComponent<CharacterController>();
        networkview = this.GetComponent<NetworkView>();
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
            }
        }
        else
        {
            if (characterController.isGrounded == false)
            {
                //播放下的动画
                PlayState(AnimsName.soldierFalling_animName);
                
            }
            else
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                if (Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f)
                {
                    PlayState(AnimsName.soldierRun_animName);                 
                }
                else
                {
                    PlayState(AnimsName.soldierIdle_animName);
                }
            }
        }
    }

    void PlayState(string animName)
    {
        anim.CrossFade(animName, 0.2f);//会有0.2秒的缓冲时间，0.2秒内渐变到animName
        //anim.Play(animName); //不管当前正在播放的动画，终止然后播放animName       
    }
}
