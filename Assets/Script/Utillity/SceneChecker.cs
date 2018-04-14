using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChecker
{
    public static bool IS_USING_FSM_SCENE
    {
        get
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                Debug.LogWarning("using fsm scene");
                return true;
            }
            return false;
        }
    }

    public static bool IS_NOT_USING_FSM_SCENE
    {
        get
        {
            if (SceneManager.GetActiveScene().name != "Main")
            {
                Debug.LogWarning("Not using fsm scene");
                return true;
            }

            return false;
        }
    }
}