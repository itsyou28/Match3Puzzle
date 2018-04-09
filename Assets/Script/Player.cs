using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    BlockFieldGO selectField, targetField;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                selectField = hit.collider.GetComponent<BlockFieldGO>();
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                targetField = hit.collider.GetComponent<BlockFieldGO>();

                StageManager.i.SwapBlock(selectField.Field, targetField.Field);
            }
        }
    }
}
