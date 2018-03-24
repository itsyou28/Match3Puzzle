#define _DebugFileManger

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;


/// <summary>
/// ** FileManager는 모두 persistentDataPath를 기준으로 작동한다. **
/// - 플랫폼별로 쓰기권한이 제공되는 경로를 기준으로 한다.
/// - 에디터에서는 Application.DataPath를 기준으로 작동한다. (GetFileStorePath() 참고)
/// 
/// - 유니티가 관리하지 않는 파일의 경우 "Resources", "StreamingAssets" 하위의 있는 파일만 빌드에 포함된다. 
/// - Resources/ 하위의 파일은 빌드 후에는 Resources.Load로만 접근할 수 있고, 쓰기권한이 없다. 
/// - StreamingAssets/ 하위의 파일은 빌드 후에는 www 등으로 접근할 수 있고, 쓰기권한이 없다.
/// </summary>
public class FileManager
{
    private static FileManager instance = null;
    public static FileManager Inst
    {
        get
        {
            if (instance == null)
                instance = new FileManager();

            return instance;
        }
    }

    private FileManager() { }

    ///<summary>
    ///ResourceLoad 함수는 파일 경로에 슬래쉬를 사용한다. 
    ///지원하지 않는 파일 타입일 경우 실패한다.
    ///직렬화 이진 파일을 저장하고 불러올 경우 ".bytes" 확장자를 사용해야 한다. 
    ///ex)FileSave("Resources", "file.bytes", saveobj);
    ///   ResourceLoad("file") as object;
    ///</summary>    
    public object ResourceLoad(string fileName)
    {
        object result = null;

        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        if (textAsset != null)
        {
            try
            {
                MemoryStream stream = new MemoryStream(textAsset.bytes);
                BinaryFormatter bf = new BinaryFormatter();
                result = bf.Deserialize(stream);

                stream.Close();

#if DebugFileManger
            Debug.Log("Resource Load Success " + fileName);
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("Resource Deserialize Failed " + fileName);
                Debug.LogException(e);
            }
        }
        else
        {
            Debug.LogError("Resource Load Failed " + fileName);
        }

        return result;
    }

    public string GetFileStorePath(string fileName = null)
    {
        string path = null;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        path = Application.dataPath;
#elif UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath;
#elif UNITY_IOS && !UNITY_EDITOR
        path = Application.persistentDataPath + "/Documents";
#endif
        if (!string.IsNullOrEmpty(fileName))
            path = Path.Combine(path, fileName);

        return path;
    }

    public string GetStreamingAssetPath(string fileName = null)
    {
        string path = null;

#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        path = "file:///" + Application.streamingAssetsPath;
#elif UNITY_ANDROID && !UNITY_EDITOR
        path =  "jar:file://" + Application.dataPath + "!/assets";
#elif UNITY_IOS && !UNITY_EDITOR
        path = "file:///" + Application.streamingAssetsPath";
#endif
        if (!string.IsNullOrEmpty(fileName))
            path = Path.Combine(path, fileName);

        return path;
    }

    public bool CheckFileExists(string filePath, string fileName)
    {
        if (filePath == null)
            filePath = GetFileStorePath(fileName);
        else
            filePath = GetFileStorePath(Path.Combine(filePath, fileName));

        if (File.Exists(filePath))
            return true;
        else
        {
            Debug.LogWarning("No file : " + filePath);
            return false;
        }
    }

    /// <summary>
    /// GetFileStorePath() 하위의 상대경로를 체크한다. 
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool CheckFileExists(string filePath)
    {
        filePath = GetFileStorePath(filePath);

        if (File.Exists(filePath))
            return true;
        else
        {
            Debug.LogWarning("No file : " + filePath);
            return false;
        }
    }

    public string[] GetFileList(string filePath)
    {
        string targetPath = GetFileStorePath(filePath);

        if (Directory.Exists(targetPath))
            return Directory.GetFiles(targetPath);

        Debug.LogWarning(targetPath + " is not exists.");

        return null;
    }

    public string[] GetFileListOrderBy(string filePath)
    {
        string targetPath = GetFileStorePath(filePath);

        if (!Directory.Exists(targetPath))
            return null;

        DirectoryInfo info = new DirectoryInfo(targetPath);
        FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();

        string[] result = new string[files.Length];

        for (int idx = 0; idx < files.Length; idx++)
        {
            result[idx] = files[idx].Name;
        }

        return result;
    }

    public bool DeleteFile(string filePath)
    {
        if (CheckFileExists(filePath))
        {
            File.Delete(GetFileStorePath(filePath));

            return true;
        }

        return false;
    }

    public object FileLoad(string filePath, string fileName)
    {
        if (filePath == null)
            filePath = GetFileStorePath(fileName);
        else
            filePath = GetFileStorePath(Path.Combine(filePath, fileName));

        object result = null;

        FileStream fs = null;

        try
        {
            fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();//binaryFormatter 생성
            result = bf.Deserialize(fs);

#if DebugFileManger
            if (result != null)
                Debug.Log("File Load Success " + fileName);
            else
            {
                Debug.Log("File Load Failed " + fileName);
                Exception e = new Exception("File Load Failed");
                throw e;
            }
#endif
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (fs != null)
                fs.Close();
        }

        return result;
    }

    ///<summary>
    ///FileStream 함수는 파일 경로에 역슬래쉬를 사용한다. 
    ///</summary>    
    public void FileSave(string filePath, string fileName, object saveObject)
    {
        //Make FileDestination
        if (filePath == null)
            filePath = GetFileStorePath(fileName);
        else
        {
            DirectoryInfo di = new DirectoryInfo(GetFileStorePath(filePath));

            if (di.Exists == false)
                di.Create();

            filePath = GetFileStorePath(Path.Combine(filePath, fileName));
        }

        FileStream fs = null;

        try
        {
            //파일스트림 생성, 파일이 있으면 오픈, 없으면 생성
            fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();//binaryFormatter 생성
            bf.Serialize(fs, saveObject);//serialize(객체, 파일스트림)

#if DebugFileManger
            Debug.Log("File Save to " + filePath);
#endif
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (fs != null)
                fs.Close();//스트림 닫기        
        }
    }

    public void FileSave(string filePath, string fileName, string[] stringLines)
    {
        if (filePath == null)
            filePath = GetFileStorePath(fileName);
        else
        {
            DirectoryInfo di = new DirectoryInfo(GetFileStorePath(filePath));

            if (di.Exists == false)
                di.Create();

            filePath = GetFileStorePath(Path.Combine(filePath, fileName));
        }

        try
        {
            System.IO.File.WriteAllLines(filePath, stringLines);

#if DebugFileManger
            Debug.Log("string lines file save to " + filePath);
#endif
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}