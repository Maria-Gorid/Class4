using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Task1
{

    public class Task1
    {
        public record IPv4Addr(string StrValue) : IComparable<IPv4Addr>
        {
            public uint IntValue = Ipstr2Int(StrValue);

            private static uint Ipstr2Int(string ip)
            {
                var ipParts = ip.Split('.');
                return (uint)(
                    Convert.ToUInt32(ipParts[0]) * Math.Pow(2, 24) +
                    Convert.ToUInt32(ipParts[1]) * Math.Pow(2, 16) +
                    Convert.ToUInt32(ipParts[2]) * Math.Pow(2, 8) +
                    Convert.ToUInt32(ipParts[3]));
            }

            public int CompareTo(IPv4Addr? other)
            {
                if (other != null) 
                    return this.IntValue.CompareTo(other.IntValue);
                return 0;
            }
        }

        public record IPRange(IPv4Addr IpFrom, IPv4Addr IpTo);

        public record IPLookupArgs(string IpsFile, List<string> IprsFiles);

        public static IPLookupArgs ParseArgs(string ipsFile, List<string> iprsFiles)
        {
            return new IPLookupArgs(ipsFile, iprsFiles);
        }

        public static List<string> LoadQuery(string filename)
        {
            return new List<string>(File.ReadAllLines(filename));
        }

        public static SortedDictionary<IPv4Addr, string> LoadRanges(List<string> filenames)
        {
            var ranges = new SortedDictionary<IPv4Addr, string>();
            foreach (var filePath in filenames)
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var iprs = line.Split(',');
                    var keyIp = new IPv4Addr(iprs[0]);
                    ranges.Add(keyIp, line);
                }
            }

            return ranges;
        }

        public static IPRange? FindRange(SortedDictionary<IPv4Addr, string> ranges, IPv4Addr query)
        {
            foreach (var range in ranges)
            {
                var iprs = range.Value.Split(',');
                var ipFrom = new IPv4Addr(iprs[0]);
                var ipTo = new IPv4Addr(iprs[1]);
                if (query.IntValue >= ipFrom.IntValue && query.IntValue <= ipTo.IntValue)
                    return new IPRange(ipFrom, ipTo);
            }
            
            return null;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Enter IPS file:");
            var ipsFile = Console.ReadLine();

            Console.WriteLine("Enter IPRS files (separated by space):");
            var iprsFiles = Console.ReadLine()?.Split(' ');

            if (string.IsNullOrWhiteSpace(ipsFile) || iprsFiles == null || iprsFiles.Length == 0)
            {
                Console.WriteLine("Invalid input. Program exiting.");
                return;
            }

            var ipLookupArgs = ParseArgs(ipsFile, new List<string>(iprsFiles));

            var queries = LoadQuery(ipLookupArgs.IpsFile);
            var ranges = LoadRanges(ipLookupArgs.IprsFiles);

            var outputLines = new List<string>();
            foreach (var ip in queries)
            {
                var findRange = FindRange(ranges, new IPv4Addr(ip));
                outputLines.Add(findRange != null
                    ? $"{ip}: YES ({findRange.IpFrom.StrValue} - {findRange.IpTo.StrValue})"
                    : $"{ip}: NO");
            }

            var outputFile = Path.ChangeExtension(ipLookupArgs.IpsFile, "out");
            File.WriteAllLines(outputFile, outputLines);

            Console.WriteLine($"Results saved to {outputFile}");
        }
    }
}