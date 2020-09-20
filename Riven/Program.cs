using System;
using System.Linq;
using System.Runtime.InteropServices;
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
        private static bool RBuff = false, CanR_2,qE=false;
        private static sbyte QCount = 0;
        private static int E_Timer=0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        #endregion

        #region Menu
        

        #endregion

        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static bool AfterAA,OnAA,BeforeAA;
        private static int getTime=> Environment.TickCount;
        private static void OnMenuLoad()
        {

            var menu = new Menu("D_Riven", "Diablos Riven", true);
            RivenMenu.Combo.AddToMainMenu(menu);
            RivenMenu.Misc.AddToMainMenu(menu);
            menu.Attach();

        }
        private static void OnGameLoad()
        {
            
            if (ObjectManager.Player.CharacterName != "Riven")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 250);
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
            if (args.Type == OrbwalkerType.OnAttack)
            {
                return;
            }
            if (args.Type == OrbwalkerType.AfterAttack)
            {
                AfterAA = true;
            }
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
            if (Player.IsDead)
                return;
        }




        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1000);
            if (target == null) { return; }
            if (RivenMenu.Combo.E && E.IsReady())
            {
               if(target.DistanceToPlayer() <= E.Range) { E.Cast(target.Position); }
               else if (target.DistanceToPlayer() <= E.Range + Q.Range && RivenMenu.Combo.Q && Q.IsReady())
                {
                    E.Cast(target.Position);
                }

            }
            if(RivenMenu.Combo.R1 && !CanR_2 &&R.IsReady())
            {
                if(E_Timer + 250 < getTime && target.HealthPercent<=RivenMenu.Combo.R1HP)
                {
                    R.Cast();
                }
            }
            else if(RivenMenu.Combo.R2 && CanR_2) {
                var rpred = R.GetPrediction(target).CastPosition;
                if ( target.Health / target.MaxHealth <= 0.25  && E_Timer + 400 < getTime && R.IsInRange(rpred) &&RivenMenu.Combo.RMax)
                {
                    R.Cast(rpred);
                } 
            }
            else if (W.IsReady() && RivenMenu.Combo.W)
            {
                if (E_Timer + 250 < getTime && W.IsInRange(target))
                {
                    W.Cast();
                }
            }
            else if (Q.IsReady() && RivenMenu.Combo.Q && LastQ + 300 <= getTime)
            {
                if (E_Timer + 250 < getTime && E_Timer + 1500 > getTime && LastQ + 1000 <= getTime && Q.IsInRange(target) && RivenMenu.Combo.QGapClose)
                {
                    Q.CastOnUnit(target);
                }
                else if (AfterAA && !Player.IsWindingUp)
                {
                    if (RivenMenu.Combo.QTarget)
                    {
                        Q.CastOnUnit(target);
                    }
                    else { Q.Cast(target.Position); }
                }
                else if (RivenMenu.Combo.QGapClose && E_Timer + 1500 < getTime && !(RivenMenu.Combo.E && E.IsReady(2000)) && !target.InCurrentAutoAttackRange(100))
                {
                    Q.CastOnUnit(target);
                }
            }

        }
       
        private static void OnAnimation(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs args)
        { 
            if (!sender.IsMe) return;
            switch (args.Animation) // when my abilitys casts this trigers; 
            {

                case "Spell1a": //1st Q
                   
                    QCount = 1;
                    LastQ = getTime;
                    DelayAction.Add(3500, SetQRange);
                    break;

                case "Spell1b": //2ndQ
                    QCount = 2;
                    LastQ = getTime;
                    SetQRange();
                    DelayAction.Add(3500, SetQRange);
                    break;

                case "Spell1c": //3rd Q
                    //QCount =0;
                    //SetQRange();
                    LastQ = getTime;
                    QCount = 3;
                    DelayAction.Add(200, () => QCount = 0);
                    break;
                    
                case "Spell2": //W
                    DelayAction.Add(350, ()=>Orbwalker.ResetAutoAttackTimer());
                    break;

                case "Spell3": //E

                    E_Timer = getTime;
                    break;

                case "Spell4a": //R Use
                    W.Range = 300;
                    RBuff = true;
                    CanR_2 = true;
                    SetQRange();
                    DelayAction.Add(14950, () => RBuff = false);
                    DelayAction.Add(14600, () => W.Range = 250);
                    DelayAction.Add(14500, SetQRange);
                    break;

                    case "Spell4b": //2nd R cast
                        CanR_2 = false;
                        break;
                default: break;

            }

        }

        private static void OnCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
          

            if (!sender.IsMe) { return; }
            if (args.SData.Name == Q.SData.Name)
            {

                Orbwalker.AttackState = false;
                Orbwalker.MovementState = false;
                DelayAction.Add(150,Reset);
   
            }
          
        }

        private static void OnProcessCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {


            if (!sender.IsMe) { return; }
            
            if (args.SData.Name == W.SData.Name)
            {

            }
        }

        private static void SetQRange()
        {
            if ((LastQ + 3500) <= getTime && QCount != 0)
            {

                if (RivenMenu.Misc.KeepQ)
                {
                    Q.Cast(Game.CursorPos);
                }
            }
            if (QCount == 2)//3rd Q
            {
                if (RBuff) //active R                       
                { Q.Range = 575; }
                else
                { Q.Range = 500; }
            }
            else
            {
                if (RBuff )
                { Q.Range = 475; }
                else
                { Q.Range = 400; }

            }
        

        }

        private static void Reset() //moves to mouse for spell animationcancel
        {

            int delay = RivenMenu.Misc.Delay.Value; 
            int therddelay = RivenMenu.Misc.ThirdQDelay.Value;
            var playerDi = Player.Direction;
            var pos =Player.Position;
            
            if (playerDi.X < 0) { pos.X +=100; }
            else { pos.X -= 100; }
            if (playerDi.Y < 0) { pos.Y += 100; }
            else
            {
                pos.Y -= 100;
            }
            
            Player.IssueOrder(GameObjectOrder.MoveTo, pos);
            if (QCount != 3)
                {
                    DelayAction.Add(delay, () => Orbwalker.AttackState = true);
                    DelayAction.Add(delay, () => Orbwalker.MovementState = true);


            }
            else
            {

                    DelayAction.Add(therddelay, () => Orbwalker.AttackState = true);
                    DelayAction.Add(therddelay, () => Orbwalker.MovementState = true);
            }
            
        }


    }

    

        
    
}
