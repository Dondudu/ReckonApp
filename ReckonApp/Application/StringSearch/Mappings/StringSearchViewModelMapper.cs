using ReckonApp.API.ViewModels;
using ReckonApp.Application.StringSearch.Commands;
using ReckonApp.Infrastructure.ExternalApi.Models;

namespace ReckonApp.Application.StringSearch.Mappings
{
    public class StringSearchCommandResultMapper : IStringSearchCommandResultMapper
    {
        public StringSearchViewModel Map(StringMatchCommandResult submitResultsModel)
        {
            return new StringSearchViewModel()
            {
                Candidate = submitResultsModel.Candidate,
                Results = submitResultsModel.Results.Select(x => new API.ViewModels.Item() { Subtext = x.Subtext, Result = x.Result }).ToList(),
                Text = submitResultsModel.Text
            };
        }
    }
}
