using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;


public struct MatchedSet
{
    public BlockField[] fields;
    public int blockType;
    public int specialType;
    public BlockField makeOverField;
}

public class BlockFieldManager
{
    BlockField[,] fields;

    //배열 길이 -2
    public int RowLength { get; private set; }
    public int ColLength { get; private set; }

    int rowArrLastIdx, colArrLastIdx; //배열 길이 -1
    int lastRow, lastCol; //가장자리 필드를 제외한 마지막 배열 인덱스

    public List<BlockField> ableField = new List<BlockField>();

    public List<MatchedSet> rowMatchedSet = new List<MatchedSet>();
    public List<MatchedSet> colMatchedSet = new List<MatchedSet>();
    public List<MatchedSet> rowSpecialSet = new List<MatchedSet>();
    public List<MatchedSet> colSpecialSet = new List<MatchedSet>();
    public List<MatchedSet> crossSpecialSet = new List<MatchedSet>();

    public string FieldName { get; private set; }

    public void CleanUp()
    {
        for (int row = 0; row <= rowArrLastIdx; row++)
        {
            for (int col = 0; col <= colArrLastIdx; col++)
            {
                fields[row, col].CleanUp();
            }
        }
    }

    // 에디터, 테스터 등에서 호출한다. 
    public BlockFieldManager(BlockField[,] fields)
    {
        this.fields = fields;

        Initialize();
    }

    /// <summary>
    /// 파일로 저장되어 있는 필드를 불러온다. 
    /// </summary>
    public BlockFieldManager(string fieldFileName)
    {
        FieldName = fieldFileName;

        fields = DataManager.Inst.stageData.LoadStageFields(fieldFileName);

        Initialize();

        if (fields == null)
            throw new Exception("check field data " + fieldFileName);

        if (fields.Length * fields.Length < 6)
            throw new Exception("small fields size : " + fieldFileName);

        Debug.Log("fields load : " + fieldFileName + " " + RowLength + " " + ColLength + " " + lastRow + " " + lastCol);
    }

    public void SaveFields()
    {
        DataManager.Inst.stageData.SaveStageFields(FieldName, fields);
    }

    void Initialize()
    {
        rowArrLastIdx = fields.GetLength(0) - 1;
        colArrLastIdx = fields.GetLength(1) - 1;
        RowLength = fields.GetLength(0) - 2;
        ColLength = fields.GetLength(1) - 2;
        lastRow = RowLength;
        lastCol = ColLength;

        BlockField.fieldMng = this;
        Block.fieldMng = this;
    }

    public void DeployBlock()
    {
        BlockField field;

        //Playable Field 위치에 랜덤 블럭 생성
        for (int i = 0; i <= rowArrLastIdx; i++)
        {
            for (int j = 0; j <= colArrLastIdx; j++)
            {
                field = fields[i, j];

                field.DeployScreen();

                if (field.IsPlayable)
                {
                    if (field.block == null)
                        field.CreateBlock();
                    else
                    {
                        field.block.CheckField(field);
                        field.block.DeployScreen();
                    }
                }
                else if (field.IsCreateField)
                    field.CreateBlock();
            }
        }

        //시작과 동시에 매칭 블럭이 없도록 조정
        //while (FindMatch())
        //{
        //    for (int i = 0; i < matchedField.Count; i += 2)
        //    {
        //        matchedField[i].block.ResetRand(matchedField[i], 5);
        //    }

        //    if (!FindMatchAble())
        //    {
        //        //임의의 able 패턴을 생성(그냥 셔플->매치가 반복되면 유저가 게임 시작도 안했는데 게임이 진행되는 셈)
        //    }
        //}
    }

    public void EditorInitialize()
    {
        BlockField field;

        //Playable Field 위치에 랜덤 블럭 생성
        for (int i = 0; i <= rowArrLastIdx; i++)
        {
            for (int j = 0; j <= colArrLastIdx; j++)
            {
                field = fields[i, j];

                field.DeployScreen();
                if (field.block != null)
                {
                    field.block.CheckField(field);
                    field.block.DeployScreen();
                }
            }
        }
    }

    public void ActiveFields(bool isEditMode)
    {
        foreach (var field in Fields())
        {
            field.Active(isEditMode);
        }
    }

    public void DeactiveFields()
    {
        foreach (var field in Fields())
        {
            field.Deactive();
        }
    }

    public void Shuffle()
    {
        int row, col;

        //모든 블럭을 한 번씩 랜덤한 위치의 블럭과 교환한다. 
        foreach (var field in Fields())
        {
            row = UnityEngine.Random.Range(1, lastRow);
            col = UnityEngine.Random.Range(1, lastCol);

            SwapBlock(field, fields[row, col]);
        }
    }

    //event Action BlockMove;

    List<BlockField> movedLineList = new List<BlockField>();

    public IEnumerable<BlockField> Fields()
    {
        for (int i = 1; i <= lastRow; i++)
        {
            for (int j = 1; j <= lastCol; j++)
            {
                yield return fields[i, j];
            }
        }
    }

    //해당 라인의 마지막 Field를 반환한다. 
    //BlockField GetLineLast(BlockField field)
    //{
    //    while (!field.IsLast)
    //        field = field.next;

    //    return field;
    //}

    bool swaping1 = false;
    bool swaping2 = false;
    public void SwapBlock(BlockField selected, BlockField target)
    {
        if (selected == null || selected.block == null || target == null || target.block == null)
            return;

        swaping1 = false;
        swaping2 = false;
        selected.block.SwapMove(target, () => { swaping1 = true; SwapResult(); });
        target.block.SwapMove(selected, () => { swaping2 = true; SwapResult(); });

        iBlock buffer = selected.block;

        selected.SetBlock(target.block);
        target.SetBlock(buffer);

        selected.block.SetSwapField(selected);
        target.block.SetSwapField(target);

        swapField[0] = selected;
        swapField[1] = target;
    }

    void SwapResult()
    {
        if (swaping1 && swaping2)

        {
            if (FindMatch())
                ExcuteMatch();
        }
    }

    public bool ValidateField()
    {
        bool result = true;
        foreach (var field in Fields())
        {
            result &= field.ValidateField();
        }

        return result;
    }

    public void SetFieldRelationInfo()
    {
        //모든 필드의 상호의존정보를 일괄 업데이트 한다. 
        foreach (var field in Fields())
        {
            field.IsDeadline = true;
            field.SetPrevArray();
        }

        //생성필드만 검색해서 해당 라인의 deadline 속성을 변경한다. 
        foreach (var field in Fields())
        {
            if (field.IsCreateField)
                field.SetDeadline(false);
        }
    }

    public bool FindMatchAble()
    {
        ableField.Clear();

        ChkRowAble();
        ChkColAble();

        return ableField.Count > 0 ? true : false;
    }

    public bool FindMatch()
    {
        rowMatchedSet.Clear();
        colMatchedSet.Clear();
        rowSpecialSet.Clear();
        colSpecialSet.Clear();
        crossSpecialSet.Clear();

        ChkMatch(1, lastRow, 1, lastCol, true);
        ChkMatch(1, lastRow, 1, lastCol, false);

        CheckCrossMatchSet();

        if (rowMatchedSet.Count + colMatchedSet.Count + rowSpecialSet.Count + colSpecialSet.Count + crossSpecialSet.Count > 0)
            return true;

        return false;
    }

    public void ExcuteMatch()
    {
        for (int i = 0; i < rowMatchedSet.Count; i++)
        {
            for (int j = 0; j < rowMatchedSet[i].fields.Length; j++)
            {
                rowMatchedSet[i].fields[j].Match();
            }
        }

        for (int i = 0; i < colMatchedSet.Count; i++)
        {
            for (int j = 0; j < colMatchedSet[i].fields.Length; j++)
            {
                colMatchedSet[i].fields[j].Match();
            }
        }

        for (int i = 0; i < rowSpecialSet.Count; i++)
        {
            rowSpecialSet[i].makeOverField.MakeOver(rowSpecialSet[i].specialType);
            for (int j = 0; j < rowSpecialSet[i].fields.Length; j++)
            {
                rowSpecialSet[i].fields[j].MakeOverDissolve(rowSpecialSet[i].makeOverField);
            }
        }

        for (int i = 0; i < colSpecialSet.Count; i++)
        {
            colSpecialSet[i].makeOverField.MakeOver(colSpecialSet[i].specialType);
            for (int j = 0; j < colSpecialSet[i].fields.Length; j++)
            {
                colSpecialSet[i].fields[j].MakeOverDissolve(colSpecialSet[i].makeOverField);
            }
        }

        for (int i = 0; i < crossSpecialSet.Count; i++)
        {
            crossSpecialSet[i].makeOverField.MakeOver(crossSpecialSet[i].specialType);
            for (int j = 0; j < crossSpecialSet[i].fields.Length; j++)
            {
                crossSpecialSet[i].fields[j].MakeOverDissolve(crossSpecialSet[i].makeOverField);
            }
        }

        swapField[0] = null;
        swapField[1] = null;
    }

    //필드가 비어있는지 체크한다. 
    public bool IsNotEmpty()
    {
        bool result = true;

        foreach (var field in Fields())
        {
            result &= !field.IsEmpty;
        }

        return result;
    }

    #region 임시 스킬 실행 코드 
    public IEnumerator Skill_LineGaro(BlockField center)
    {
        yield return true;
        center.Match();

        yield return new WaitForEndOfFrame();

        int l = center.Col;
        int r = l;

        while (l > 0 || r < colArrLastIdx)
        {
            l--;
            r++;
            if (l > 0)
                fields[center.Row, l].Match();
            if (r < colArrLastIdx)
                fields[center.Row, r].Match();

            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator Skill_LineSero(BlockField center)
    {
        yield return true;
        center.Match();

        yield return new WaitForEndOfFrame();

        int l = center.Row;
        int r = l;

        while (l > 0 || r < rowArrLastIdx)
        {
            l--;
            r++;

            if (l > 0)
                fields[l, center.Col].Match();
            if (r < rowArrLastIdx)
                fields[r, center.Col].Match();

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Skill_SmallBomb(BlockField center)
    {
        yield return true;

        center.Match();

        yield return new WaitForEndOfFrame();

        GetBlockField(center.Row, center.Col, 2).Match();
        GetBlockField(center.Row, center.Col, 4).Match();
        GetBlockField(center.Row, center.Col, 6).Match();
        GetBlockField(center.Row, center.Col, 8).Match();

    }

    public IEnumerator Skill_MiddleBomb(BlockField center)
    {
        yield return true;

        center.Match();

        yield return new WaitForEndOfFrame();

        GetBlockField(center.Row, center.Col, 2).Match();
        GetBlockField(center.Row, center.Col, 4).Match();
        GetBlockField(center.Row, center.Col, 6).Match();
        GetBlockField(center.Row, center.Col, 8).Match();

        yield return new WaitForEndOfFrame();

        GetBlockField(center.Row, center.Col, 1).Match();
        GetBlockField(center.Row, center.Col, 3).Match();
        GetBlockField(center.Row, center.Col, 7).Match();
        GetBlockField(center.Row, center.Col, 9).Match();

        yield return new WaitForEndOfFrame();

    }

    public IEnumerator Skill_BigBomb(BlockField center)
    {
        yield return true;
        center.Match();

        yield return new WaitForEndOfFrame();

        GetBlockField(center.Row, center.Col, 2).Match();
        GetBlockField(center.Row, center.Col, 4).Match();
        GetBlockField(center.Row, center.Col, 6).Match();
        GetBlockField(center.Row, center.Col, 8).Match();

        yield return new WaitForEndOfFrame();

        GetBlockField(center.Row, center.Col, 1).Match();
        GetBlockField(center.Row, center.Col, 3).Match();
        GetBlockField(center.Row, center.Col, 7).Match();
        GetBlockField(center.Row, center.Col, 9).Match();

        yield return new WaitForEndOfFrame();

        BlockField t;
        t = GetBlockField(center.Row, center.Col, 2);
        GetBlockField(t.Row, t.Col, 2).Match();
        t = GetBlockField(center.Row, center.Col, 4);
        GetBlockField(t.Row, t.Col, 4).Match();
        t = GetBlockField(center.Row, center.Col, 6);
        GetBlockField(t.Row, t.Col, 6).Match();
        t = GetBlockField(center.Row, center.Col, 8);
        GetBlockField(t.Row, t.Col, 8).Match();
    }


    #endregion

    #region Search Matchable Pattern

    //row와 row+2가 동일한 블럭을 검색해서 일치할 경우 매치가 가능한 패턴인지 확인한다. 
    void ChkRowAble()
    {
        for (int row = 1; row <= lastRow; row++)
        {
            for (int col = 1; col <= lastCol - 2; col++)
            {
                if (fields[row, col].BlockType == fields[row, col + 2].BlockType)
                    ChkRowPattern(row, col);
            }
        }
    }

    void ChkColAble()
    {
        for (int col = 1; col <= lastCol; col++)
        {
            for (int row = 1; row <= lastRow - 2; row++)
            {
                if (fields[row, col].BlockType == fields[row + 2, col].BlockType)
                    ChkColPattern(row, col);
            }
        }
    }


    /// ◆ : 검색 기준
    /// △ : 비교 대상
    /// ■ : 기준 블럭과 동일 블럭
    /// ☆ : 매치가 가능한 위치

    /// 1       2
    /// ◆☆■△ △◆☆■
    /// 
    /// 3     4
    /// ◆☆■ □△□  
    /// □△□ ◆☆■
    void ChkRowPattern(int row, int col)
    {
        if (fields[row, col].BlockType == fields[row, col + 3].BlockType ||
            fields[row, col].BlockType == fields[row, col - 1].BlockType ||
            fields[row, col].BlockType == fields[row + 1, col + 1].BlockType ||
            fields[row, col].BlockType == fields[row - 1, col + 1].BlockType)
        {
            ableField.Add(fields[row, col + 1]);
        }
    }

    /// 1   2   3     4
    /// ◆  △   □◆  ◆□ 
    /// ☆  ◆   △☆  ☆△ 
    /// ■  ☆   □■  ■□ 
    /// △  ■             
    void ChkColPattern(int row, int col)
    {
        if (fields[row, col].BlockType == fields[row + 3, col].BlockType ||
            fields[row, col].BlockType == fields[row - 1, col].BlockType ||
            fields[row, col].BlockType == fields[row + 1, col + 1].BlockType ||
            fields[row, col].BlockType == fields[row + 1, col - 1].BlockType)
        {
            ableField.Add(fields[row + 1, col]);
        }
    }
    #endregion


    #region Search Match Pattern

    List<BlockField> matchBuffer = new List<BlockField>();
    BlockField[] swapField = new BlockField[2];


    //유저가 Swap한 필드가 있을 경우 해당 필드를 특수블럭이 생성될 필드로 지정한다. 
    private bool CheckMakeOverFieldByUser(BlockField target, ref MatchedSet set)
    {
        for (int idx = 0; idx < 2; idx++)
        {
            if (swapField[idx] != null && swapField[idx] == target)
            {
                set.makeOverField = swapField[idx];
                swapField[idx] = null;

                return true;
            }
        }

        return false;
    }

    //범위 내에서 3개 이상의 연속으로 동일한 블럭을 검사한다. 
    //isRow 플래그에 따라 가로, 세로 패턴을 구분한다. 
    void ChkMatch(int minRow, int maxRow, int minCol, int maxCol, bool isRow)
    {
        int l, r, cnt, min, max, rLimit;

        if (isRow)
        {
            min = minRow;
            max = maxRow;
            rLimit = maxCol + 1;
        }
        else
        {
            min = minCol;
            max = maxCol;
            rLimit = maxRow + 1;
        }


        //라인마다 l과 r을 비교해서 동일한 블럭이 연속으로 있는지 검사한다. 
        for (int line = min; line <= max; line++)
        {
            if (isRow)
                l = minCol;
            else
                l = minRow;
            r = l + 1;
            cnt = 0;

            //라인을 시작할 때 l이 비교대상인지 확인한다. 
            if (!GetFieldFlip(line, l, isRow).IsPlayable)
            {
                l++;
                r = l + 1;
            }
            while (r <= rLimit)
            {
                //l과 r이 동일할 경우 r을 1씩 증가시켜서 동일한 블럭 개수를 카운트한다. 
                BlockField fieldR = GetFieldFlip(line, r, isRow);

                if (r < rLimit && fieldR.IsPlayable && 
                    fieldR.BlockType >= GlobalVal.BLOCKTYPE_NORMAL_MIN && 
                    fieldR.BlockType <= GlobalVal.BLOCKTYPE_NORMAL_MAX &&
                    GetFieldFlip(line, l, isRow).BlockType == fieldR.BlockType)
                {
                    cnt++;
                }
                else
                {
                    //동일 블럭이 3개이상일 경우 매치리스트에 포함시키고 l을 r위치로 옮기고 r을 1증가시킨다. 
                    //l과 r이 동일하지 않을 경우 l을 r위치로 옮기고 r을 1증가시킨다. 
                    if (cnt == 2)
                    {
                        //cnt+1 match!
                        MatchedSet set = new MatchedSet();
                        set.fields = new BlockField[3];
                        for (int idx = 0; idx <= cnt; idx++)
                        {
                            set.fields[idx] = GetFieldFlip(line, l + idx, isRow);
                            //Debug.Log("Row Match (" + line + ", " + (l + i).ToString() + ") " + fields[line,l+i].BlockType.type);
                        }
                        set.blockType = set.fields[0].BlockType;

                        if (isRow)
                            rowMatchedSet.Add(set);
                        else
                            colMatchedSet.Add(set);
                    }
                    else if (cnt > 2)
                        MakeSpecialMatchSet(cnt, line, l, isRow);

                    l = r;
                    cnt = 0;
                }
                r++;
            }
        }
    }

    void MakeSpecialMatchSet(int cnt, int line, int l, bool isRow)
    {
        matchBuffer.Clear();
        MatchedSet set = new MatchedSet();

        switch (cnt)
        {
            case 3:
                if (isRow)
                    set.specialType = GlobalVal.BLOCKTYPE_SKILL_SERO;
                else
                    set.specialType = GlobalVal.BLOCKTYPE_SKILL_SERO;
                break;
            case 4:
            default:
                set.specialType = GlobalVal.BLOCKTYPE_SKILL_MIDDLEBOMB;
                break;
        }

        int makeOverIdx = 0;

        //매치 리스트 중에 swap필드가 있다면 해당 필드를 makeOverField로 지정한다. 
        for (int idx = 0; idx <= cnt; idx++)
        {
            if (CheckMakeOverFieldByUser(GetFieldFlip(line, l + idx, isRow), ref set))
            {
                makeOverIdx = idx;
                break;
            }
        }

        //swap필드가 없다면 makeOverField를 랜덤지정한다. 
        if (set.makeOverField == null)
        {
            makeOverIdx = UnityEngine.Random.Range(0, cnt);
            set.makeOverField = GetFieldFlip(line, l + makeOverIdx, isRow);
        }

        //makeOverField를 제외한 필드를 set에 삽입한다. 
        for (int idx = 0; idx <= cnt; idx++)
        {
            if (idx != makeOverIdx)
                matchBuffer.Add(GetFieldFlip(line, l + idx, isRow));
        }
        set.fields = matchBuffer.ToArray();

        set.blockType = set.makeOverField.BlockType;

        if (isRow)
            rowSpecialSet.Add(set);
        else
            colSpecialSet.Add(set);
    }

    BlockField GetFieldFlip(int line, int curIdx, bool isRow)
    {
        return isRow ? fields[line, curIdx] : fields[curIdx, line];
    }

    //행 매치셋과 열 매치셋간에 교차필드가 있는지 검사해서 크로스 패턴이 있는지 검색하고 
    //있을 경우 기존 매치셋을 수정하고 크로스 매치셋을 생성한다. 
    void CheckCrossMatchSet()
    {
        ChkCrossBetween_List(ref rowMatchedSet, ref colMatchedSet);
        ChkCrossBetween_List(ref rowMatchedSet, ref colSpecialSet);
        ChkCrossBetween_List(ref rowSpecialSet, ref colMatchedSet);
        ChkCrossBetween_List(ref rowSpecialSet, ref colSpecialSet);
    }

    //매칭셋트 리스트끼리 비교하여 세트간에 교차 필드가 있는지 검사한다. 
    //교차필드가 있을 경우 두 세트를 하나로 병합한다. 
    //병합 세트를 크로스 세트 리스트에 삽입하고 기존 세트를 기존 리스트에서 제거한다. 
    private void ChkCrossBetween_List(ref List<MatchedSet> list1, ref List<MatchedSet> list2)
    {
        if (list1.Count == 0 || list2.Count == 0)
            return;
        
        //역방향으로 검색해야 중간에 삭제가 발생해도 index 오류가 발생하지 않고 전체 순회를 한다.
        for (int i = list1.Count-1; i >= 0; i--)
        {
            for (int j = list2.Count-1; j >= 0; j--)
            {
                BlockField crossField = null;

                if (i < 0 || i >= list1.Count || j < 0 || j >= list2.Count)
                    Debug.LogError(i + " " + list1.Count + " // " + j + " " + list2.Count);

                if (ChkCrossBetween_MatchedSet(list1[i], list2[j], out crossField))
                {
                    MergeCrossMatche(crossField, list1[i], list2[j]);
                    list1.RemoveAt(i);
                    list2.RemoveAt(j);

                    break;
                }
            }

            if (list1.Count == 0 || list2.Count == 0)
                return;
        }
    }

    bool ChkCrossBetween_MatchedSet(MatchedSet set1, MatchedSet set2, out BlockField crossField)
    {
        crossField = null;

        if (set1.blockType != set2.blockType)
            return false;

        if (set1.makeOverField != null && (set1.makeOverField == set2.makeOverField))
        {
            crossField = set1.makeOverField;
            return true;
        }

        for (int i = 0; i < set1.fields.Length; i++)
        {
            if (set1.fields[i] == set2.makeOverField)
            {
                crossField = set2.makeOverField;
                return true;
            }
            for (int j = 0; j < set2.fields.Length; j++)
            {
                if (set1.makeOverField == set2.fields[j] ||
                    set1.fields[i] == set2.fields[j])
                {
                    crossField = set2.fields[j];
                    return true;
                }
            }
        }

        return false;
    }

    void MergeCrossMatche(BlockField crossField, MatchedSet set1, MatchedSet set2)
    {
        MatchedSet mergeSet = new MatchedSet();
        mergeSet.makeOverField = crossField;
        mergeSet.blockType = mergeSet.makeOverField.BlockType;

        matchBuffer.Clear();

        for (int i = 0; i < set1.fields.Length; i++)
        {
            if (set1.fields[i] == mergeSet.makeOverField)
                continue;

            matchBuffer.Add(set1.fields[i]);
        }

        for (int i = 0; i < set2.fields.Length; i++)
        {
            if (set2.fields[i] == mergeSet.makeOverField)
                continue;

            matchBuffer.Add(set2.fields[i]);
        }
        
        mergeSet.fields = matchBuffer.ToArray();

        switch (mergeSet.fields.Length)
        {
            case 4:
                mergeSet.specialType = GlobalVal.BLOCKTYPE_SKILL_SMALLBOMB;
                break;
            case 5:
                mergeSet.specialType = GlobalVal.BLOCKTYPE_SKILL_MIDDLEBOMB;
                break;
            case 6:
            default:
                mergeSet.specialType = GlobalVal.BLOCKTYPE_SKILL_BIGBOMB;
                break;
        }
                
        crossSpecialSet.Add(mergeSet);
    }
    #endregion

    #region GetFields
    public IEnumerable<BlockField> GetArounds(BlockField centerField)
    {
        for (int i = centerField.Row - 1; i < 3; i++)
        {
            for (int j = centerField.Col - 1; j < 3; j++)
            {
                yield return fields[i, j];
            }
        }
    }

    public IEnumerable<BlockField> GetAroundsFour(BlockField centerField)
    {
        yield return GetBlockField(centerField.Row, centerField.Col, 2);
        yield return GetBlockField(centerField.Row, centerField.Col, 4);
        yield return GetBlockField(centerField.Row, centerField.Col, 6);
        yield return GetBlockField(centerField.Row, centerField.Col, 8);
    }

    public BlockField GetNextByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 8);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 4);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 2);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 6);
        }
    }

    public BlockField GetReverseNextByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 2);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 6);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 8);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 4);
        }
    }

    public BlockField GetRightByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 6);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 8);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 4);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 2);
        }
    }

    public BlockField GetLeftByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 4);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 2);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 6);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 8);
        }
    }

    public BlockField GetRightUpDiagnalByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 3);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 9);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 7);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 1);
        }
    }

    public BlockField GetLeftUpDiagnalByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 1);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 3);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 9);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 7);
        }
    }

    public BlockField GetRightDownDiagnalByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 9);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 7);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 1);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 3);
        }
    }

    public BlockField GetLeftDownDiagnalByDir(BlockField centerField)
    {
        switch (centerField.Direction)
        {
            default:
            case 0://down
                return GetBlockField(centerField.Row, centerField.Col, 7);
            case 1://left
                return GetBlockField(centerField.Row, centerField.Col, 1);
            case 2://up
                return GetBlockField(centerField.Row, centerField.Col, 3);
            case 3://right
                return GetBlockField(centerField.Row, centerField.Col, 9);
        }
    }

    /// <summary>
    /// 1 2 3
    /// 4   6
    /// 7 8 9
    /// 5 위치를 기준으로 숫자에 해당하는 상대위치의 필드 반환
    /// </summary>
    public BlockField GetBlockField(int row, int col, int dir = 5)
    {
        switch (dir)
        {
            default:
            case 5:
                return fields[row, col];
            case 1:
                if (row <= 0 || col <= 0)
                    return null;
                if (row > rowArrLastIdx || col > colArrLastIdx)
                    return null;
                return fields[row - 1, col - 1];
            case 2:
                if (row <= 0)
                    return null;
                if (row > rowArrLastIdx || col < 0 || col > colArrLastIdx)
                    return null;
                return fields[row - 1, col];
            case 3:
                if (row <= 0 || col >= colArrLastIdx)
                    return null;
                if (row > rowArrLastIdx || col < 0)
                    return null;
                return fields[row - 1, col + 1];
            case 4:
                if (col <= 0)
                    return null;
                if (row < 0 || row > rowArrLastIdx || col > colArrLastIdx)
                    return null;
                return fields[row, col - 1];
            case 6:
                if (col >= colArrLastIdx)
                    return null;
                if (row < 0 || row > rowArrLastIdx || col < 0)
                    return null;
                return fields[row, col + 1];
            case 7:
                if (row >= rowArrLastIdx || col <= 0)
                    return null;
                if (row < 0 || col > colArrLastIdx)
                    return null;
                return fields[row + 1, col - 1];
            case 8:
                if (row >= rowArrLastIdx)
                    return null;
                if (row < 0 || col < 0 || col > colArrLastIdx)
                    return null;
                return fields[row + 1, col];
            case 9:
                if (row >= rowArrLastIdx || col >= colArrLastIdx)
                    return null;
                if (row < 0 || col < 0)
                    return null;
                return fields[row + 1, col + 1];
        }
    }

    #endregion

}