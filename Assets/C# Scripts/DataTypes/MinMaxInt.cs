


/// <summary>
/// A lightweight struct that holds an int min and int max.
/// </summary>
[System.Serializable]
public struct MinMaxInt
{
    public int min;
    public int max;

    public MinMaxInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }


    // Math oerators
    public static MinMaxInt operator +(MinMaxInt a, MinMaxInt b)
    {
        return new MinMaxInt(a.min + b.min, a.max + b.max);
    }
    public static MinMaxInt operator -(MinMaxInt a, MinMaxInt b)
    {
        return new MinMaxInt(a.min - b.min, a.max - b.max);
    }
    public static MinMaxInt operator *(MinMaxInt a, MinMaxInt b)
    {
        return new MinMaxInt(a.min * b.min, a.max * b.max);
    }
    public static MinMaxInt operator *(MinMaxInt a, int b)
    {
        return new MinMaxInt(a.min * b, a.max * b);
    }
}