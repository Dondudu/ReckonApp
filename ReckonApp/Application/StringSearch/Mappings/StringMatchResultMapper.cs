using ReckonApp.Domain.Models;
using ReckonApp.Infrastructure.ExternalApi.Models;

namespace ReckonApp.Application.StringSearch.Mappings
{
    public class StringMatchResultMapper : IStringMatchResultMapper
    {
        public SubmitResultsModel Map(StringMatchResult model, string text, string candidateName)
        {
            return new SubmitResultsModel()
            {
                Candidate = candidateName,
                Results = model.SubStringIndices.Select(kvp => new SubmitResultsItem(kvp.Key,
                kvp.Value.Count > 0
                ? string.Join(",", kvp.Value)
                : "<No Output>")).ToList(),
                Text = text
            };
        }
    }
}
