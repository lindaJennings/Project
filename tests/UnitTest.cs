using Bunit;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace app.Tests
{
    public class MatchGameTests : TestContext
    {
        [Fact]
        public void GameStartsWith16Cards_AllHidden()
        {
            var cut = RenderComponent<app.Pages.MatchGame>();
            var buttons = cut.FindAll("button.card");

            Assert.Equal(16, buttons.Count);
            Assert.All(buttons, b => Assert.Equal("❓", b.TextContent));
        }

        [Fact]
        public void ClickingTwoMatchingCards_RevealsAndMarksAsMatched()
        {
            var cut = RenderComponent<app.Pages.MatchGame>();

            // Reveal internal state via reflection for test purposes
            var emojisField = cut.Instance.GetType().GetField("emojis", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var emojis = (List<object>)emojisField.GetValue(cut.Instance);

            // Get a pair of matching symbols
            var first = emojis.GroupBy(e => e.GetType().GetProperty("Symbol").GetValue(e))
                              .First(g => g.Count() == 2)
                              .ToArray();

            // Click them via index
            int index1 = emojis.IndexOf(first[0]);
            int index2 = emojis.IndexOf(first[1]);

            var buttons = cut.FindAll("button.card");
            buttons[index1].Click();
            buttons[index2].Click();

            cut.Render(); // Re-render to reflect UI changes

            Assert.NotEqual("❓", buttons[index1].TextContent);
            Assert.NotEqual("❓", buttons[index2].TextContent);
        }

        [Fact]
        public async Task ClickingTwoNonMatchingCards_HidesThemAfterDelay()
        {
            var cut = RenderComponent<app.Pages.MatchGame>();

            var emojisField = cut.Instance.GetType().GetField("emojis", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var emojis = (List<object>)emojisField.GetValue(cut.Instance);

            // Find two with different symbols
            var first = emojis[0];
            var second = emojis.First(e =>
                !e.GetType().GetProperty("Symbol").GetValue(e)
                .Equals(first.GetType().GetProperty("Symbol").GetValue(first)));

            int index1 = emojis.IndexOf(first);
            int index2 = emojis.IndexOf(second);

            var buttons = cut.FindAll("button.card");
            buttons[index1].Click();
            buttons[index2].Click();

            await Task.Delay(600); // Wait for Hide delay (500ms + margin)

            cut.Render(); // Refresh UI

            Assert.Equal("❓", buttons[index1].TextContent);
            Assert.Equal("❓", buttons[index2].TextContent);
        }

        [Fact]
        public void TimerStartsAtZero()
        {
            var cut = RenderComponent<app.Pages.MatchGame>();
            var timeText = cut.Find("p").TextContent;

            Assert.Equal("0.0s", timeText);
        }

        [Fact]
        public async Task TimerIncrementsOverTime()
        {
            var cut = RenderComponent<app.Pages.MatchGame>();

            await Task.Delay(500);
            cut.Render();

            var display = cut.Find("p").TextContent;
            Assert.NotEqual("0.0s", display);
        }

        [Fact]
        public void ClickingPlayAgainResetsGame()
        {
            var cut = RenderComponent<app.Pages.MatchGame>();

            // Simulate setting state to win condition
            var matchesField = cut.Instance.GetType().GetField("matchesFound", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            matchesField.SetValue(cut.Instance, 8);

            cut.Render();

            // Check "Play Again?" appears
            var playAgainBtn = cut.Find("button:not(.card)");
            Assert.Equal("Play Again?", playAgainBtn.TextContent);

            playAgainBtn.Click();
            cut.Render();

            var buttons = cut.FindAll("button.card");
            Assert.Equal(16, buttons.Count);
            Assert.All(buttons, b => Assert.Equal("❓", b.TextContent));
        }
    }
}
