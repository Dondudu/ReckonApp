using ReckonApp.Domain.Models;
using ReckonApp.Infrastructure.ExternalApi.Models;

namespace ReckonApp.Application.StringSearch.Mappings
{
    public interface IStringMatchResultMapper
    {
        SubmitResultsModel Map(StringMatchResult model, string text, string candidateName);
    }
}