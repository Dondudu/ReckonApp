using ReckonApp.Infrastructure.ExternalApi.Models;

namespace ReckonApp.Infrastructure.ExternalApi
{
    public interface IReckonApiClient
    {
        Task<SubTextsResult> GetSubTextsAsync();
        Task<StringToSearchResult> GetTextToSearchAsync();
        Task<bool> PostSubmitResultsAsync(SubmitResultsModel data);
    }
}
