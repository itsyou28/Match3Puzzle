using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Message : MonoBehaviour
{
    [SerializeField]
    GameObject textOrigin;

    ObjectPool<GameObject> textPool;

    GameObject CreateText()
    {
        GameObject obj = Instantiate(textOrigin);

        obj.transform.SetParent(transform, false);
        obj.SetActive(false);

        return obj;
    }

    private void Awake()
    {
        textPool = new ObjectPool<GameObject>(3, 1, CreateText);

        EMC_MAIN.Inst.AddEventCallBackFunction(EMC_CODE.DISP_MSG, OnMsgShow);
    }

    void OnMsgShow(params object[] args)
    {
        //파라메터 체크

        if(args == null || args.Length == 0)
        {
            UDL.LogError("No Msg");
            return;
        }

        string msg = (string)args[0];

        StartCoroutine(ScaleShow(msg));
    }

    IEnumerator ScaleShow(string msg)
    {
        GameObject obj = textPool.Pop();
        Text text = obj.GetComponent<Text>();
        text.text = msg;

        float elapseTime = 0;
        float aniTime = 4;
        float normalizer = 1 / aniTime;

        Vector3 vScale = Vector3.zero;
        Vector3 vPos = Vector3.zero;

        float normalizeTime = 0;
        Color textColor = text.color;
        
        obj.transform.localScale = vScale;
        obj.transform.localPosition = vPos;

        yield return null;

        obj.SetActive(true);

        while (elapseTime < aniTime)
        {            
            elapseTime += Time.deltaTime;
            normalizeTime = elapseTime * normalizer;

            textColor.a = 1 - Ease.InCubic(normalizeTime);
            vPos.y = Ease.InOutCubic(normalizeTime) * 300;
            vScale.x = vScale.y = Ease.OutElastic(normalizeTime*3.5f);

            text.color = textColor;
            obj.transform.localPosition = vPos;
            obj.transform.localScale = vScale;

            yield return null;
        }

        obj.transform.localScale = Vector3.one;

        yield return null;

        obj.SetActive(false);

        textColor.a = 1;
        text.color = textColor;

        textPool.Push(obj);
    }
}