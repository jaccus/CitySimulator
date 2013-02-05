using System.Collections.Generic;

namespace City
{
    public class SymulationType
    {
        public static string ShortestDistance = "Shortest Distance";
        public static string Random = "Random";

        public string Label { get; set; }
        public bool IsEnabled { get; set; }
        public int Index { get; set; }

        public static IEnumerable<string> List()
        {
            return new List<string> {ShortestDistance, Random};
        }

        public override string ToString()
        {
            return Label;
        }
    }
}