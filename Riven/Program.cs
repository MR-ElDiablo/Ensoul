using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SharpDX;



namespace Riven
{
    class  Program
    {
        # region Decler
        private static float LastQ;
        private static Spell Q, W, E, R;
        private static GameObject targ;
        private static bool RBuff = false;
        private static sbyte QCount = 0;
        private static int LastTik;
        private static int LastQs=0;
        public static AIHeroClient myhero { get { return ObjectManager.Player; } }
        #endregion

        #region Menu
        

        #endregion

        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static bool AfterAA,OnAA,BeforeAA;
        private static int tik = 0;
        private static void OnMenuLoad()
        {

            var menu = new Menu("D_Riven", "Diablos Riven", true);
            RivemMenu.Combo.AddToMainMenu(menu);
            menu.Attach();

        }
        private static void OnGameLoad()
        {
            
            if (ObjectManager.Player.CharacterName != "Riven")
            {
                Console.WriteLine("Diablo Riven Not Loaded");
                return;
            }

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R, 900);
            R.SetSkillshot(0.25f, 45, 1600, false, false, SkillshotType.Cone);
            OnMenuLoad();
            
            //Game.OnUpdate += OnUpdate;

            AIBaseClient.OnDoCast += OnCast;
            //AIBaseClient.OnProcessSpellCast += OnProcessCast;
            AIBaseClient.OnPlayAnimation += OnAnimation;
            Orbwalker.OnAction += OrbAction;
            Game.Print("<b><font color='#0040FF'>Diablo</font><font color='#990000'> Riven </font></b> Loaded!");
        }

        private static void OrbAction(object sender, OrbwalkerActionArgs args)
        {
         
            if (args.Type == OrbwalkerType.AfterAttack) { AfterAA = true;  }
            else AfterAA = false;
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
                default: break;
            }
        }

        private static void OnUpdate(EventArgs args) //OnTick 
        {
            if (myhero.IsDead)
                return;
        }




        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1000);
            if (target == null) { return; }

            if(Q.IsReady() && AfterAA )
            {
                    Q.CastOnUnit(target);
            }
        }
       
        private static void OnAnimation(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs args)
        { 
            if (!sender.IsMe) return;
            switch (args.Animation) // when my abilitys casts this trigers; 
            {

                case "Spell1a": //1st Q
                   
                   // QCount = 1;
                  // DelayAction.Add(4000, SetQRange);

                   //DelayAction.Add(100, Reset);
                    break;

                case "Spell1b": //2ndQ
                    //LastQ = Variables.GameTimeTickCount;
                    //QCount=2;
                    //SetQRange();
                    //DelayAction.Add(4000, SetQRange);
                    //DelayAction.Add(40, Reset);
                    break;

                case "Spell1c": //3rd Q
                    //QCount =0;
                    //SetQRange();
                    //DelayAction.Add(40, Reset);
                    QCount = 1;
                    DelayAction.Add(200, () => QCount = 0);
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

        private static void OnCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
          

            if (!sender.IsMe) { return; }
            if(args.SData.Name == Q.SData.Name)
            {

                Orbwalker.AttackState = false;
                Orbwalker.MovementState = false;
                //targ=args.Target;
                DelayAction.Add(150,Reset);
   
            }
        }

        private static void OnProcessCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {


            if (!sender.IsMe) { return; }
            if (args.SData.Name == Q.SData.Name)
            {
               
            }
        }

        private static void SetQRange()
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

        private static void Reset() //moves to mouse for spell animationcancel
        {

            int delay = RivemMenu.Combo.Delay.Value;
            int therddelay = RivemMenu.Combo.TherdQDelay.Value;
            var playerDi = myhero.Direction;
            var pos = myhero.Position;
            if (playerDi.X < 0) { pos.X +=100; }
            else { pos.X -= 100; }
            if (playerDi.Y < 0) { pos.Y += 100; }
            else { pos.Y -= 100; }
            myhero.IssueOrder(GameObjectOrder.MoveTo, pos);
            if (QCount == 0)
            {
                DelayAction.Add(delay - Game.Ping, () => Orbwalker.AttackState = true);
                DelayAction.Add(delay - Game.Ping, () => Orbwalker.MovementState = true);
            }
            else
            {
                DelayAction.Add(therddelay - Game.Ping, () => Orbwalker.AttackState = true);
                DelayAction.Add(therddelay - Game.Ping, () => Orbwalker.MovementState = true);
            }

        }


    }

    

        
    
}
