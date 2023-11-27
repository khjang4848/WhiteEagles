namespace WhiteEagles.Infrastructure.Serialization
{
    public interface ITextSerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string serialized);
    }
}
