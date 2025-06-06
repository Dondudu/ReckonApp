using ReckonApp.API.ViewModels;
using ReckonApp.Application.StringSearch.Commands;

namespace ReckonApp.Application.StringSearch.Mappings
{
    public interface IStringSearchCommandResultMapper
    {
        StringSearchViewModel Map(StringMatchCommandResult submitResultsModel);
    }
}