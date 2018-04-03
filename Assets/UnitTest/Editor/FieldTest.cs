using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class FieldTest
{
    int rowMax = 5;
    int colMax = 3;

    [Test]
    public void CreateField()
    {
        BlockFieldMaker.Inst.CreateField(rowMax, colMax, "TestField");
        BlockFieldMaker.Inst.SaveField();
    }

    [Test]
    public void LoadField()
    {
        BlockField[,] loadData = BlockFieldMaker.Inst.LoadField("TestField");

        Assert.AreEqual(rowMax+2, loadData.GetLength(0));
        Assert.AreEqual(colMax+2, loadData.GetLength(1));
    }

    [Test]
    public void MatchTest1()
    {
        BlockFieldManager stage = new BlockFieldManager("TestField");

        stage.ExcuteMatch();
    }

    [Test]
    public void MakeTestCase()
    {
        int[,] testCase1 = {   
            { 0, 2, 3 },
            { 1, 1, 1 },
            { 0, 2, 3 }};

        BlockFieldMaker.Inst.MakeTestField(testCase1, "MatchTestCase1");
        LoadTest(testCase1, "MatchTestCase1");

        int[,] testCase2 = {
            { 2, 1, 2 },
            { 1, 1, 1 },
            { 2, 1, 2 }};

        BlockFieldMaker.Inst.MakeTestField(testCase2, "MatchTestCase2");
        LoadTest(testCase2, "MatchTestCase2");


        int[,] testCase3 = {
            { 2, 1, 2, 1, 2 },
            { 1, 1, 2, 1, 2 },
            { 2, 3, 3, 3, 2}};

        BlockFieldMaker.Inst.MakeTestField(testCase3, "MatchTestCase3");
        LoadTest(testCase3, "MatchTestCase3");
    }

    void LoadTest(int[,] testCase, string fieldName)
    {
        AssetDatabase.Refresh();
        BlockField[,] fields = BlockFieldMaker.Inst.LoadField(fieldName);

        for (int row = 0; row < testCase.GetLength(0); row++)
        {
            for (int col = 0; col < testCase.GetLength(1); col++)
            {
                Assert.AreEqual(testCase[row,col], fields[row+1, col+1].block.BlockType);
            }
        }
    }


    [Test]
    public void MoveBlock()
    {
        Assert.Fail();
    }

    [Test]
    public void ableFieldTest()
    {
        Assert.Fail();
    }
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
