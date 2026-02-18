using System.Text;

namespace LongRunningTask.Domain;

public class StringProcessor
{
    /// <summary>
    /// Processes the input string by counting the occurrences of each character and encoding the original string in Base64 format.
    /// </summary>
    public string Process(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input string cannot be null or empty.");
        }

        var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

        var processedString = string.Join(
            "",
            input.GroupBy(c => c)
            .OrderBy(c => c.Key)
            .Select(c => $"{c.Key}{c.Count()}")
            );

        return $"{processedString}/{base64String}";
    }
}
