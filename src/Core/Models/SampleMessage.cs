using System.Reflection;
using System.Text;

namespace Core.Models
{
    // condition
    public enum MessageSize
    {
        Small = 1,   // 1KB
        Medium = 10, // 10KB  
        Large = 100  // 100KB
    }

    public enum MessageFrequency
    {
        Low = 100,    // 100 msg/sec
        Medium = 1000, // 1K msg/sec
        High = 5000   // 5K msg/sec
    }

    public enum LoadCondition
    {
        Light,    // Small × Low
        Standard, // Medium × Medium
        Heavy     // Large × High
    }

    // data
    public class PaddingData
    {
        public required string Property1 { get; set; }
        public decimal Property2 { get; set; }
        public required List<SubPaddingData> Property3 { get; set; }
        public List<SubPaddingData>? Property4 { get; set; }
    }

    public class SubPaddingData
    {
        public required string SubProperty1 { get; set; }
        public int SubProperty2 { get; set; }
    }

    public class SampleMessage
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public required List<PaddingData> PaddingData { get; set; }  // to control msg size

        // load json data according to the given size condition
        public SampleMessage()
        {
            
        }
        public static string LoadJsonData(MessageSize size)
        {

            string fileName = size switch
            {
                MessageSize.Small => "small.json",
                MessageSize.Medium => "medium.json",
                MessageSize.Large => "large.json",
            };

            string dataPath = Path.Combine("C:\\_Nile\\00_Projects\\dotnet-messaging-bottlenecks\\src\\Core\\Data", fileName);

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Looking for file at: {dataPath}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] File exists: {File.Exists(dataPath)}");

            return File.ReadAllText(dataPath, Encoding.UTF8);
        }
    }
}