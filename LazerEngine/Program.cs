using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var Game = new LazerGameCore())
                Game.Run();
        }
    }
}
