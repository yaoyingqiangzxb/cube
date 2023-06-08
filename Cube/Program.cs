using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube
{
    class Program
    {
        static void Main(string[] args)
        {
            MagicCube magicCube = new MagicCube(3);

            Console.Write(magicCube.ToString());
            magicCube.RotateFace(CubeFace.Upper);
            Console.WriteLine("------------------------------------");
            Console.Write(magicCube.ToString());

            Console.ReadLine();
        }
    }
}
