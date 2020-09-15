using System;
using System.Linq;
using System.Collections.Generic;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp;
using SharpDX;
using static EnsoulSharp.SDK.Items;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;
using EnsoulSharp.SDK.Utils;
namespace Kayne
{
    class Kayne
    {
        static void Main(string[] args)
        {

            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static Spell Q, Q2, W, W2, E, E2, R;
        private static AIHeroClient Player;
        private static void OnGameLoad()
        {
            Player = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(0.25f, 100f, 1900f, true, true, SkillshotType.Line);
            R = new Spell(SpellSlot.R, 550f);
            Orbwalker.OnAction += OrbAction;
            Game.Print("Diablos Kayne loaded");

        }
        private static void OrbAction(Object sender, OrbwalkerActionArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;

                case OrbwalkerMode.Harass:
                    //Harass();
                    break;

                case OrbwalkerMode.LaneClear:
                    //JClear();
                    //LClear();
                    break;
            }
        }

        private static void Combo()
        {

            var newtarget = TargetSelector.GetTarget(1000);
            if (newtarget == null) { return; }
            var targPred = SpellPrediction.GetPrediction(newtarget, 0.5f);
            if (Q.CanCast(newtarget))
            {
                Q.Cast(targPred.CastPosition);
            }
            if (W.CanCast(newtarget)) { W.Cast(targPred.CastPosition); }
            if (R.CanCast(newtarget)&&Player.HealthPercent<50) { R.CastOnUnit(newtarget); }
        }
    }
}
