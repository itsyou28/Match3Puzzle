using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReactionByState))]
[RequireComponent(typeof(Animator))]
public class Reaction_UI_trm_Anim : Reaction_Expand
{
    Animator uAnim;
    
    public override void Initialize()
    {
        base.Initialize();

        unShowStateHash = Animator.StringToHash("UI_4_UnShow_by_ScaleZero");

        uAnim = GetComponent<Animator>();
    }
    
    public override void Show()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        UDL.Log(gameObject.name + " Show after activeSelf : " + gameObject.activeSelf, nLogOption, bIsDebug, nLogLevel);
    }

    public override void Hide()
    {
        BroadcastMessage("OnBeforeDisable", SendMessageOptions.DontRequireReceiver);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);

        UDL.Log(gameObject.name + " Hide after activeSelf : " + gameObject.activeSelf, nLogOption, bIsDebug, nLogLevel);
    }

    public override void Excute(int _nExcuteId)
    {
        if (uAnim != null && uAnim.isActiveAndEnabled)
        {
            UDL.LogWarning(gameObject.name + " Excute ID : " + _nExcuteId, nLogOption, bIsDebug, nLogWarningLevel);
            aniStateBuffer = -1;
            uAnim.SetInteger("idx", _nExcuteId);
            uAnim.SetTrigger("go");

            LogCurAnimInfo("Excute");
        }

    }

    public override void Change()
    {
        UDL.Log(gameObject.name + " Change : ");
    }

    public void OnBeforeDisable()
    {
        UDL.Log(gameObject.name + " OnBeforeDisable ", nLogOption, bIsDebug, nLogLevel);

        if (uAnim != null)
        {            
            aniStateBuffer = uAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
            LogCurAnimInfo("OnBeforeDisable");
        }
    }

    int aniStateBuffer = -1;
    static int unShowStateHash = -1;

    void OnEnable()
    {
        UDL.Log(gameObject.name + " OnEnable ", nLogOption, bIsDebug, nLogLevel);

        StartCoroutine(RecoverAnimatorState());
    }

    IEnumerator RecoverAnimatorState()
    {
        yield return true;

        if (uAnim != null)
        {
            if (aniStateBuffer != -1 && aniStateBuffer != unShowStateHash)
            {
                UDL.LogWarning(gameObject.name + " aniStateBuffer : " + aniStateBuffer, nLogOption, bIsDebug, nLogWarningLevel);
                uAnim.Play(aniStateBuffer);
                LogCurAnimInfo("RecoverAnimatorState"); 
            }
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void LogCurAnimInfo(string prefix)
    {
        AnimatorStateInfo state = uAnim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clip = uAnim.GetCurrentAnimatorClipInfo(0);

        string clipInfo = "";
        foreach (var item in clip)
        {
            clipInfo += item.clip.name + " / " + item.weight;
        }

        UDL.LogWarning(gameObject.name + " // "+ prefix+" // " + state.shortNameHash + "\n" + clipInfo, nLogOption, bIsDebug);
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        UDL.Log(gameObject.name + " OnDisable ", nLogOption, bIsDebug, nLogLevel);
    }
#endif
}
