using MediatR;
using Microsoft.Extensions.Options;
using ReckonApp.Application.StringSearch.Mappings;
using ReckonApp.Domain.Services.StringMatcher;
using ReckonApp.Infrastructure.ExternalApi;
using ReckonApp.Models;

namespace ReckonApp.Application.StringSearch.Commands
{
    public class StringMatchCommand : IRequest<StringMatchCommandResult>
    {
    }

    public class StringMatchCommandHandler : IRequestHandler<StringMatchCommand, StringMatchCommandResult>
    {
        readonly IStringMatcher _stringMatcher;
        readonly IReckonApiClient _reckonApiClient;
        readonly IStringMatchResultMapper _mapper;
        readonly ApplicationSettings _candidateName;

        public StringMatchCommandHandler(IReckonApiClient reckonApiClient,
            IStringMatchResultMapper mapper,
            IStringMatcher stringMatcher,
            IOptions<ApplicationSettings> candidateName)
        {
            _reckonApiClient = reckonApiClient;
            _mapper = mapper;
            _candidateName = candidateName.Value;
            _stringMatcher = stringMatcher;
        }

        public async Task<StringMatchCommandResult> Handle(StringMatchCommand request, CancellationToken cancellationToken)
        {
            var stringToMatch = await _reckonApiClient.GetTextToSearchAsync();
            var subTexts = await _reckonApiClient.GetSubTextsAsync();
            var result = _stringMatcher.MatchStrings(stringToMatch.Text, subTexts.SubTexts);
            var submitResults = _mapper.Map(result, stringToMatch.Text, _candidateName.CandidateName);
            var success = await _reckonApiClient.PostSubmitResultsAsync(submitResults);
            if (!success)
                throw new Exception($"Failed to post results");

            return new StringMatchCommandResult()
            {
                Candidate = submitResults.Candidate,
                Results = submitResults.Results.Select(r => new Item(r.Subtext, r.Result)).ToList(),
                Text = submitResults.Text,
            };
        }
    }

    public class StringMatchCommandResult
    {
        public required string Candidate { get; set; }
        public required string Text { get; set; }
        public required List<Item> Results { get; set; }
    }

    public class Item
    {
        public string Subtext { get; set; }
        public string Result { get; set; }

        public Item(string subtext, string result)
        {
            Subtext = subtext;
            Result = result;
        }
    }
}
