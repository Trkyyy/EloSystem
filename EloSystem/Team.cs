using System;
using System.Collections.Generic;
using System.Text;

namespace EloSystem
{
    public class team
    {
        public team()
        {
            player1 = new playerStats();
            player2 = new playerStats();
            player3 = new playerStats();
            player4 = new playerStats();
        }

        public team(playerStats player11, playerStats player22, playerStats player33, playerStats player44)
        {
            player1 = player11;
            player2 = player22;
            player3 = player33;
            player4 = player44;
        }


        public bool findPlayer(playerStats playerToFind)
        {
            bool found = false;

            if (player1.playerName == playerToFind.playerName)
            {
                found = true;
            }
            else if (player2.playerName == playerToFind.playerName)
            {
                found = true;
            }
            else if (player3.playerName == playerToFind.playerName)
            {
                found = true;
            }
            else if (player4.playerName == playerToFind.playerName)
            {
                found = true;
            }

            return found;
        }

        public bool sameTeam(team teamToCheck)
        {
            return teamToCheck.findPlayer(player1) && teamToCheck.findPlayer(player2) && teamToCheck.findPlayer(player3) && teamToCheck.findPlayer(player4);
        }

        public double calculateTeamEloAvg()
        {
            double teamElo = 0;
            teamElo = teamElo + player1.playerElo;
            teamElo = teamElo + player2.playerElo;
            teamElo = teamElo + player3.playerElo;
            teamElo = teamElo + player4.playerElo;
            return Convert.ToDouble(teamElo / 4);
        }

        public playerStats player1 { get; set; }
        public playerStats player2 { get; set; }
        public playerStats player3 { get; set; }
        public playerStats player4 { get; set; }
    }
}
