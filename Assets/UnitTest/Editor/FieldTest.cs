using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class FieldTest
{
}

public class MatchTest : IPrebuildSetup
{
    public void Setup()
    {
        Debug.LogWarning("Setup");
    }
    
    [Test]
    public void MatchTest1()
    {
        int[] arrCorrect = { 1, 1, 1 };
        TestMatch(arrCorrect, "MatchTestCase1");
    }

    [Test]
    public void MatchTest2()
    {
        int[] arrCorrect = { 1, 1, 1, 1, 1, 1 };
        TestMatch(arrCorrect, "MatchTestCase2");
    }

    [Test]
    public void MatchTest3()
    {
        int[] arrCorrect = { 3,3,3,2,2,2 };
        TestMatch(arrCorrect, "MatchTestCase3");
    }

    private static void TestMatch(int[] arrCorrect, string fieldName)
    {
        BlockFieldManager stage = new BlockFieldManager(fieldName);
        
        stage.ExcuteMatch();

        Assert.AreEqual(arrCorrect.Length, stage.matchedField.Count);

        for (int i = 0; i < stage.matchedField.Count; i++)
        {
            Assert.AreEqual(arrCorrect[i], stage.matchedField[i].block.BlockType);
        }
    }
}
