using System;
using System.Collections.Generic;

public interface iDataFile
{
    void Load();
    void Save();
}

class DataFileManager
{
    private static DataFileManager instance = null;
    public static DataFileManager Inst
    {
        get
        {
            if (instance == null)
                instance = new DataFileManager();

            return instance;
        }
    }
    
    public StageDataFile stageDataFile;

    iDataFile[] arriData;

    private DataFileManager()
    {
        stageDataFile = new StageDataFile();

        arriData = new iDataFile[1];
        arriData[0] = stageDataFile;

        Load();
    }

    public void Load()
    {
        for (int i = 0; i < arriData.Length; i++)
        {
            arriData[i].Load();
        }
    }

    public void Save()
    {
        for (int i = 0; i < arriData.Length; i++)
        {
            arriData[i].Save();
        }
    }
}

