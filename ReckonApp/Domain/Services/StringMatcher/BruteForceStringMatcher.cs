using ReckonApp.Domain.Models;

namespace ReckonApp.Domain.Services.StringMatcher
{
    public class BruteForceStringMatcher : IStringMatcher
    {
        public StringMatchResult MatchStrings(string stringToSearch, List<string> subStrings)
        {
            var result = new Dictionary<string, List<int>>();
            foreach (var subString in subStrings)
            {
                result.Add(subString, FindSubstrings(stringToSearch, subString));
            }
            return new StringMatchResult() { SubStringIndices = result };
        }

        static List<int> FindSubstrings(string stringToSearch, string subString)
        {
            var result = new List<int>();
            stringToSearch = stringToSearch.ToLower();
            subString = subString.ToLower();

            for (int i = 0; i < stringToSearch.Length - subString.Length + 1; i++)
            {
                if(subString.Length == 0)
                    continue;
                bool stringFound = true;
                for (int j = 0; j < subString.Length; j++)
                {
                    if (stringToSearch[i + j] != subString[j])
                    {
                        stringFound = false;
                        break;
                    }
                }
                if (stringFound)
                    result.Add(i + 1);
            }

            return result;
        }
    }
}
