using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace EloSystem
{
    class Program
    {
        static string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=K:\Elo\EloSystem\EloSystem\BasedElo.mdf;Integrated Security=True";
        static List<playerStats> heldPlayerStats = new List<playerStats>();
        static List<map> heldMaps = new List<map>();
        static List<matchResults> matchResultsPrev = new List<matchResults>();
        static playerStats[] playersInGame = new playerStats[8];
        static List<playerStats> playersInGameL = new List<playerStats>();
        static team team1 = new team();
        static team team2 = new team();
        static map mapChoice = new map();
        static double team1Wins = 0;
        static double team2Wins = 0;
        static int draws = 0;




        static void Main(string[] args)
        {
            //Grabbing data from database
            grabPlayerStats();
            grabPreviousMatchStats();
            grabMapStats();

            Console.WriteLine("---------------------------- Welcome to the Elo zone ---------------------------- \n\n\nWould you like to generate random teams or enter a result? (T/R) ");
            string answer = "";
            bool correctAns = true;
            while (correctAns)
            {
                answer = Console.ReadLine();
                if (answer.ToUpper() == "R")
                {
                    grabTeams();
                    
                    Console.WriteLine("Please enter the map played: ");
                    string mapChoiceS = "";
                    bool nameCheck = true;
                    while (nameCheck)
                    {
                        mapChoiceS = Console.ReadLine();
                        bool testBool = checkMap(mapChoiceS);
                        if (testBool)
                        {
                            nameCheck = false;
                        }
                        else
                        {
                            Console.WriteLine("No record with that name, please try again: ");
                        }

                    }
                    mapChoice = heldMaps.Find(x => x.mapName.ToUpper() == mapChoiceS.ToUpper());
                    if (mapChoice.maps != 0)
                    {
                        Console.WriteLine("Please enter the rounds won by Team 1 between 1 - " + Convert.ToString(mapChoice.maps) + ": ");
                        team1Wins = Convert.ToDouble(Console.ReadLine());
                    }
                    else
                    {
                        Console.WriteLine("Fatal error");
                        //Add other path that accounts for this big. Not important at all
                    }
                    Console.WriteLine("Please enter the rounds drawn, between 0 - " + Convert.ToString(mapChoice.maps - team1Wins) + ": ");
                    draws = Convert.ToInt32(Console.ReadLine());
                    team2Wins = mapChoice.maps - team1Wins - draws;
                    for (int t = 0; t < draws; t++)
                    {
                        team1Wins = team1Wins + 0.5;
                        team2Wins = team2Wins + 0.5;
                    }
                    calculateAndApplyEloChanges();
                    matchResults gameResults = new matchResults(mapChoice.mapName, team1.player1, team1.player2, team1.player3, team1.player4, team2.player1, team2.player2, team2.player3, team2.player4
                        , team1Wins, team2Wins, DateTime.Now);
                    commitMatchResult(gameResults);
                    correctAns = false;
                }
                else if (answer.ToUpper() == "T")
                {
                    grabPlayers();
                    makeTeams();
                    correctAns = false;
                }
                else
                {
                    Console.WriteLine("Please enter T or R");
                    correctAns = true;
                }
            }


        }

        static bool checkName(string playerName)
        {
            bool correctName = false;
            for(int t = 0; t < heldPlayerStats.Count; t++)
            {
                if(correctName != true)
                {
                    if(heldPlayerStats[t].playerName.ToUpper() == playerName.ToUpper())
                    {
                        correctName = true;
                    }
                }

            }
                return correctName;
        }

        static bool checkMap(string mapName)
        {
            bool correctName = false;
            for (int t = 0; t < heldMaps.Count; t++)
            {
                if (correctName != true)
                {
                    if (heldMaps[t].mapName.ToUpper() == mapName.ToUpper())
                    {
                        correctName = true;
                    }
                }

            }
            return correctName;
        }

        static void getName(string nameToFill)
        {
            bool nameCheck = true;
            while (nameCheck)
            {
                nameToFill = Console.ReadLine();
                bool testBool = checkName(nameToFill);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            //Add the ability to display player list in this section if the user enteres # or smth
        }

        static void getMapName(string nameToFill)
        {
            bool nameCheck = true;
            while (nameCheck)
            {
                nameToFill = Console.ReadLine();
                bool testBool = checkMap(nameToFill);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            //Add the ability to display map list in this section if the user enteres ' or smth
        }

        static void instansiateStatsArray(playerStats[] instAll)
        {
            instAll[0] = new playerStats();
            instAll[1] = new playerStats();
            instAll[2] = new playerStats();
            instAll[3] = new playerStats();
            instAll[4] = new playerStats();
            instAll[5] = new playerStats();
            instAll[6] = new playerStats();
            instAll[7] = new playerStats();
        }

        static void calculateAndSetVolatility(playerStats player)
        {
            int gamesPlayed = player.matchesPlayed;
            player.volatility = 1.25 * (1 / (0.2 * Convert.ToDouble(gamesPlayed) + 1)) + 0.3;
        }

        static void grabPlayerStats()
        {
            using (SqlConnection eloCon = new SqlConnection(conStr))
            {
                string queryStr = "Select * FROM PlayerStats";
                SqlCommand cmd = new SqlCommand(queryStr, eloCon);

                eloCon.Open();
                using (SqlDataReader oReader = cmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        playerStats tempStats = new playerStats(Convert.ToString(oReader[1]), Convert.ToInt32(oReader[2]), 0.0);
                        heldPlayerStats.Add(tempStats);
                    }
                }
            }
            instansiateStatsArray(playersInGame);
        }

        static void grabMapStats()
        {
            using (SqlConnection eloCon = new SqlConnection(conStr))
            {
                string queryStr = "Select * FROM Maps";
                SqlCommand cmd = new SqlCommand(queryStr, eloCon);

                eloCon.Open();
                using (SqlDataReader oReader = cmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        map tempMap = new map(Convert.ToString(oReader[1]), Convert.ToInt32(oReader[2]));
                        heldMaps.Add(tempMap);
                    }
                }
            }
        }

        static void grabPreviousMatchStats()
        {
            using (SqlConnection eloCon = new SqlConnection(conStr))
            {
                string queryStr = "Select * FROM MatchResults WHERE DateOfMatch >= DATEADD(M, -3, GETDATE()) ";
                SqlCommand cmd = new SqlCommand(queryStr, eloCon);

                eloCon.Open();
                using (SqlDataReader oReader = cmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        matchResults tempMatch = new matchResults();
                        tempMatch = tempMatch.matchResultsTeamInput(Convert.ToString(oReader[1]), team1, team2, Convert.ToInt32(oReader[3]), 
                                Convert.ToInt32(oReader[4]), Convert.ToDateTime(oReader[5]));
                        matchResultsPrev.Add(tempMatch);
                    }
                }
            }
        }
        static void findMatchesPlayed(playerStats player)
        {
            using (SqlConnection eloCon = new SqlConnection(conStr))
            {
                string queryStr = "Select COUNT(idMatchResults) FROM MatchResults WHERE DateOfMatch >= DATEADD(M, -3, GETDATE()) AND" +
                    "(team1player1 = '" + player.playerName + "' OR " +
                    "team1player2 = '" + player.playerName + "' OR " +
                    "team1player3 = '" + player.playerName + "' OR " +
                    "team1player4 = '" + player.playerName + "' OR " +
                    "team2player1 = '" + player.playerName + "' OR " +
                    "team2player2 = '" + player.playerName + "' OR " +
                    "team2player3 = '" + player.playerName + "' OR " +
                    "team2player4 = '" + player.playerName + "')";
                SqlCommand cmd = new SqlCommand(queryStr, eloCon);

                eloCon.Open();
                using (SqlDataReader oReader = cmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        player.matchesPlayed = Convert.ToInt32(oReader[0]);
                    }
                }
            }
        }

        static double calculateTeamPerformaceRating(double eteamElo, double roundsWon, double roundsLost)
        {
            double performanceRating = Math.Floor(Convert.ToDouble(eteamElo + (200 * (roundsWon - roundsLost))));
            return performanceRating;
        }

        static double calculateEloChange(playerStats player,int TeamPerformanceRating, bool team1B)
        {
            double eloChange = 0;
            int dp = TeamPerformanceRating - player.playerElo;
            double calculatedExpectedScore = calculateExpectedScore(team1, team2);
            double expectedMapWinsTeam1 = mapChoice.maps * calculatedExpectedScore;
            double expectedMapWinsTeam2 = mapChoice.maps * (1-calculatedExpectedScore);

            if (team1B) 
            {
                eloChange = dp * player.volatility * (Math.Abs((team1Wins - expectedMapWinsTeam1)) /(2 * mapChoice.maps));
            }
            else
            {
                eloChange = dp * player.volatility * (Math.Abs(team2Wins - expectedMapWinsTeam2) / (2 * mapChoice.maps));
            }

            return eloChange;
        }

        static double calculateExpectedScore(team team1, team team2)
        {

            double expectedScore = 0.0;

            expectedScore = 1 / (1 + Math.Pow(10, (-(team1.calculateTeamEloAvg() - team2.calculateTeamEloAvg()) / 400)));

            return expectedScore;
        }


        static void calculateAndApplyEloChanges()
        {
            int team1Perf = Convert.ToInt32(Math.Ceiling(calculateTeamPerformaceRating(team2.calculateTeamEloAvg(), team1Wins, team2Wins)));
            int team2Perf = Convert.ToInt32(Math.Ceiling(calculateTeamPerformaceRating(team1.calculateTeamEloAvg(), team2Wins, team1Wins)));
            double player1EloChange = calculateEloChange(team1.player1, team1Perf, true);
            double player2EloChange = calculateEloChange(team1.player2, team1Perf, true);
            double player3EloChange = calculateEloChange(team1.player3, team1Perf, true);
            double player4EloChange = calculateEloChange(team1.player4, team1Perf, true);
            double player5EloChange = calculateEloChange(team2.player1, team2Perf, false);
            double player6EloChange = calculateEloChange(team2.player2, team2Perf, false);
            double player7EloChange = calculateEloChange(team2.player3, team2Perf, false);
            double player8EloChange = calculateEloChange(team2.player4, team2Perf, false);

            applyEloChange(player1EloChange, team1.player1);
            applyEloChange(player2EloChange, team1.player2);
            applyEloChange(player3EloChange, team1.player3);
            applyEloChange(player4EloChange, team1.player4);
            applyEloChange(player5EloChange, team2.player1);
            applyEloChange(player6EloChange, team2.player2);
            applyEloChange(player7EloChange, team2.player3);
            applyEloChange(player8EloChange, team2.player4);

            File.AppendAllText( @"K:\\Elo\\EloSystem\\EloSystem\\logs.txt", team1.player1.playerName + "Change = " + Convert.ToString(player1EloChange) + ", " +
                    team1.player2.playerName + "Change = " + Convert.ToString(player2EloChange) + ", " +
                    team1.player3.playerName + "Change = " + Convert.ToString(player3EloChange) + ", " +
                    team1.player4.playerName + "Change = " + Convert.ToString(player4EloChange) + ", " +
                    team2.player1.playerName + "Change = " + Convert.ToString(player5EloChange) + ", " +
                    team2.player2.playerName + "Change = " + Convert.ToString(player6EloChange) + ", " +
                    team2.player3.playerName + "Change = " + Convert.ToString(player7EloChange) + ", " +
                    team2.player4.playerName + "Change = " + Convert.ToString(player8EloChange) + ", " +
                    "Team1Perf = " + Convert.ToString(team1Perf) + ", " +
                    "Team2Perf = " + Convert.ToString(team2Perf) + ", " +
                    "DateOfMatch = " + Convert.ToString(DateTime.Now) + Environment.NewLine);

            File.AppendAllText(@"K:\\Elo\\EloSystem\\EloSystem\\aaronPython.txt", Environment.NewLine + team1.player1.playerName + "," + Convert.ToString(player1EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team1.player2.playerName + "," + Convert.ToString(player2EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team1.player3.playerName + "," + Convert.ToString(player3EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team1.player4.playerName + "," + Convert.ToString(player4EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team2.player1.playerName + "," + Convert.ToString(player5EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team2.player2.playerName + "," + Convert.ToString(player6EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team2.player3.playerName + "," + Convert.ToString(player7EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                + Environment.NewLine + team2.player4.playerName + "," + Convert.ToString(player8EloChange) + "," + Convert.ToString(DateTime.Now).Replace(" ", "")
                );

        }

        static void applyEloChange(double eloChange, playerStats player)
        {

            using (SqlConnection eloCon = new SqlConnection(conStr))
            {
                eloCon.Open();
                string queryStr = "UPDATE PlayerStats SET PlayerElo = " + Convert.ToString(Math.Ceiling(Convert.ToDouble(player.playerElo) + eloChange)) + " WHERE PlayerName = '" + player.playerName + "'";
                SqlCommand cmd = new SqlCommand(queryStr, eloCon);
                cmd.ExecuteNonQuery();
            }
        }
        static void commitMatchResult(matchResults matchResultsToCommit)
        {
            using (SqlConnection eloCon = new SqlConnection(conStr))
            {
                eloCon.Open();
                string queryStr = "INSERT INTO MatchResults (MapName, Team1Wins, Team2Wins, Draws, DateOfMatch, team1player1, team1player2" +
                    ", team1player3, team1player4, team2player1, team2player2, team2player3, team2player4) VALUES ('" +
                    mapChoice.mapName + "','" + Convert.ToString(team1Wins) + "','" + Convert.ToString(team2Wins) + "','" + Convert.ToString(draws) + "','" +
                    Convert.ToString(DateTime.Now) + "','" + team1.player1.playerName + "','" + team1.player2.playerName + "','" + team1.player3.playerName + "','" +
                    team1.player4.playerName + "','" + team2.player1.playerName + "','" + team2.player2.playerName + "','" + team2.player3.playerName + "','" +
                    team2.player4.playerName + "')";
                SqlCommand cmd = new SqlCommand(queryStr, eloCon);
                cmd.ExecuteNonQuery();
            }
        }

        

        static void calcPlayerStats()
        {
            findMatchesPlayed(team1.player1);
            findMatchesPlayed(team1.player2);
            findMatchesPlayed(team1.player3);
            findMatchesPlayed(team1.player4);
            findMatchesPlayed(team2.player1);
            findMatchesPlayed(team2.player2);
            findMatchesPlayed(team2.player3);
            findMatchesPlayed(team2.player4);

            calculateAndSetVolatility(team1.player1);
            calculateAndSetVolatility(team1.player2);
            calculateAndSetVolatility(team1.player3);
            calculateAndSetVolatility(team1.player4);
            calculateAndSetVolatility(team2.player1);
            calculateAndSetVolatility(team2.player2);
            calculateAndSetVolatility(team2.player3);
            calculateAndSetVolatility(team2.player4);

        }



        static void simulateGames(int numGames)
        {
            for (int i = 0; i < numGames; i++)
            {
                Random rand = new Random();
                int player1id = rand.Next(0, 25);
                int player2id = 0;
                int player3id = 0;
                int player4id = 0;
                int player5id = 0;
                int player6id = 0;
                int player7id = 0;
                int player8id = 0;

                int[] bannedId = new int[8];
                bannedId[0] = player1id;
                bool idCheck = true;
                while (idCheck)
                {
                    player2id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player2id);

                    if(pos > -1)
                    {
                        continue;
                    }
                    bannedId[1] = player2id;
                    idCheck = false;
                }
                idCheck = true;
                while (idCheck)
                {
                    player3id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player3id);

                    if (pos > -1)
                    {
                        continue;
                    }
                    bannedId[2] = player3id;
                    idCheck = false;
                }
                idCheck = true;

                while (idCheck)
                {
                    player4id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player4id);

                    if (pos > -1)
                    {
                        continue;
                    }
                    bannedId[3] = player4id;
                    idCheck = false;
                }
                idCheck = true;

                while (idCheck)
                {
                    player5id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player5id);

                    if (pos > -1)
                    {
                        continue;
                    }
                    bannedId[4] = player5id;
                    idCheck = false;
                }
                idCheck = true;

                while (idCheck)
                {
                    player6id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player6id);

                    if (pos > -1)
                    {
                        continue;
                    }
                    bannedId[5] = player6id;
                    idCheck = false;
                }
                idCheck = true;
                while (idCheck)
                {
                    player7id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player7id);

                    if (pos > -1)
                    {
                        continue;
                    }
                    bannedId[6] = player7id;
                    idCheck = false;
                }
                idCheck = true;

                while (idCheck)
                {
                    player8id = rand.Next(0, 25);
                    int pos = Array.IndexOf(bannedId, player8id);

                    if (pos > -1)
                    {
                        continue;
                    }
                    bannedId[7] = player8id;
                    idCheck = false;
                }

                team1.player1 = heldPlayerStats[player1id];
                team1.player2 = heldPlayerStats[player2id];
                team1.player3 = heldPlayerStats[player3id];
                team1.player4 = heldPlayerStats[player4id];
                team2.player1 = heldPlayerStats[player5id];
                team2.player2 = heldPlayerStats[player6id];
                team2.player3 = heldPlayerStats[player7id];
                team2.player4 = heldPlayerStats[player8id];


                mapChoice = heldMaps[rand.Next(0, 20)];

                team1Wins = rand.Next(0, mapChoice.maps);
                draws = rand.Next(0, Convert.ToInt32(mapChoice.maps - team1Wins));
                team2Wins = mapChoice.maps - team1Wins - draws;

                calcPlayerStats();

                for (int t = 0; t < draws; t++)
                {
                    team1Wins = team1Wins + 0.5;
                    team2Wins = team2Wins + 0.5;
                }
                calculateAndApplyEloChanges();
                matchResults gameResults = new matchResults(mapChoice.mapName, team1.player1, team1.player2, team1.player3, team1.player4, team2.player1, team2.player2, team2.player3, team2.player4
                    , team1Wins, team2Wins, DateTime.Now);
                commitMatchResult(gameResults);

                team1Wins = 0;
                team2Wins = 0;
                draws = 0;
            }
        }

        static void makeTeams()
        {
            playersInGameL.Sort((x, y) => x.playerElo.CompareTo(y.playerElo));
            playersInGameL.Reverse();
            team1.player1 = playersInGameL[0];
            team1.player2 = playersInGameL[3];
            team1.player3 = playersInGameL[5];
            team1.player4 = playersInGameL[7];

            team2.player1 = playersInGameL[1];
            team2.player2 = playersInGameL[2];
            team2.player3 = playersInGameL[4];
            team2.player4 = playersInGameL[6];

            Console.WriteLine("Alternate sort: \n\tTeam 1:\n\t\t-{0}\n\t\t-{1}\n\t\t-{2}\n\t\t-{3}\n\n\t\tAverage Elo = {8}\n\tTeam 2:\n\t\t-{4}\n\t\t-{5}\n\t\t-{6}\n\t\t-{7}\n\n\t\tAverage Elo = {9}",
                team1.player1.playerName, team1.player2.playerName, team1.player3.playerName, team1.player4.playerName,
                team2.player1.playerName, team2.player2.playerName, team2.player3.playerName, team2.player4.playerName,
                Convert.ToString(team1.calculateTeamEloAvg()), Convert.ToString(team2.calculateTeamEloAvg()));
        }

        static void grabTeams()
        {
            bool nameCheck = true;
            Console.WriteLine("Please enter 1st player's name in Team 1: ");
            while (nameCheck)
            {
                playersInGame[0].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[0].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 2nd player's name in Team 1: ");
            while (nameCheck)
            {
                playersInGame[1].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[1].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 3rd player name in Team 1: ");
            while (nameCheck)
            {
                playersInGame[2].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[2].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 4th player name in Team 1: ");
            while (nameCheck)
            {
                playersInGame[3].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[3].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 1st player name in Team 2: ");
            while (nameCheck)
            {
                playersInGame[4].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[4].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 2nd player name in Team 2: ");
            while (nameCheck)
            {
                playersInGame[5].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[5].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 3rd player name in Team 2: ");
            while (nameCheck)
            {
                playersInGame[6].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[6].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }
            nameCheck = true;
            Console.WriteLine("Please enter 4th player name in Team 2: ");
            while (nameCheck)
            {
                playersInGame[7].playerName = Console.ReadLine();
                bool testBool = checkName(playersInGame[7].playerName);
                if (testBool)
                {
                    nameCheck = false;
                }
                else
                {
                    Console.WriteLine("No record with that name, please try again: ");
                }

            }

            team1.player1 = heldPlayerStats.Find(x => x.playerName == playersInGame[0].playerName);
            playersInGameL.Add(team1.player1);
            team1.player2 = heldPlayerStats.Find(x => x.playerName == playersInGame[1].playerName);
            playersInGameL.Add(team1.player2);
            team1.player3 = heldPlayerStats.Find(x => x.playerName == playersInGame[2].playerName);
            playersInGameL.Add(team1.player3);
            team1.player4 = heldPlayerStats.Find(x => x.playerName == playersInGame[3].playerName);
            playersInGameL.Add(team1.player4);
            team2.player1 = heldPlayerStats.Find(x => x.playerName == playersInGame[4].playerName);
            playersInGameL.Add(team2.player1);
            team2.player2 = heldPlayerStats.Find(x => x.playerName == playersInGame[5].playerName);
            playersInGameL.Add(team2.player2);
            team2.player3 = heldPlayerStats.Find(x => x.playerName == playersInGame[6].playerName);
            playersInGameL.Add(team2.player3);
            team2.player4 = heldPlayerStats.Find(x => x.playerName == playersInGame[7].playerName);
            playersInGameL.Add(team2.player4);
            calcPlayerStats();
        }

        static void grabPlayers()
        {
            for(int i = 0; i < 8; i++)
            {
                bool nameCheck = true;
                Console.WriteLine("Please enter next player: ");
                while (nameCheck)
                {
                    string tempName = Console.ReadLine();
                    bool testBool = checkName(tempName);
                    if (testBool)
                    {
                        nameCheck = false;
                        playersInGameL.Add(heldPlayerStats.Find(x => x.playerName == tempName));
                    }
                    else
                    {
                        Console.WriteLine("No record with that name, please try again: ");
                    }
                }
            }
        }

        static void averageEloCalculator()
        {
            playerStats[,] pairs = new playerStats[4, 2];

            for(int i = 0; i < 8; i++)
            {
                bool belowLarger = false;
                if(i-1 > 0)
                {

                }
            }
        }

        /*
        public static  (team, team) teamCreation(List<playerStats> players)
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
                    foreach (int o in others.GetRange(1, others.Count)
                    {
                        if (row[o] < row[min])
                        {
                            min = o;
                        }
                    }
                    mins.Add(List<int>.IndexOf(eloDiff, row), min);
                }
                foreach (int p in mins)
                {
                    if (!((pairs.Contains((p, mins[p]))) or(pairs.Contains((mins[p], p))) ) )
				{
                        if ((mins.Contains(mins[p])) and(mins[mins[p]] == p))
					{
                            pairs.Add((p, mins[p]));
                            left.Remove(p);
                            left.Remove(mins[p]);
                        }
                    }
                }
                if (previous == left.Length)
                {
                    // IF THIS IS REACHED, SHIT BROKE
                }
            }
            List<int> team1 = new List<int>;
            List<int> team2 = new List<int>;
            foreach ((int one, int two) in pairs)
            {
                team1.Add(one);
                team2.Add(two);
            }
            return (team1, team2);
        }
        */

    }
}



/*
 * If players closest elo is also the closest elo players closest elo player then they are a pair
 *Pairs are not on same team
 * If players have no pair, place the higher of the 'pair' on the currently lower rated team
 * 
 * 
 */
