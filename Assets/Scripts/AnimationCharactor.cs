using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharactor : MonoBehaviour
{
  [SerializeField] private SkeletonAnimation skeletonAnimation;
  private float timeDelay;

  public void onDamage(float currenthp, bool isNormalAttack)
  {
    if (currenthp > 0)
    {
      if (isNormalAttack)
      {
        skeletonAnimation.AnimationName = "Moody UnFriendly";
        timeDelay = 1.7f;
        StartCoroutine("ResetAnimation");
      }
      else
      {
        skeletonAnimation.AnimationName = "Moody Friendly";
        timeDelay = 1.7f;
        StartCoroutine("ResetAnimation");
      }
    }
  }

  public void onMiss()
  {
    skeletonAnimation.AnimationName = "Drag Friendly";
    timeDelay = 1f;
    StartCoroutine("ResetAnimation");
  }

  private IEnumerator ResetAnimation()
  {
    yield return new WaitForSeconds(timeDelay);
    skeletonAnimation.AnimationName = "Idle Friendly 1";
  }

  public void WinAnimation()
  {
    skeletonAnimation.AnimationName = "Moody UnFriendly";
  }

  public void LossAnimation()
  {
    skeletonAnimation.AnimationName = "Cheer Friendly";
  }

  public void ResetAnimationToIdle()
  {
    skeletonAnimation.AnimationName = "Idle Friendly 1";
  }
}