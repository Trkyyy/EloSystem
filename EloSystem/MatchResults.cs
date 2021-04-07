using System;
using System.Collections.Generic;
using System.Text;

namespace EloSystem
{
    public class matchResults
    {
        public matchResults()
        {
            mapName = "empty";
            team1Player1 = new playerStats();
            team1Player2 = new playerStats();
            team1Player3 = new playerStats();
            team1Player4 = new playerStats();
            team2Player1 = new playerStats();
            team2Player2 = new playerStats();
            team2Player3 = new playerStats();
            team2Player4 = new playerStats();
            team1Wins = 0;
            team2Wins = 0;
            dateOfMatch = new DateTime();
            draws = 0;
        }

        public matchResults(string mapname, playerStats t1p1, playerStats t1p2, playerStats t1p3, playerStats t1p4, playerStats t2p1, playerStats t2p2, 
            playerStats t2p3, playerStats t2p4, double team1wins, double team2wins, DateTime dateMatch)
        {
            mapName = mapname;
            team1Player1 = t1p1;
            team1Player2 = t1p2;
            team1Player3 = t1p3;
            team1Player4 = t1p4;
            team2Player1 = t2p1;
            team2Player2 = t2p2;
            team2Player3 = t2p3;
            team2Player4 = t2p4;
            team1Wins = team1wins;
            team2Wins = team2wins;
            dateOfMatch = dateMatch;
            draws = 0;
        }

        public matchResults matchResultsNoDT(string mapname, playerStats t1p1, playerStats t1p2, playerStats t1p3, playerStats t1p4, playerStats t2p1, playerStats t2p2,
            playerStats t2p3, playerStats t2p4, double team1wins, double team2wins)
        {
            mapName = mapname;
            team1Player1 = t1p1;
            team1Player2 = t1p2;
            team1Player3 = t1p3;
            team1Player4 = t1p4;
            team2Player1 = t2p1;
            team2Player2 = t2p2;
            team2Player3 = t2p3;
            team2Player4 = t2p4;
            team1Wins = team1wins;
            team2Wins = team2wins;
            dateOfMatch = DateTime.Now;
            draws = 0;

            return this;
        }

        public matchResults matchResultsTeamInput(string mapname, team t1, team t2, double team1wins, double team2wins, DateTime dateMatch)
        {
            this.mapName = mapname;
            this.team1Player1 = t1.player1;
            this.team1Player2 = t1.player2;
            this.team1Player3 = t1.player3;
            this.team1Player4 = t1.player4;
            this.team2Player1 = t2.player1;
            this.team2Player2 = t2.player2;
            this.team2Player3 = t2.player3;
            this.team2Player4 = t2.player4;
            this.team1Wins = team1wins;
            this.team2Wins = team2wins;
            this.dateOfMatch = dateMatch;

            return this;
        }

        public string mapName { get; set; }
        public playerStats team1Player1 { get; set; }
        public playerStats team1Player2 { get; set; }
        public playerStats team1Player3 { get; set; }
        public playerStats team1Player4 { get; set; }
        public playerStats team2Player1 { get; set; }
        public playerStats team2Player2 { get; set; }
        public playerStats team2Player3 { get; set; }
        public playerStats team2Player4 { get; set; }

        public double team1Wins { get; set; }
        public double team2Wins { get; set; }
        public int draws { get; set; }

        public DateTime dateOfMatch { get; set; }
    }
}
