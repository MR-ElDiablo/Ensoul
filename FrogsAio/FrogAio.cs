using System;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using System.Security.Permissions;

namespace ForggyAio
{
    class FrogAio
    {
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        static void OnGameLoad()
        {
            Menu menu;
            try
            {
                var load = System.Reflection.Assembly.Load(FroggyAio.Resource.FroggyLib);

                menu = new Menu("FroggyAioz", "FroggyAio", true)
                {
                    new MenuSeparator("setz", "Press F5 2x To Apply"),
                    new MenuBool("ChampionScriptsz", "Use Frogs Champ"),
                    new MenuBool("SkinChangersz", "Use Frogs SkinChanger")
                };
                menu.Attach();

                if (menu["ChampionScriptsz"].GetValue<MenuBool>().Enabled) switch (GameObjects.Player.CharacterName)
                {
                    case "Thresh":
                            var getType = load.GetType("Froggy.Program");
                            var loadTh = getType.GetMethod("OnGameLoad");
                            loadTh.Invoke(null, null);
                            break;
                    case "BlitzCrank":

                            var blitz = load.GetType("Froggy.FrogBlitzcrank");
                            var blitzOnLoad = blitz.GetMethod("OnGameLoad");
                            blitzOnLoad.Invoke(null, null);
                        break;
                    case "Riven":
                            
                            var riven = load.GetType("Froggy.Riven");
                            var rivenOnLoad = riven.GetMethod("OnGameLoad");
                            rivenOnLoad.Invoke(null, null);
                            break;
                    case "Ezreal":
                            
                            var Ezreal = load.GetType("Froggy.Ezreal");
                            var EzrealOnLoad = Ezreal.GetMethod("OnGameLoad");
                            EzrealOnLoad.Invoke(null, null);

                }

                if (menu["SkinChangersz"].GetValue<MenuBool>().Enabled)
                {

                    var kappaSkin = load.GetType("Froggy.SkinChangers");
                    var kappaLoad= kappaSkin.GetMethod("OnLoad");
                    kappaLoad.Invoke(null, null);
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
