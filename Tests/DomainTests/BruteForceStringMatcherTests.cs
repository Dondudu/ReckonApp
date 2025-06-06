using ReckonApp.Domain.Services.StringMatcher;
using System.Diagnostics;
using System.Text;

namespace ReckonApp.Domain.Tests.Services.StringMatcher
{
    public class BruteForceStringMatcherTests
    {
        private readonly IStringMatcher _stringMatcher;

        public BruteForceStringMatcherTests()
        {
            _stringMatcher = new BruteForceStringMatcher();
        }

        [Fact]
        public void MatchStrings_ValidInput_ReturnsCorrectIndices()
        {
            // Arrange
            string text = "hello world hello";
            var subStrings = new List<string> { "hello", "world" };
            var expected = new Dictionary<string, List<int>>
            {
                { "hello", new List<int> { 1, 13 } },
                { "world", new List<int> { 7 } }
            };

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Equal(expected["hello"], result.SubStringIndices["hello"]);
            Assert.Equal(expected["world"], result.SubStringIndices["world"]);
        }

        [Fact]
        public void MatchStrings_CaseInsensitive_ReturnsCorrectIndices()
        {
            // Arrange
            string text = "Hello WORLD hello";
            var subStrings = new List<string> { "HELLO", "world" };
            var expected = new Dictionary<string, List<int>>
            {
                { "HELLO", new List<int> { 1, 13 } },
                { "world", new List<int> { 7 } }
            };

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Equal(expected["HELLO"], result.SubStringIndices["HELLO"]);
            Assert.Equal(expected["world"], result.SubStringIndices["world"]);
        }

        [Fact]
        public void MatchStrings_NoMatches_ReturnsEmptyLists()
        {
            // Arrange
            string text = "hello world";
            var subStrings = new List<string> { "xyz", "abc" };
            var expected = new Dictionary<string, List<int>>
            {
                { "xyz", new List<int>() },
                { "abc", new List<int>() }
            };

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Equal(expected["xyz"], result.SubStringIndices["xyz"]);
            Assert.Equal(expected["abc"], result.SubStringIndices["abc"]);
        }

        [Fact]
        public void MatchStrings_EmptyText_ReturnsEmptyLists()
        {
            // Arrange
            string text = "";
            var subStrings = new List<string> { "hello", "world" };
            var expected = new Dictionary<string, List<int>>
            {
                { "hello", new List<int>() },
                { "world", new List<int>() }
            };

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Equal(expected["hello"], result.SubStringIndices["hello"]);
            Assert.Equal(expected["world"], result.SubStringIndices["world"]);
        }

        [Fact]
        public void MatchStrings_EmptySubStrings_ReturnsEmptyDictionary()
        {
            // Arrange
            string text = "hello world";
            var subStrings = new List<string>();

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Empty(result.SubStringIndices);
        }

        [Fact]
        public void MatchStrings_EmptySubString_ReturnsEmptyList()
        {
            // Arrange
            string text = "hello world";
            var subStrings = new List<string> { "" };
            var expected = new Dictionary<string, List<int>>
            {
                { "", new List<int>() }
            };

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Equal(expected[""], result.SubStringIndices[""]);
        }

        [Fact]
        public void MatchStrings_OverlappingSubstrings_ReturnsAllMatches()
        {
            // Arrange
            string text = "aaa";
            var subStrings = new List<string> { "aa" };
            var expected = new Dictionary<string, List<int>>
            {
                { "aa", new List<int> { 1, 2 } }
            };

            // Act
            var result = _stringMatcher.MatchStrings(text, subStrings);

            // Assert
            Assert.Equal(expected["aa"], result.SubStringIndices["aa"]);
        }


        [Fact]
        public void MatchStrings_LargeTextAndMultipleSubstrings_PerformsWithinThreshold()
        {
            // Arrange
            // Generate a large text
            var textLength = 500000;
            var random = new Random(42); // Fixed seed for reproducibility
            var textBuilder = new StringBuilder(textLength);
            for (int i = 0; i < textLength; i++)
            {
                textBuilder.Append((char)('a' + random.Next(0, 26)));
            }
            string largeText = textBuilder.ToString();

            // Create substrings: 10 substrings of varying lengths
            var subStrings = new List<string>();
            var expectedIndices = new Dictionary<string, List<int>>();
            for (int i = 0; i < 10; i++)
            {
                int subStringLength = 10 + i * 50;
                string subString = largeText.Substring(i * 50, subStringLength);
                subStrings.Add(subString);
                // Expected indices (1-based, as per implementation)
                expectedIndices[subString] = new List<int> { i * 50 + 1 };
            }
            // Add a substring that doesn't exist
            subStrings.Add(new string('z', 100));
            expectedIndices[new string('z', 100)] = new List<int>();

            // Define performance threshold (1 second, adjust as needed)
            TimeSpan maxDuration = TimeSpan.FromSeconds(1);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var result = _stringMatcher.MatchStrings(largeText, subStrings);
            stopwatch.Stop();

            // Assert
            // Verify performance
            Assert.True(stopwatch.Elapsed <= maxDuration,
                $"Execution took {stopwatch.Elapsed.TotalMilliseconds:F2}ms, exceeded {maxDuration.TotalMilliseconds}ms");

            // Verify correctness
            foreach (var subString in subStrings)
            {
                Assert.Equal(expectedIndices[subString], result.SubStringIndices[subString]);
            }
        }
    }
}