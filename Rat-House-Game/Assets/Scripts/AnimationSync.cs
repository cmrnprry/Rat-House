using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSync : MonoBehaviour
{
    //The animator controller attached to this GameObject
    public Animator animator;

    //Records the animation state or animation that the Animator is currently in
    public AnimatorStateInfo animatorStateInfo;

    //Used to address the current state within the Animator using the Play() function
    public int currentState;

    void Start()
    {
        //Get the info about the current animator state
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //Convert the current state name to an integer hash for identification
        currentState = animatorStateInfo.fullPathHash;
    }

    void Update()
    {
        //Start playing the current animation from wherever the current conductor loop is
        animator.Play(currentState, -1, (GameManager.instance.loopPositionInAnalog));
        
        //Set the speed to 0 so it will only change frames when you next update it
        animator.speed = 0;
    }
}
