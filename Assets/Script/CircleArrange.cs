using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircleArrange : MonoBehaviour
{
    [SerializeField]
    float r = 200;

    private void Start()
    {
        float x, y;
        float angle = 6.28319f / transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            x = Mathf.Sin(angle * i) * r;
            y = Mathf.Cos(angle * i) * r;

            transform.GetChild(i).transform.localPosition = new Vector3(x, y);
        }

        Transform t;

        t = transform.GetChild(0);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetPlayable(); });
        t.GetComponentInChildren<Text>().text = "Playable";

        t = transform.GetChild(1);
        t.GetComponentInChildren<Button>().onClick.AddListener(
            () => { EditManager.i.SetNonplayable(); });
        t.GetComponentInChildren<Text>().text = "Nonplayable";

    }
}
