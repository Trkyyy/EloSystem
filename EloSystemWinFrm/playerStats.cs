using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;


namespace EloSystem
{
    public class playerStats
    {
        public playerStats()
        {
            playerName = "empty";
            playerElo = 1000;
            volatility = 1.5;
            matchesPlayed = 0;
        }

        public playerStats(string playername, int playerelo, double vol)
        {
            playerName = playername;
            playerElo = playerelo;
            volatility = vol;
            matchesPlayed = 0;
        }

        public string playerName { get; set; }
        public int playerElo { get; set; }
        public int matchesPlayed { get; set; }
        public double volatility { get; set; }

    }
}
