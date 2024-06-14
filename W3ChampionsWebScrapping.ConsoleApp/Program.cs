using System;
using System.Collections.Generic;
using System.Threading;
using WebScrapping.Models;
public class Program
{
    public static void Main(string[] args)
    {
        var players = new List<Player>()
        {
            new Player(231357, "DoNotWinMe"),
            new Player(2311750, "BlOoDy"),
            new Player(231778, "KodoGuachi"),
            new Player(2313178, "Fenix"),
            new Player(231437, "MoonAndino"),
            new Player(232365, "DanGer"),
            new Player(231153826, "HiddenPants"),
            new Player(231234, "JperezImba")
        };

        /*
        var getData = WebScrapping.Test.W3Champions.First(players);

        for (int i = 0; i < getData.Count; i++)
        {
            var key = players[i].Name;
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"- {players[i]} {getData[key].Count} -");
            Console.WriteLine("-----------------------------------");
            foreach (var data in getData[key])
            {
                Console.WriteLine(data);
            }
        }
        */

        players = new List<Player>()
        {
            new Player(231357, "DoNotWinMe"),
            new Player(2311750, "BlOoDy"),
        };

        var getTable = WebScrapping.Test.W3Champions.GetData1vs1(players);

        Console.WriteLine(getTable);
    }
}