[System.Serializable]
public class PlayerCard
{
    public CardBaseData data;
    public int level = 1;

    public float GetValue()
    {
        return data.baseValue + (level - 1) * data.valuePerLevel;
    }
}
