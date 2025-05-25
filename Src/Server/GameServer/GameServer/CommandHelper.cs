using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class CommandHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                string line = Console.ReadLine().ToLower().Trim();
                try
                {
                    string[] cmd = line.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    switch (line.ToLower().Trim())
                    {
                        case "addexp":
                            AddExp(int.Parse(cmd[1]), int.Parse(cmd[2]));
                            break;
                        case "exit":
                            run = false;
                            break;
                        default:
                            Help();
                            break;
                    }
                }catch(Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        private static void AddExp(int id, int exp)
        {
            var cha = Managers.CharacterManager.Instance.GetCharacter(id);
            if (cha == null)
            {
                Console.WriteLine("CharacterID {0} not found", id);
                return;
            }
            cha.AddExp(exp);
        }

        public static void Help()
        {
            Console.Write(@"
Help:
    addexp <characterID> <exp>
    exit    Exit Game Server
    help    Show Help
");
        }
    }
}
