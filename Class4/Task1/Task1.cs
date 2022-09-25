using System.Runtime.CompilerServices;

namespace Task1
{
    // Необходимо заменить на более подходящий тип (коллекцию), позволяющий
    // эффективно искать диапазон по заданному IP-адресу
    using IPRangesDatabase = List<Task1.IPRange>;

    public class Task1
    {
        /*
        * Объекты этого класса создаются из строки, но хранят внутри помимо строки
        * ещё и целочисленное значение соответствующего адреса. Например, для адреса
         * 127.0.0.1 должно храниться число 1 + 0 * 2^8 + 0 * 2^16 + 127 * 2^24 = 2130706433.
        */
        internal record IPv4Addr(string StrValue) : IComparable<IPv4Addr>
        {
            internal readonly uint IntValue = Ipstr2Int(StrValue);

            private static uint Ipstr2Int(string StrValue)
            {
                var separated = StrValue.Split(".");
                uint num = Convert.ToUInt32(separated[0]) * Convert.ToUInt32(Math.Pow(2, 24)) +
                           Convert.ToUInt32(separated[1]) * Convert.ToUInt32(Math.Pow(2, 16)) +
                           Convert.ToUInt32(separated[2]) * Convert.ToUInt32(Math.Pow(2, 8)) +
                           Convert.ToUInt32(separated[3]);
                return num;
            }

            // Благодаря этому методу мы можем сравнивать два значения IPv4Addr
            public int CompareTo(IPv4Addr other)
            {
                return IntValue.CompareTo(other.IntValue);
            }

            public override string ToString()
            {
                return StrValue;
            }
        }

        internal record class IPRange(IPv4Addr IpFrom, IPv4Addr IpTo)
        {
            public override string ToString()
            {
                return $"{IpFrom},{IpTo}";
            }
        }

        internal record class IPLookupArgs(string IpsFile, List<string> IprsFiles);

        internal static IPLookupArgs? ParseArgs(string[] args)
        {
            if (args.Length < 2)
                return null;
            
            var ipsFile = args.First();
            if (!File.Exists(@ipsFile))
                return null;

            var iprsFiles = new List<string>();

            for (int i = 1; i < args.Length; i++)
            {
                iprsFiles.Add(args[i]);
                if (!File.Exists(@iprsFiles[i-1]))
                    return null;
            }

            return new IPLookupArgs(ipsFile, iprsFiles);
        }

        internal static List<string> LoadQuery(string filename)
        {
            return new List<string>(File.ReadAllLines(@filename));
        }

        internal static IPRangesDatabase LoadRanges(List<String> filenames)
        {
            var database = new IPRangesDatabase();
            foreach (var file in filenames)
            {
                var lines = File.ReadAllLines(@file);
                foreach (var line in lines)
                {
                    var start = line.Split(',').First();
                    var end = line.Split(',').Last();
                    database.Add(new IPRange(new IPv4Addr(start), new IPv4Addr(end)));
                }
            }

            return database;
        }

        internal static IPRange? FindRange(IPRangesDatabase ranges, IPv4Addr query)
        {
            foreach (var range in ranges)
            {
                if (query.CompareTo(range.IpFrom) >= 0 && query.CompareTo(range.IpTo) <= 0)
                {
                    return range;
                }
            }

            return null;
        }

        public static void Main(string[] args)
        {
            var ipLookupArgs = ParseArgs(args);
            if (ipLookupArgs == null)
            {
                return;
            }

            string outputFilePath = ipLookupArgs.IpsFile.Replace("ips", "out");
            if (File.Exists(outputFilePath)) 
                File.Delete(outputFilePath);
            using (FileStream fileStream = File.Create(@outputFilePath)){}
            using StreamWriter writer = new StreamWriter(@outputFilePath);

            var queries = LoadQuery(ipLookupArgs.IpsFile);
            var ranges = LoadRanges(ipLookupArgs.IprsFiles);
            foreach (var ip in queries)
            {
                var findRange = FindRange(ranges, new IPv4Addr(ip));
                var result = findRange == null ? "NO" : $"YES {findRange.IpFrom},{findRange.IpTo}";
                Console.WriteLine($"{ip}: {result}");
                writer.WriteLine($"{ip}: {result}");
            }
        }
    }
}