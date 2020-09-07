using System;
using System.Collections.Generic;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;

namespace ModuleLauncher.Re.Utils
{
    public static class CollectionHelper
    {
        public static IEnumerable<MinecraftLibrariesEntity> ExcludeRepeat(IEnumerable<MinecraftLibrariesEntity> src)
        {
            var entities = src.ToList();

            for (var i = 0; i < entities.Count; i++)
            {
                if (!Version.TryParse(entities[i].UnformattedName[2], out var v1)) continue;
                for (var j = i + 1; j < entities.Count; j++)
                {
                    if (entities[i].UnformattedName[1] != entities[j].UnformattedName[1]) continue;
                    if (!Version.TryParse(entities[j].UnformattedName[2], out var v2)) continue;
                    if (v1 < v2) entities.RemoveAt(i);
                }
            }

            return entities;
        }
    }
}