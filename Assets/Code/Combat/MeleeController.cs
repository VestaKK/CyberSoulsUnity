using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] private MeleeHitboxController meleeHitbox;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Animator playerAnimator;

    public bool isResting = false;
    public bool isAttacking = false;
    public float attackCoolDown = 0;
    public int clickCount = 0;

    private MeleeAttackInfo attackInfo = new MeleeAttackInfo();

    struct MeleeAttackInfo 
    {
        public MeleeAttackInfo
            (int damage, float swingSpeed, float lingerTime, 
            float range, float reach, Vector3 offset) 
        {
            this.damage = damage;
            this.swingSpeed = swingSpeed;
            this.lingerTime = lingerTime;
            this.range = range;
            this.reach = reach;
        }

        public int damage;
        public float swingSpeed;
        public float lingerTime;
        public float range;
        public float reach;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Melee Hit 1"))
        {
            playerAnimator.SetBool("Hit1", false);
            attackCoolDown = 0.1f;
            isAttacking = false;
            isResting = true;
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Melee Hit 2"))
        {
            playerAnimator.SetBool("Hit2", false);
            attackCoolDown = 0.3f;
            isAttacking = false;
            isResting = true;
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Melee Hit 3"))
        {
            playerAnimator.SetBool("Hit3", false);
            attackCoolDown = 0.5f;
            isAttacking = false;
            isResting = true;
        }

        if (attackCoolDown <= 0)
        {
            if (isResting) 
            {
                clickCount = 0;
                isResting = false;
            }
            attackCoolDown = 0;
        }
        else 
        {
            attackCoolDown -= Time.deltaTime;
        }

        if (isAttacking) 
        {
            CheckAnimationTransitions();
        }
    }

    public void OnClick() {

        if (isResting) return;

        clickCount++;

        clickCount = Mathf.Clamp(clickCount, 0, 3);

        CheckAnimationTransitions();
    }

    void CheckAnimationTransitions() 
    {
        if (clickCount == 1)
        {
            isAttacking = true;
            playerAnimator.SetBool("Hit1", true);
            attackInfo = new MeleeAttackInfo(10, 10, 0.1f, 1, 1, offset);
        }

        if (clickCount >= 2 &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Melee Hit 1"))
        {
            attackInfo = new MeleeAttackInfo(10, 10, 0.2f, 1, 1, offset);
            playerAnimator.SetBool("Hit1", false);
            playerAnimator.SetBool("Hit2", true);
        }

        if (clickCount >= 3 &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f &&
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Melee Hit 2"))
        {
            attackInfo = new MeleeAttackInfo(10, 10, 0.3f, 1, 1, offset);
            playerAnimator.SetBool("Hit2", false);
            playerAnimator.SetBool("Hit3", true);
        }
    }

    public void AttackAnimationEvent() 
    {
        SpawnHitbox(attackInfo);
    }

    private void SpawnHitbox(MeleeAttackInfo attackInfo) {

        int damage = attackInfo.damage;
        float reach = attackInfo.reach;
        float range = attackInfo.range;
        float lingerTime = attackInfo.lingerTime;

        MeleeHitboxController newMeleeSwing = Instantiate(
                meleeHitbox,
                transform.position + transform.forward * reach + offset,
                transform.rotation,
                transform) as MeleeHitboxController;
        newMeleeSwing.transform.localScale *= range;
        newMeleeSwing.damage = damage;
        newMeleeSwing.meleeLingerTime = lingerTime;
    }


}
