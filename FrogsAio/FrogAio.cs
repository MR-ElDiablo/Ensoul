using System;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using System.Security.Permissions;
using EnsoulSharp;

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
                menu = new Menu("FroggyAio", "FroggyAio", true)
                {
                    new MenuSeparator("setz", "Press F5 2x To Apply"),
                    new MenuBool("FroggyChamps", "Use Frogs Champ"),
                    new MenuBool("FroggySkin", "Use Frogs SkinChanger")
                };
                menu.Attach();
                if (menu["FroggyChamps"].GetValue<MenuBool>().Enabled) switch (GameObjects.Player.CharacterName)
                {
                    case "Thresh":
                            var getType = load.GetType("Froggy.Program");
                            var loadTh = getType.GetMethod("OnGameLoad");
                            loadTh.Invoke(null, null);
                            break;
                    case "Blitzcrank":
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

                            var ezreal = load.GetType("Froggy.Ezreal");
                            var ezrealLoad = ezreal.GetMethod("OnGameLoad");
                            ezrealLoad.Invoke(null, null);
                            break;
                    case "Jhin":
                            var jhin = load.GetType("Froggy.Jhin");
                            var jhinLoad = jhin.GetMethod("OnGameLoad");
                            jhinLoad.Invoke(null, null);
                            break;

                    }
                if (menu["FroggySkin"].GetValue<MenuBool>().Enabled)
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
