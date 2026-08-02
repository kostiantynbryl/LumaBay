using NUnit.Framework;

namespace LumaBay.Tests
{
    public sealed class CampaignProgressionTests
    {
        [Test]
        public void CampaignContainsSixtyLevels()
        {
            Assert.AreEqual(60, LevelCatalog.Count);
            for (int id = 1; id <= LevelCatalog.Count; id++)
            {
                LevelDefinition level = LevelCatalog.Get(id);
                Assert.AreEqual(id, level.Id);
                Assert.GreaterOrEqual(level.Moves, 18);
                Assert.Greater(level.TargetCount, 0);
                Assert.LessOrEqual(level.FogCount + level.CrateCount + level.IceCount + level.NetCount, 30);
            }
        }

        [Test]
        public void LighthouseRoadmapContainsFortyEightOrderedTasks()
        {
            Assert.AreEqual(48, LighthouseTaskCatalog.Count);
            int previousLevel = 0;
            int previousVisual = 0;
            int totalStarCost = 0;

            for (int index = 0; index < LighthouseTaskCatalog.Count; index++)
            {
                LighthouseTask task = LighthouseTaskCatalog.Get(index);
                Assert.NotNull(task);
                Assert.AreEqual(index + 1, task.Id);
                Assert.GreaterOrEqual(task.UnlockLevel, previousLevel);
                Assert.LessOrEqual(task.UnlockLevel, LevelCatalog.Count);
                Assert.That(task.StarCost, Is.InRange(1, 3));
                Assert.GreaterOrEqual(task.VisualState, previousVisual);
                Assert.That(task.VisualState, Is.InRange(0, 31));
                Assert.IsNotEmpty(task.TitleRu);
                Assert.IsNotEmpty(task.TitleEn);
                Assert.IsNotEmpty(task.DescriptionRu);
                Assert.IsNotEmpty(task.DescriptionEn);

                previousLevel = task.UnlockLevel;
                previousVisual = task.VisualState;
                totalStarCost += task.StarCost;
            }

            Assert.LessOrEqual(totalStarCost, LevelCatalog.Count * 3);
        }

        [Test]
        public void LighthouseFirstLaunchHappensDuringEarlyCampaign()
        {
            // Task 31 commissions the lighthouse; it must provide a major payoff
            // before the player reaches the final third of the 60-level campaign.
            LighthouseTask launchTask = LighthouseTaskCatalog.Get(30);
            Assert.That(launchTask.UnlockLevel, Is.InRange(30, 45));
            Assert.That(launchTask.TitleEn, Does.Contain("lighthouse").IgnoreCase);
        }

        [Test]
        public void VisualProgressReachesFinalState()
        {
            Assert.AreEqual(0, LighthouseTaskCatalog.VisualState(0));
            Assert.AreEqual(31, LighthouseTaskCatalog.VisualState(LighthouseTaskCatalog.Count));
            Assert.AreEqual(1f, LighthouseTaskCatalog.Progress01(LighthouseTaskCatalog.Count), 0.0001f);
        }
    }
}
