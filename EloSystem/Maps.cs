using System;
using System.Collections.Generic;
using System.Text;

namespace EloSystem
{
    public class map
    {
        public map()
        {
            mapName = "empty";
            maps = 0;
        }

        public map(string mapname, int ms)
        {
            mapName = mapname;
            maps = ms;
        }

        public string mapName { get; set; }
        public int maps { get; set; }
    }
}
