﻿using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

namespace Riven
{
    class RivemMenu
    {
        public class Combo
        {
            public static MenuBool Q, W, E, R;
            public static MenuSlider Delay,TherdQDelay;
            public static void AddToMainMenu(Menu MainMenu)
            {

                var menu = new Menu("Combo", "Combo");
                Q = new MenuBool("Q", "Q");
                W = new MenuBool("W", "W");
                E = new MenuBool("E", "E");
                R = new MenuBool("R", "R");
                Delay = new MenuSlider("Delay","Delay",0,0,1000);
                TherdQDelay= new MenuSlider("Therd Q Delay", "Therd Q Delay", 0, 0, 1000);
                menu.Add(Q);
                menu.Add(W);
                menu.Add(E);
                menu.Add(R);
                menu.Add(Delay);
                menu.Add(TherdQDelay);
                MainMenu.Add(menu);
            }/*
            private static void OnMenuLoad()
        {
            menu = new RivemMenu("DRiven", "Diablos Riven", true);
            menu.Add(new MenuSeparator("RivenV", "v1.0"));

            combo = new RivemMenu("combo", "combo");
            combo.Add(new MenuSeparator("CQS", "Q Settings"));
            combo.Add(new MenuBool("CQ", "Use Q"));
            combo.Add(new MenuList("CQPos", "Q On ", new string[] { "Target", "Target Position", "Mouse Position" }));
            combo.Add(new MenuBool("CQG", "Use Q To GapClose"));
            combo.Add(new MenuSeparator("CWS", "W Settings"));
            combo.Add(new MenuSlider("CW", "Min W To Use, 0 = Off", 1, 0, 5));
            combo.Add(new MenuSeparator("CES", "E Settings"));
            combo.Add(new MenuBool("CE", "Use E"));
            combo.Add(new MenuBool("CEG", "Use E To GapClose"));
            combo.Add(new MenuSeparator("CRS", "R Settings"));
            combo.Add(new MenuSlider("CR", "Use R1 If Enemys Nerby, 0=off", 1, 0, 5));
            combo.Add(new MenuBool("CRM", "Use R Max Damge"));

            menu.Add(combo);
            harass = new RivemMenu("harss", "harass");
            harass.Add(new MenuSeparator("CQS", "Q Settings"));
            harass.Add(new MenuBool("HQ", "Use Q"));
            harass.Add(new MenuList("HQP", "Q On ", new string[] { "Target", "Target Position", "Mouse Position" }));
            harass.Add(new MenuBool("HQGap", "Use Q Gapcloser"));
            harass.Add(new MenuSeparator("HWS", "W Settings"));
            harass.Add(new MenuSlider("HW", "Min W To Use, 0 = Off", 1, 0, 5));
            menu.Add(harass);

            laneclear = new RivemMenu("LaneClear", "LaneClear");
            laneclear.Add(new MenuBool("LCQ", "Use Q"));
            laneclear.Add(new MenuBool("LCW", "Use W"));
            laneclear.Add(new MenuBool("LCE", "Use E"));
            laneclear.Add(new MenuBool("LCT", "Use Tiamat"));
            menu.Add(laneclear);

            jungle = new RivemMenu("Jungle", "Jungle");
            jungle.Add(new MenuBool("JQ", "Use Q"));
            jungle.Add(new MenuBool("JW", "Use W"));
            jungle.Add(new MenuBool("JE", "Use E"));
            jungle.Add(new MenuBool("JT", "Use Tiamat"));
            menu.Add(jungle);

            misc = new RivemMenu("misc", "Misc");
            misc.Add(new MenuBool("MAW", "Auto W"));
            misc.Add(new MenuBool("ABE", "Auto Block Damge E"));
            misc.Add(new MenuSlider("AENW", "Auto Enterupt(W), 0=off, 1=on, 2=only in combo"));
            misc.Add(new MenuSlider("AENQ", "Auto Enterupt(3rdQ), 0=off, 1=on, 2=only in combo"));
            misc.Add(new MenuBool("AENE", "Use E To Enterupt(if not in range)"));
            menu.Add(misc);*/

           
        }
    }
}
