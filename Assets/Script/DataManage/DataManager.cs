using System;
using System.Collections.Generic;

public interface iDataFile
{
    void Load();
    void Save();
}

class DataManager
{
    private static DataManager instance = null;
    public static DataManager Inst
    {
        get
        {
            if (instance == null)
                instance = new DataManager();

            return instance;
        }
    }
    
    public StageData stageData;

    iDataFile[] arriData;

    private DataManager()
    {
        stageData = new StageData();

        arriData = new iDataFile[1];
        arriData[0] = stageData;

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

