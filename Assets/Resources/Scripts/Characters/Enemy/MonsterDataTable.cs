using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MonsterDataTable : MonoBehaviour
{
    public static MonsterDataTable Instance;
    public Dictionary<int, MonsterData> monsterDic = new();

    private void Awake()
    {
        Instance = this;
        LoadMonsterCSV();
    }

    void LoadMonsterCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("Data/MonsterData");

        if (csv == null)
        {
            Debug.LogError("MonsterData.csv 파일을 찾을 수 없습니다.");
            return;
        }

        StringReader reader = new StringReader(csv.text);

        // 헤더 읽기
        string headerLine = reader.ReadLine();
        if (string.IsNullOrEmpty(headerLine))
        {
            Debug.LogError("CSV 헤더가 비어 있습니다.");
            return;
        }

        int lineIndex = 1;

        while (true)
        {
            string line = reader.ReadLine();
            if (line == null)
                break;

            lineIndex++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] v = line.Split(',');

            try
            {
                MonsterData data = new MonsterData
                {
                    ID = int.Parse(v[0]),
                    Name = v[1],
                    RemainTurn = int.Parse(v[2]),
                    AttackRange = int.Parse(v[3]),
                    MonsterType = (MonsterType)System.Enum.Parse(
                        typeof(MonsterType), v[4]),
                    Hp = int.Parse(v[5]),
                    Atk = int.Parse(v[6])
                };

                monsterDic.Add(data.ID, data);
            }
            catch (System.Exception e)
            {
                Debug.LogError(
                    $"MonsterData.csv {lineIndex}번째 줄 파싱 실패\n{e.Message}");
            }
        }

        Debug.Log($"MonsterData 로드 완료 : {monsterDic.Count}개");
    }
}
