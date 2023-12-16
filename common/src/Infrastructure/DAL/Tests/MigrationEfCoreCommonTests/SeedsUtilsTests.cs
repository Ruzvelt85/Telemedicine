using System;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon.Utils;
using Xunit;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests
{
    public class SeedsUtilsTests
    {
        [Fact]
        public void SeedsWithOrdersTest()
        {
            const string? filenameWithNoPriorityPrefix = "./Seeds/TestSeedWithNoPrefix";
            const string? filenameWithFirstPriority = "./Seeds/1_TestSeed";
            const string? anotherFilenameWithFirstPriority = "./Seeds/1_AnotherTestSeed";
            const string? filenameWithSecondPriority = "./Seeds/3_TestSeed";

            var seedsFilenames = new[] { filenameWithSecondPriority, anotherFilenameWithFirstPriority, filenameWithNoPriorityPrefix, filenameWithFirstPriority };

            var orderedSeeds = SeedsUtils.GetOrderedSeeds(seedsFilenames);

            Assert.Equal(orderedSeeds[0][0], filenameWithNoPriorityPrefix);

            Assert.Equal(orderedSeeds[1][0], anotherFilenameWithFirstPriority);
            Assert.Equal(orderedSeeds[1][1], filenameWithFirstPriority);

            Assert.Equal(orderedSeeds[2][0], filenameWithSecondPriority);
        }

        [Fact]
        public void SeedWithWrongOrderPrefixTest()
        {
            const string? filenameWithNoPriorityPrefix = "./Seeds/TestSeedWithNoPrefix";
            const string? filenameWithFirstPriority = "./Seeds/1_TestSeed";
            const string? filenameWithSecondPriority = "./Seeds/2X_TestSeed";

            var seedsFilenames = new[] { filenameWithSecondPriority, filenameWithNoPriorityPrefix, filenameWithFirstPriority };

            void ExceptionCauseAction() => SeedsUtils.GetOrderedSeeds(seedsFilenames);

            Assert.Throws<Exception>(ExceptionCauseAction);
        }
    }
}
