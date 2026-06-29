using UnityEngine;
using System;
using System.Collections;

public class ScreenWipeManager : Singleton<ScreenWipeManager>
{
    [SerializeField] private Animator animator;
    public void PlayWipeOn(Action callback = null)
    {
        animator.Play("wipeOn");
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.34f);
            callback?.Invoke();
        }
    }

    public void PlayWipeOff(Action callback = null)
    {
        animator.Play("wipeOff");
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.34f);
            callback?.Invoke();
        }
    }
}
