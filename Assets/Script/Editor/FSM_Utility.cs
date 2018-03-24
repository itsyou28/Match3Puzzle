using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FiniteStateMachine
{

    public class StatePopupList
    {
        public string[] arr_sName;
        public int[] arr_nID;
    }


    public static class FSM_Utility
    {

        static System.Array arr_sStateID = null;
        static Dictionary<FSM_ID, StatePopupList> dic_ePopupArray = new Dictionary<FSM_ID, StatePopupList>();

        public static void Initialize()
        {
            if (arr_sStateID == null || arr_sStateID.Length != System.Enum.GetValues(typeof(STATE_ID)).Length)
            {
                arr_sStateID = System.Enum.GetValues(typeof(STATE_ID));
                MakePopupArray();
            }
        }

        public static StatePopupList GetPopupList(FSM_ID target)
        {
            return dic_ePopupArray[target];
        }

        static void MakePopupArray()
        {
            dic_ePopupArray.Clear();

            System.Array arr_FSMID = System.Enum.GetValues(typeof(FSM_ID));

            for (int idx = 0; idx < arr_FSMID.Length; idx++)
            {
                FSM_ID targetID = (FSM_ID)arr_FSMID.GetValue(idx);
                dic_ePopupArray.Add(targetID, MakePopupArray(targetID));
            }
        }

        private static StatePopupList MakePopupArray(FSM_ID fsmid)
        {
            string prefix = fsmid.ToString();
            string ts;
            string[] split;

            List<string> tArr_Name = new List<string>();
            List<int> tArr_ID = new List<int>();

            for (int i = 0; i < arr_sStateID.Length; i++)
            {
                ts = arr_sStateID.GetValue(i).ToString();
                split = ts.Split('_');

                if (split[0] == prefix)
                {
                    tArr_Name.Add(ts);
                    tArr_ID.Add((int)arr_sStateID.GetValue(i));
                }
            }

            tArr_Name.Add(STATE_ID.HistoryBack.ToString());
            tArr_ID.Add((int)STATE_ID.HistoryBack);

            StatePopupList result = new StatePopupList();

            result.arr_sName = tArr_Name.ToArray() as string[];
            result.arr_nID = tArr_ID.ToArray();

            return result;
        }
    }

}