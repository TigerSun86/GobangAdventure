public interface ICsvRowParser<T>
{
    T ParseRow(string[] values, string[] headers);
}