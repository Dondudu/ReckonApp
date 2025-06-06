using ReckonApp.Domain.Models;

namespace ReckonApp.Domain.Services.StringMatcher
{
    public interface IStringMatcher
    {
        StringMatchResult MatchStrings(string stringToSearch, List<string> subStrings);
    }
}