using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Utils
{
    internal class PayloadGenerator
    {
        private static readonly Random Random = new();

        public static byte[] GeneratePayload(int sizeInKB)
        {
            var bytes = new byte[sizeInKB * 1024];
            Random.NextBytes(bytes);
            return bytes;
        }

        public static string GenerateStringPayload(int sizeInKB)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var length = sizeInKB * 1024;
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[Random.Next(chars.Length)];
            }

            return new string(result);
        }

        public static Dictionary<string, object> GenerateComplexPayload(int approximateSizeKB)
        {
            var payload = new Dictionary<string, object>();
            var currentSize = 0;
            var counter = 0;

            while (currentSize < approximateSizeKB * 1024)
            {
                var key = $"Property_{counter++}";
                var randomType = Random.Next(0, 4);
                object value = randomType switch
                {
                    0 => Random.Next(1000000),
                    1 => GenerateStringPayload(1), // 1KB 문자열
                    2 => DateTime.UtcNow.AddDays(Random.Next(-365, 365)),
                    3 => Random.NextDouble() * 1000000,
                    _ => Guid.NewGuid()
                };

                payload[key] = value;
                currentSize += EstimateSize(value);
            }

            return payload;
        }

        private static int EstimateSize(object value)
        {
            return value switch
            {
                string s => s.Length * 2, // Unicode 문자는 2바이트
                int => 4,
                long => 8,
                double => 8,
                DateTime => 8,
                Guid => 16,
                _ => 50 // 기본 추정값
            };
        }
    }
}
