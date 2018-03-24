using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public  class BK_Function
{
    static float rangeInterval, percent = 0;

    /// <summary>
    /// 범위와 값을 입력하면 해당하는 비율(%)을 반환한다. 
    /// 범위는 항상 양수여야 한다. 
    /// </summary>
    public static float ConvertRangePercent(float start, float end, float curValue)
    {
        if (start >= end)
        {
            Exception e = new Exception("start value(" + start.ToString() +
                ") must small than end value(" + end.ToString() + ")");
            throw (e);
        }

        if (start < 0)
        {
            Exception e = new Exception("start value can't be less than zero");
            throw (e);
        }
        
        curValue -= start;
        end -= start;
        start -= start;

        rangeInterval = end - start;        

        if (curValue == 0)
            return 0;
        else
            return curValue / rangeInterval;
    }

    public static float ConvertRangePercentClamp(float start, float end, float curValue)
    {
        float result = ConvertRangePercent(start, end, curValue);

        if (result < 0)
            return 0;
        else if (result > 1)
            return 1;

        return result;
    }

    /// <summary>
    /// 범위와 비율을 입력하면 해당하는 값을 반환한다. 
    /// 범위는 항상 양수여야 한다. 
    /// </summary>
    public static float ConvertRangeValue(float start, float end, float curPercent)
    {
        if (start >= end)
        {
            Exception e = new Exception("start value(" + start.ToString() +
                ") must small than end value(" + end.ToString() + ")");
            throw (e);
        }

        if (start < 0)
        {
            Exception e = new Exception("start value can't be less than zero");
            throw (e);
        }
        
        rangeInterval = end - start;

        if (curPercent == 0)
            return start;
        else
            return (curPercent * rangeInterval) + start;
    }

    /// <summary>
    /// 주어진 범위의 값을 목표 범위의 값으로 변환해서 반환한다. 
    /// 범위는 항상 양수여야 한다. 
    /// </summary>
    public static float ConvertRange(float curStart, float curEnd, float targetStart, float targetEnd, float curValue)
    {
        percent = ConvertRangePercent(curStart, curEnd, curValue);

        return ConvertRangeValue(targetStart, targetEnd, percent);
    }

    public static List<int> RandomPickWithAdjustPool(List<int> pool, int pickLength)
    {
        List<int> result = new List<int>();

        int randomPick = 0;

        for (int idx = 0; idx < pickLength; idx++)
        {
            randomPick = UnityEngine.Random.Range(0, pool.Count);

            result.Add(pool[randomPick]);
            pool[randomPick] = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
        }

        return result;
    }

    public static List<int> RandomPick(List<int> pool, int pickLength)
    {
        List<int> copyPool = DeepCopy(pool) as List<int>;
        List<int> result = new List<int>();

        int randomPick = 0;

        for (int idx = 0; idx < pickLength; idx++)
        {
            randomPick = UnityEngine.Random.Range(0, copyPool.Count);

            result.Add(copyPool[randomPick]);
            copyPool[randomPick] = copyPool[copyPool.Count - 1];
            copyPool.RemoveAt(copyPool.Count - 1);
        }

        return result;
    }

    /// <summary>
    /// 총 시간 대비 경과시간 비율로 주어진 범위내 값을 반환한다. 
    /// </summary>
    public static float GradualChange(float start, float end, float time, float currentTime)
    {
        if (start == end)
        {
            Exception e = new Exception("start value and end value must not equal");
            throw (e);
        }
        
        float interval = start - end;
        float mark = interval / Mathf.Abs(interval);

        interval = interval * mark;

        if (currentTime < time)
            return start - (ConvertRange(0, time, 0, interval, currentTime) * mark);
        else
            return end;
    }

    public static Vector3 Gradualchange(Vector3 start, Vector3 end, float time, float currentTime)
    {
        if (start == end)
        {
            Exception e = new Exception("start value and end value must not equal");
            throw (e);
        }

        Vector3 vDirection = start - end;

        float current = 0;

        current = ConvertRangePercent(0, time, currentTime);

        return start - (vDirection * current);
    }

    /// <summary>
    /// 객체를 깊은 복사합니다.
    /// </summary>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    public static object DeepCopy(object targetObject)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(ms, targetObject);
        ms.Seek(0, 0);        
        return formatter.Deserialize(ms);
    }

    public static T[] CopyArray<T>(T[] targetArr)
    {
        T[] buffer_arr = new T[targetArr.Length];

        for (int i = 0; i < targetArr.Length; i++)
            buffer_arr[i] = targetArr[i];

        return buffer_arr;
    }

    public static void ResizeArr<T>(int size, ref T[] targetArr)
    {
        if (targetArr == null)
        {
            targetArr = new T[size];
            return;
        }

        T[] buffer_arr = CopyArray(targetArr);
        targetArr = new T[size];

        for (int i = 0; i < size; i++)
        {
            if (buffer_arr.Length > i)
                targetArr[i] = buffer_arr[i];
        }
    }

    public static void ResizeArrWithDeepCopy<T>(int size, ref T[] targetArr)
    {
        if (targetArr == null)
        {
            targetArr = new T[size];
            return;
        }

        T[] buffer_arr = DeepCopy(targetArr) as T[];
        targetArr = new T[size];


        for (int i = 0; i < size; i++)
        {
            if (buffer_arr.Length > i)
                targetArr[i] = buffer_arr[i];
        }
    }

    public static void ResizeArrWithDeepCopy<T>(int size, ref T[][] targetArr)
    {
        if (targetArr == null)
        {
            targetArr = new T[size][];
            return;
        }

        T[][] buffer_arr = DeepCopy(targetArr) as T[][];
        targetArr = new T[size][];


        for (int i = 0; i < size; i++)
        {
            if (buffer_arr.Length > i)
                targetArr[i] = buffer_arr[i];
        }
    }

    public static void RemoveAtArr<T>(int idx, ref T[] targetArr)
    {
        if (targetArr == null || idx >= targetArr.Length)
            throw new Exception("can't remove. index is over value.");

        if (targetArr.Length == 1)
        {
            targetArr = new T[0];
            return;
        }

        T[] buffer_arr = CopyArray(targetArr) as T[];

        targetArr = new T[targetArr.Length - 1];

        for (int i = 0; i < targetArr.Length; i++)
        {
            if (i < idx)
                targetArr[i] = buffer_arr[i];
            else
                targetArr[i] = buffer_arr[i + 1];
        }
    }

    public static void RemoveAtArrWitDeepCopy<T>(int idx, ref T[] targetArr)
    {
        if (targetArr == null || idx >= targetArr.Length)
            throw new Exception("can't remove. index is over value.");

        if (targetArr.Length == 1)
        {
            targetArr = null;
            return;
        }

        T[] buffer_arr = DeepCopy(targetArr) as T[];

        targetArr = new T[targetArr.Length - 1];

        for (int i = 0; i < targetArr.Length; i++)
        {
            if (i < idx)
                targetArr[i] = buffer_arr[i];
            else
                targetArr[i] = buffer_arr[i + 1];
        }
    }

    public static void RemoveAtArr<T>(int idx, ref T[][] targetArr)
    {
        if (targetArr == null || idx >= targetArr.Length)
            throw new Exception("can't remove. index is over value.");

        if (targetArr.Length == 1)
        {
            targetArr = null;
            return;
        }

        T[][] buffer_arr = DeepCopy(targetArr) as T[][];

        targetArr = new T[targetArr.Length - 1][];

        for (int i = 0; i < targetArr.Length; i++)
        {
            if (i < idx)
                targetArr[i] = buffer_arr[i];
            else
                targetArr[i] = buffer_arr[i + 1];
        }
    }

    public static byte[] BinaryFormatter(object target)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(ms, target);
        return ms.ToArray();
    }

    public static object BinaryDeserialize(byte[] data)
    {
        MemoryStream ms = new MemoryStream(data);
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter.Deserialize(ms);
    }

    public static string BinaryStringFormatter(object target)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(ms, target);

        return BitConverter.ToString(ms.ToArray());
    }

    public static object BinaryStringDeserialize(string data)
    {
        String[] tempAry = data.Split('-');
        byte[] decBytes2 = new byte[tempAry.Length];
        for (int i = 0; i < tempAry.Length; i++)
            decBytes2[i] = Convert.ToByte(tempAry[i], 16);

        MemoryStream ms = new MemoryStream(decBytes2);
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter.Deserialize(ms);
    }

    public static void CreateBillboardMesh(string targetCameraName, GameObject targetObj, 
        float fWidth_at_device, float fHeight_at_device, Material mat = null)
    {
        Camera targetCamera = GameObject.Find(targetCameraName).GetComponent<Camera>();

        MeshFilter meshFilter = targetObj.AddComponent<MeshFilter>();   //MeshFilter 컴포넌트 추가

        Mesh mesh = new Mesh();

        targetObj.AddComponent<MeshRenderer>();  //MeshRenderer컴포넌트 추가


        Vector3 vLeftTop, vRightTop, vLeftBottom, vRightBottom;

        float halfwidth = fWidth_at_device * 0.5f;
        float halfheight = fHeight_at_device * 0.5f;

        Vector3 thisScreenPos = targetCamera.WorldToScreenPoint(targetObj.transform.position);

        vLeftTop = targetCamera.ScreenToWorldPoint(new Vector3(
            thisScreenPos.x - halfwidth,
            thisScreenPos.y + halfheight,
            thisScreenPos.z));

        vRightTop = targetCamera.ScreenToWorldPoint(new Vector3(
            thisScreenPos.x + halfwidth,
            thisScreenPos.y + halfheight,
            thisScreenPos.z));

        vLeftBottom = targetCamera.ScreenToWorldPoint(new Vector3(
            thisScreenPos.x - halfwidth,
            thisScreenPos.y - halfheight,
            thisScreenPos.z));

        vRightBottom = targetCamera.ScreenToWorldPoint(new Vector3(
            thisScreenPos.x + halfwidth,
            thisScreenPos.y - halfheight,
            thisScreenPos.z));



        mesh.vertices = new Vector3[]{                   //정점 4개만들어줌
            vLeftTop, vRightTop,
            vLeftBottom, vRightBottom
        };

        mesh.uv = new Vector2[]{                        //정점 별로 UV좌표 찍어줌
            new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f)
        };

        mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };        //삼각형그릴때 순서

        mesh.RecalculateNormals();              //일반좌표계로 다시 계산해주는 함수
        meshFilter.mesh = mesh;                 //메쉬 필터에 Mesh를 넣어줌 

        if (mat != null)
            targetObj.GetComponent<Renderer>().material = mat;
    }


    public static void RefreshShader(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            Material sharedMaterial = renderers[i].sharedMaterial;

            if (sharedMaterial != null)
            {
                if (sharedMaterial.shader != null)
                {
                    string shaderName = sharedMaterial.shader.name;
                    Shader newShader = Shader.Find(shaderName);

                    if (newShader != null)
                        renderers[i].sharedMaterial.shader = newShader;
                }
            }
        }
    }

    /// <summary>
    /// 텍스쳐 보간 리사이즈 알고리즘
    /// http://lhh3520.tistory.com/300
    /// </summary>
    public static Texture2D ResizeTexture(Texture2D source, Vector2 size)
    {
        //*** Get All the source pixels
        Color[] aSourceColor = source.GetPixels(0);
        Vector2 vSourceSize = new Vector2(source.width, source.height);

        //*** Calculate New Size
        float xWidth = size.x;
        float xHeight = size.y;

        //*** Make New
        Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = (int)xWidth * (int)xHeight;
        Color[] aColor = new Color[xLength];

        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

        //*** Loop through destination pixels and process
        Vector2 vCenter = new Vector2();
        for (int ii = 0; ii < xLength; ii++)
        {
            //*** Figure out x&y
            float xX = (float)ii % xWidth;
            float xY = Mathf.Floor((float)ii / xWidth);

            //*** Calculate Center
            vCenter.x = (xX / xWidth) * vSourceSize.x;
            vCenter.y = (xY / xHeight) * vSourceSize.y;

            //*** Average
            //*** Calculate grid around point
            int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
            int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
            int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
            int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

            //*** Loop and accumulate
            //Vector4 oColorTotal = new Vector4();
            Color oColorTemp = new Color();
            float xGridCount = 0;
            for (int iy = xYFrom; iy < xYTo; iy++)
            {
                for (int ix = xXFrom; ix < xXTo; ix++)
                {

                    //*** Get Color
                    oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                    //*** Sum
                    xGridCount++;
                }
            }

            //*** Average Color
            aColor[ii] = oColorTemp / (float)xGridCount;
        }

        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();

        //*** Return
        return oNewTex;
    }


    public static DateTime GetDateTimeFromUTC(int nTime)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(nTime);
    }

}
