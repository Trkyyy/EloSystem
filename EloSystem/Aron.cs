using System;
using System.Collections.Generic;
using System.Text;

namespace EloSystem
{
    class Aron
    {
        public static (team, team) teamCreation(List<playerStats> players)
        {
            int n = players.Count;
            List<List<int>> eloDiff = new List<List<int>>();
            for (int a = 0; a < n; a++)
            {
                for (int b = a + 1; b < n; b++)
                {
                    eloDiff[a][b] = Math.Abs(players[a].playerElo - players[b].playerElo);
                }
            }
            List<(int, int)> pairs = new List<(int, int)>();
            List<int> left = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            while (left.Count > 0)
            {
                int previous = left.Count;
                Dictionary<int, int> mins = new Dictionary<int, int>();
                foreach (int index in left)
                {
                    List<int> row = eloDiff[index];
                    List<int> others = left;
                    others.Remove(index);
                    int min = others[0];
                    foreach (int o in others.GetRange(1, others.Count))
                    {
                        if (row[o] < row[min])
                        {
                            min = o;
                        }
                    }
                    mins.Add(eloDiff.IndexOf(row), min);
                }

                foreach (int p in mins)
                {
                    Boolean inPairs = pairs.Contains((p, mins[p])) | pairs.Contains((mins[p], p));
                    if (!inPairs)
                    {
                        Boolean inMins = mins.ContainsKey(mins[p]) & mins[mins[p]] == p;
                        if (inMins)
                        {
                            pairs.Add((p, mins[p]));
                            left.Remove(p);
                            left.Remove(mins[p]);
                        }
                    }
                }
                if (previous == left.Count)
                {
                    Console.WriteLine("Team creation broke because pairs are not able to be made with this setup");
                }
            }
            List<int> team1 = new List<int>();
            List<int> team2 = new List<int>();
            foreach ((int one, int two) in pairs)
            {
                team1.Add(one);
                team2.Add(two);
                // TODO: random team assignment + intelligent team assignment
            }
            return (team1, team2);
        }

    }
}
