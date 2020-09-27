using System;
using EnsoulSharp.SDK;
using Froggy;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using FrogThresh;

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
            Menu menu;
            try
            {
                menu = new Menu("FroggyAio", "FroggyAio",true);
                menu.Add(new MenuSeparator("set", "Press F5 2x To Apply"));
                menu.Add(new MenuBool("ChampionScripts", "Use Frogs Champ"));
                menu.Add(new MenuBool("SkinChanger", "Use Frogs SkinChanger"));
                menu.Attach();
                if (menu["ChampionScripts"].GetValue<MenuBool>().Enabled) switch (GameObjects.Player.CharacterName)
                {
                    case "Thresh":
                        Program.OnGameLoad();
                        break;
                    case "BlitzCrank":
                        FrogBlitzcrank.OnGameLoad();
                        break;
                    case "Riven":
                        Riven.OnGameLoad();
                        break;

                    case "Ezrail":
                        Ezrail.OnGameLoad();
                        break;
                }

                if(menu["SkinChanger"].GetValue<MenuBool>().Enabled) SkinChangers.OnLoad();

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
