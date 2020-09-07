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
            var e = src.ToList();

            //第一层循环
            for (var i = 0; i < e.Count; i++)
            {
                /*Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"LOOP ONE:{i}\nLib:{e[i].Name}\nVer:{e[i].UnformattedName[2]}");*/
                //尝试解析库版本，失败则continue
                if (!Version.TryParse(e[i].UnformattedName[2], out var v1)) 
                    continue;
                
                //第二层循环
                for (var j = i + 1; j < e.Count; j++)
                {
                    if (e[i].UnformattedName[1] != e[j].UnformattedName[1]) 
                        continue;
                    if (!Version.TryParse(e[j].UnformattedName[2], out var v2)) 
                        continue;
                    
                    //Console.ForegroundColor = ConsoleColor.DarkRed;
                    //Console.WriteLine($"LOOP TWO:{j}\nLib:{e[j].Name}\nVer:{e[j].UnformattedName[2]}");
                    if (v1 < v2)
                    {
                        //Console.ForegroundColor = ConsoleColor.DarkBlue;
                        //Console.WriteLine($"LOOP ONE {i} REMOVED");
                        e.RemoveAt(i);
                    }
                    /*else
                    {
                        //Console.ForegroundColor = ConsoleColor.DarkBlue;
                        //Console.WriteLine($"LOOP TWO {j} REMOVED");
                        e.RemoveAt(j);
                    }*/
                        
                }
            }

            return e;
        }
    }
}