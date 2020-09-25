using System;
using EnsoulSharp.SDK;
using Froggy;
namespace ForggyAio
{
    class FrogAio
    {
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        static void OnGameLoad()
        {
            try
            {
                switch (GameObjects.Player.CharacterName)
                {
                    case "Thresh":
                        Thresh.OnGameLoad();
                        break;
                    case "BlitzCrank":
                        FrogBlitzcrank.OnGameLoad();
                        break;
                    case "Riven":
                        Riven.OnGameLoad();
                        break;
                }
                SkinChanger.OnLoad();

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
