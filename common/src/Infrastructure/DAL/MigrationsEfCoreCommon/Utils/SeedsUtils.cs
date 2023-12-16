using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests")]
namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon.Utils
{
    internal static class SeedsUtils
    {
        public static List<List<string>> GetOrderedSeeds(IEnumerable<string> seedsFilenames)
        {
            var seedsByOrder = new SortedDictionary<int, List<string>>();

            foreach (var seedFilename in seedsFilenames)
            {
                var parts = Path.GetFileName(seedFilename).Split("_");
                // if there is no order prefix in file name then order is first
                if (parts.Length < 2)
                {
                    AddToSeeds(int.MinValue, seedFilename);
                    continue;
                }

                if (!int.TryParse(parts[0], out var order))
                { throw new Exception($"Wrong order prefix in '{seedFilename}'. Order prefix must be integer."); }

                AddToSeeds(order, seedFilename);
            }

            return seedsByOrder.Values.ToList();

            void AddToSeeds(int key, string value)
            {
                if (!seedsByOrder.TryGetValue(key, out var seeds))
                {
                    seedsByOrder[key] = new List<string> { value };
                    return;
                }

                seeds.Add(value);
            }
        }
    }
}
