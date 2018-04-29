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

        //Assert.AreEqual(arrCorrect.Length, stage.matchedSet.Count);

        //for (int i = 0; i < stage.matchedSet.Count; i++)
        //{
        //    Assert.AreEqual(arrCorrect[i], stage.matchedSet[i].block.BlockType);
        //}
    }

    [Test]
    public void BlockEqualTest()
    {
        Block b1 = new Block();
        b1.ID = 1;
        b1.SetBlockType(1);
        Block b2 = new Block();
        b2.ID = 2;
        b2.SetBlockType(1);
        Block b3 = new Block();
        b3.ID = 1;
        b3.SetBlockType(1);

        iBlock ib1 = b1;
        iBlock ib2 = b2;

        Assert.AreNotSame(b1, b2);

        Assert.AreNotSame(ib1, ib2);
        Assert.AreSame(b1, ib1);

        Assert.IsTrue(b1 == b2);
        Assert.IsTrue(ib1 == ib2);
    }
}
