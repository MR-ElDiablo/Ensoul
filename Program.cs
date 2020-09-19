using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SharpDX;


namespace DiablosRiven
{
    class Program
    {
        # region Decler
        private static Menu menu, combo, harass, laneclear, jungle, misc;
        private static float LastQ;
        private static Spell Q, W, E, R, R2;
        private static bool RBuff = false;
        private static sbyte QCount = 0;
        private static AIBaseClient target;
        private static int tik => Variables.GameTimeTickCount;
        private static int LastTik;
        private static int LastQs=0;

        private static bool C_Q => menu["combo"].GetValue<MenuBool>("CQ");
        private static bool C_QT => menu["combo"].GetValue<MenuBool>("CQTarget");
        private static bool C_QGap => menu["combo"].GetValue<MenuBool>("CQG");
        private static int C_W => menu["combo"].GetValue<MenuSlider>("CW").Value;
        private static bool C_E => menu["combo"].GetValue<MenuBool>("CE");
        private static int C_R => menu["combo"].GetValue<MenuSlider>("CR").Value;
        private static bool C_RM => menu["combo"].GetValue<MenuBool>("CRM");


        private static bool LC_Q => menu["laneclear"].GetValue<MenuBool>("LCQ");
        private static bool LC_W => menu["laneclear"].GetValue<MenuBool>("LCW");
        private static bool LC_E => menu["laneclear"].GetValue<MenuBool>("LCE");
        private static bool J_Q => menu["jungle"].GetValue<MenuBool>("JQ");
        private static bool J_W => menu["jungle"].GetValue<MenuBool>("JW");
        private static bool J_E => menu["jungle"].GetValue<MenuBool>("JE");

        private static bool M_Eent => menu["misc"].GetValue<MenuBool>("AE");

        public static AIHeroClient myhero { get { return ObjectManager.Player; } }
        #endregion

        #region Menu
        private static void OnMenuLoad()
        {
            menu = new Menu("DRiven", "Diablos Riven", true);
            menu.Add(new MenuSeparator("RivenV", "v1.0"));
            combo = new Menu("combo", "combo");
            combo.Add(new MenuSeparator("CQS", "Q Settings"));
            combo.Add(new MenuBool("CQ", "Use Q"));
            combo.Add(new MenuBool("CQTarget", "Fast Q "));
            combo.Add(new MenuBool("CQG", "Use Q To GapClose"));
            combo.Add(new MenuSeparator("CWS", "W Settings"));
            combo.Add(new MenuSlider("CW", "Min Emenemys Around W To Use, 0 = Off", 1, 0, 5));
            combo.Add(new MenuSeparator("CES", "E Settings"));
            combo.Add(new MenuBool("CE", "Use E"));
            combo.Add(new MenuBool("CEG", "Use E To GapClose"));
            combo.Add(new MenuSeparator("CRS", "R Settings"));
            combo.Add(new MenuSlider("CR", "Use R1 If X Enemy Nerby, 0=off",1,0,5));
            combo.Add(new MenuBool("CRM", "Use R Max Damge"));

            menu.Add(combo);
            harass = new Menu("harss", "harass");
            harass.Add(new MenuSeparator("CQS", "Q Settings"));
            harass.Add(new MenuBool("HQ", "Use Q"));
            harass.Add(new MenuList("HQP", "Q On ", new string[] { "Target", "Target Position", "Mouse Position" }));
            harass.Add(new MenuBool("HQGap", "Use Q Gapcloser"));
            harass.Add(new MenuSeparator("HWS", "W Settings"));
            harass.Add(new MenuSlider("HW", "Min W To Use, 0 = Off",1,0,5));
            menu.Add(harass);

            laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.Add(new MenuBool("LCQ", "Use Q"));
            laneclear.Add(new MenuBool("LCW", "Use W"));
            laneclear.Add(new MenuBool("LCE", "Use E"));
            laneclear.Add(new MenuBool("LCT", "Use Tiamat"));
            menu.Add(laneclear);

            jungle = new Menu("Jungle", "Jungle");
            jungle.Add(new MenuBool("JQ", "Use Q"));
            jungle.Add(new MenuBool("JW", "Use W"));
            jungle.Add(new MenuBool("JE", "Use E"));
            jungle.Add(new MenuBool("JT", "Use Tiamat"));
            menu.Add(jungle);

            misc = new Menu("misc", "Misc");
            misc.Add(new MenuBool("MAW", "Auto W"));
            misc.Add(new MenuBool("ABE", "Auto Block Damge E"));
            misc.Add(new MenuSlider("AENW", "Auto Enterupt(W), 0=off, 1=on, 2=only in combo"));
            misc.Add(new MenuSlider("AENQ", "Auto Enterupt(3rdQ), 0=off, 1=on, 2=only in combo"));
            misc.Add(new MenuBool("AENE", "Use E To Enterupt(if not in range)"));
            menu.Add(misc);

            menu.Attach();
        }

        #endregion

        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
            
        }
        private static void OnGameLoad()
        {
            
            if (ObjectManager.Player.CharacterName != "Riven")
            {
                Console.WriteLine("Diablo Riven Not Loaded");

                return;
            }

            Q = new Spell(SpellSlot.Q, 150);
            W = new Spell(SpellSlot.W, 250);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R, 900);
            R.SetSkillshot(0.25f, 45, 1600, false, false, SkillshotType.Cone);
            OnMenuLoad();

            
           Game.OnUpdate += OnUpdate;
            AIBaseClient.OnProcessSpellCast += OnDoCast;
            //AIBaseClient.OnPlayAnimation += OnPlay;
            Console.WriteLine("riven loaded");
            Game.Print("<b><font color='#0040FF'>Diablo</font><font color='#990000'> Riven </font></b> Loaded!");
        }

        
        private static void OnUpdate(EventArgs args) //OnTick 
        {
            if (myhero.IsDead)
                return;

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;/*
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break; */

            }

        }

        
        private static void Combo()
        {



            var target = TargetSelector.GetTarget(900);
            if (!target.IsValid) return;
            //var pos = myhero.Position.Extend(target.Position, Q.Range); use?

            if (!Orbwalker.Attack(target) && target.InAutoAttackRange(700) &&( (C_Q) || ( C_R >= 1)||C_E) && target.IsValidTarget())

            {
                if (E.IsReady()&& (C_E)) { E.Cast(target.Position); }
                if (R.IsReady()) { DelayAction.Add(20, () => R.Cast()); }
                if (C_W >= 1 && W.IsReady() && W.IsInRange(target)) DelayAction.Add(250, () => W.Cast()); 
                if (Q.IsReady() && C_QT && Q.IsInRange(target)) DelayAction.Add(320, () => Q.Cast(target));
                else if (C_QGap) DelayAction.Add(320, () => Q.Cast(target.Position));

            }
            

 
        }
       
        private void OnPlay(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs args)
        { //spellchecker
            if (!sender.IsMe) return;
            switch (args.Animation) // when my abilitys casts this trigers; 
            {

                case "Spell1a": //1st Q

                    QCount = 1;
                    DelayAction.Add(4000, SetQRange);
                    DelayAction.Add(50, Reset);
                    break;

                case "Spell1b": //2ndQ
                    LastQ = Variables.GameTimeTickCount;
                    QCount=2;
                    SetQRange();
                    DelayAction.Add(4000, SetQRange);
                    DelayAction.Add(50, Reset);
                    break;

                case "Spell1c": //3rd Q
                    QCount =0;
                    SetQRange();
                    DelayAction.Add(30, Reset);

                    break;

                case "Spell3": //W
                    DelayAction.Add(250, ()=>Orbwalker.ResetAutoAttackTimer());
                    break;

                case "Spell4a": //R Use
                    W.Range = 300;
                    RBuff = true;
                    SetQRange();
                    DelayAction.Add(14950, () => RBuff = false);
                    DelayAction.Add(15000, () => W.Range = 250);
                    DelayAction.Add(14990, SetQRange);
                    Orbwalker.ResetAutoAttackTimer();
                    break;
                    case "Spell4b": //2nd R cast

                    Orbwalker.ResetAutoAttackTimer();
                    break;
            
            }

            

    }
        private static void  OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name; //return if NOT AA
            if (!sender.IsMe || !Orbwalker.IsAutoAttack(spellName)) { return; }
            

            target = (AIBaseClient)args.Target;

            Game.Print("Last Tik");
            Game.Print(tik-LastTik);
            LastTik = tik;

            if (Q.IsReady() && ((C_Q && Orbwalker.ActiveMode == OrbwalkerMode.Combo) || (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && (LC_Q || J_Q))))
            {
                Q.Cast(target);

            }


        }
        private void SetQRange()
        {

            if ((LastQ + 3800) >= Variables.GameTimeTickCount && QCount != 0 )
            {

                if (QCount == 2) //3rd Q range
                {
                    if (RBuff == true) //active R                       
                    { Q.Range = 300; }
                    else
                    { Q.Range = 200; }
                }
                else
                {
                    if (RBuff == false) //active R                       
                    { Q.Range = 150; }
                    else
                    { Q.Range = 200; }
                }
            }

            else
            {
                if (RBuff == true)
                { Q.Range = 200; }
                else
                { Q.Range = 150; }

                QCount = 0;
            }

        }

        private void Reset() //moves to mouse for spell animationcancel
        {

            //Orbwalker.LastAutoAttackTick = 0;
            myhero.IssueOrder(GameObjectOrder.MoveTo, (Game.CursorPos).Extend(myhero.Position, +10));
            
            
        }
        

    }

    

        
    
}
