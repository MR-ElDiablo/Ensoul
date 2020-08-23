using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using System.Net;
using System.Threading.Tasks;
using System.Reflection;

namespace DiablosRengar
{


    class Program

    {

        # region Decler
        private static Menu menu, combo, harass, laneclear, jungle, Uitem, misc;
        private static Items.Item _youmuu, _tiamat, _hydra, _titanic, _blade, _bilge, _rand, _lotis;
        private static Spell Q, W, E, R;
        //private static AIBaseClient target;
        private static bool dash, AfterAA, OnAA = false;
        //private static int newtik; //used for delay on spellcasts
        private static sbyte i = 0; //used as counter for emp spells
       
        //private static bool CC_W = false; //CC check
        private static int Tok => Environment.TickCount;
        
        private static bool RengarPassive => myhero.HasBuff("rengarpassivebuff");
        private static bool RengarR => myhero.HasBuff("RengarR");
        //combo
        private static int C_EMP => menu["combo"].GetValue<MenuList>("CEMP").Index;
        private static bool C_Save => menu["combo"].GetValue<MenuBool>("CSAVE");
        private static bool C_Q => menu["combo"].GetValue<MenuBool>("CQ");
        private static bool C_QAA => menu["combo"].GetValue<MenuBool>("CQAA");
        //private static bool C_QEmpAA=> menu["combo"].GetValue<MenuBool>("CEmpQAA");
        private static bool C_IQAA => menu["combo"].GetValue<MenuBool>("ICQAA");
        private static bool C_W => menu["combo"].GetValue<MenuBool>("CW");
        private static bool CC_W = false;
        private static bool C_E => menu["combo"].GetValue<MenuBool>("CE");

        private static int C_EChance => menu["combo"].GetValue<MenuList>("EChance").Index;
        private static int C_EEmpChance => menu["combo"].GetValue<MenuList>("EEmpChance").Index;
        
        private static bool C_Eemp => menu["combo"].GetValue<MenuBool>("CEemp");
        private static bool C_R => menu["combo"].GetValue<MenuBool>("CR");
        private static int C_RHP => menu["combo"].GetValue<MenuSlider>("CR_HP").Value;
        private static int C_RRange => menu["combo"].GetValue<MenuSlider>("CR_Range").Value;
        private static bool C_Hydra => Uitem["HD"].GetValue<MenuBool>("C_HD");
        private static bool C_GhostB => Uitem["GH"].GetValue<MenuBool>("C_GH");
        private static int C_GhostB_Range => Uitem["GH"].GetValue<MenuSlider>("C_GH_Range").Value;

        private static bool C_GhostB_R => Uitem["GH"].GetValue<MenuBool>("C_GH_Ult");

        /*harass
        private static int H_EMP => menu["harass"].GetValue<MenuList>("HEMP").Index;
        private static bool H_Save => menu["harass"].GetValue<MenuBool>("HSAVE");
        private static bool H_Q => menu["harass"].GetValue<MenuBool>("HQ");
        private static bool H_QAA => menu["harass"].GetValue<MenuBool>("HQAA");
        private static bool H_IQAA => menu["harass"].GetValue<MenuBool>("IHQAA");
        private static bool H_W => menu["harass"].GetValue<MenuBool>("HW");
        private static bool H_E => menu["harass"].GetValue<MenuBool>("HE");
        //private static bool H_Eemp => menu["harass"].GetValue<MenuBool>("HEemp");*/

        //Lane Clear
        private static int LC_EMP => menu["laneclear"].GetValue<MenuList>("LCEMP").Index;
        private static bool LC_Save => menu["laneclear"].GetValue<MenuBool>("LCSAVE");
        private static bool LC_Q => menu["laneclear"].GetValue<MenuBool>("LCQ");
        private static bool LC_W => menu["laneclear"].GetValue<MenuBool>("LCW");
        private static bool LC_E => menu["laneclear"].GetValue<MenuBool>("LCE");
        private static bool LC_Hydra => Uitem["HD"].GetValue<MenuBool>("LC_HD");

        //Jungle
        private static int J_EMP => menu["Jungle"].GetValue<MenuList>("JEMP").Index;
        private static bool J_Save => menu["Jungle"].GetValue<MenuBool>("JSAVE");
        private static bool J_Q => menu["Jungle"].GetValue<MenuBool>("JQ");
        private static bool J_W => menu["Jungle"].GetValue<MenuBool>("JW");
        private static bool J_E => menu["Jungle"].GetValue<MenuBool>("JE");
        private static bool J_Hydra => Uitem["HD"].GetValue<MenuBool>("J_HD");


        //misc
        //private static bool M_Eent => menu["misc"].GetValue<MenuBool>("AW");

        public static AIHeroClient myhero => ObjectManager.Player;

        #endregion 
        #region Menu
        private static void OnMenuLoad()
        {
            menu = new Menu("DRengar", "Diablos Rengar", true);
            menu.Add(new MenuSeparator("DiablosRengar", "Diablos Rengar v1.0"));
            combo = new Menu("combo", "combo");
            combo.Add(new MenuBool("CSAVE", "Save Empowerd",false));
            combo.Add(new MenuList("CEMP", "Empowerd", new string[] { "Q", "W", "E" }));
            combo.Add(new MenuBool("CQ", "Use Q"));
            combo.Add(new MenuBool("CQAA", "^After AA"));
            combo.Add(new MenuBool("ICQAA", "^Ignore on Dash"));
            combo.Add(new MenuBool("CW", "Use W"));
            combo.Add(new MenuBool("CE", "Use E"));
            combo.Add(new MenuList("EChance", " E Hitchance", new string[] { "Low", "Medium", "High", "Very High" }, 1));
            combo.Add(new MenuList("EEmpChance", "Empwerd E Hitchance", new string[] { "Low", "Medium", "High", "Very High" }, 1));
            combo.Add(new MenuBool("CEemp", "Use E Empowerd If Out Of Q Range"));
            combo.Add(new MenuBool("CR", "Use R",false));
            combo.Add(new MenuSlider("CR_HP", "^If Enemy Hp <", 50, 1, 100));
            combo.Add(new MenuSlider("CR_Range", "Min Range To Ult <", 1000, 1, 2000));
            menu.Add(combo);
            /*harass = new Menu("harass", "harass");
            harass.Add(new MenuList("HEMP", "Empowerd", new string[] { "Q", "W", "E" }));
            harass.Add(new MenuBool("HSAVE", "Save Empowerd"));
            harass.Add(new MenuBool("HQ", "Use Q"));
            harass.Add(new MenuBool("HQAA", "^After AA"));
            harass.Add(new MenuBool("ICQAA", "^Ignore on Dash"));
            harass.Add(new MenuBool("HW", "Use W"));
            harass.Add(new MenuBool("HE", "Use E"));
            //harass.Add(new MenuBool("HEemp", "Use E Empowerd If Out Of Q Range"));
            menu.Add(harass);*/


            laneclear = new Menu("laneclear", "LaneClear");
            laneclear.Add(new MenuList("LCEMP", "Empowerd", new string[] { "Q", "W", "E" }));
            laneclear.Add(new MenuBool("LCSAVE", "Save Empowerd", false)); ;
            laneclear.Add(new MenuBool("LCQ", "Use Q"));
            laneclear.Add(new MenuBool("LCW", "Use W"));
            laneclear.Add(new MenuBool("LCE", "Use E"));
            menu.Add(laneclear);

            jungle = new Menu("Jungle", "JungleClear");
            jungle.Add(new MenuList("JEMP", "Use Empowerd", new string[] { "Q", "W", "E"}));
            jungle.Add(new MenuBool("JSAVE", "Save Empowerd",false));
            jungle.Add(new MenuBool("JQ", "Use Q"));
            jungle.Add(new MenuBool("JW", "Use W"));
            jungle.Add(new MenuBool("JE", "Use E"));
            menu.Add(jungle);
            Uitem = new Menu("Uitem", "Use Items");
            Menu GhostBMenu = new Menu("GH", "Ghost Blade");
            GhostBMenu.Add(new MenuBool("C_GH", "Use On Combo"));
            GhostBMenu.Add(new MenuSlider("C_GH_Range", "Min Range To Use <", 1000, 1, 2000));
            GhostBMenu.Add(new MenuBool("C_GH_Ult", "^Use Only With R"));
            GhostBMenu.Add(new MenuBool("H_GH", "Use On Harass"));
            Uitem.Add(GhostBMenu);

            Menu HydraMenu = new Menu("HD", "Hydra/Titanic");
            HydraMenu.Add(new MenuBool("C_HD", "Use On Combo"));
            HydraMenu.Add(new MenuBool("H_HD", "Use On Harass"));
            HydraMenu.Add(new MenuBool("LC_HD", "Use In LaneClear"));
            HydraMenu.Add(new MenuBool("J_HD", "Use In Jungle"));
            Uitem.Add(HydraMenu);

            /*Menu BorkMenu = new Menu("BORK", "BORK");
            BorkMenu.Add(new MenuBool("C_BORK", "Use On Combo"));
            BorkMenu.Add(new MenuBool("H_BORK", "Use On Harass"));
            BorkMenu.Add(new MenuSlider("BorkEnemyhp", "If Enemy Hp <", 50, 1, 100));
            BorkMenu.Add(new MenuSlider("Borkmyhp", "Or Your Hp <", 85, 1, 100));
            Uitem.Add(BorkMenu);

            Menu QSSMenu = new Menu("QSS", "QSS");
            QSSMenu.Add(new MenuBool("C_QSS", "Use On Combo"));
            Menu QSSComboMenu = new Menu("QSSCOMBO", "QSS Combo");
            QSSComboMenu.Add(new MenuBool("QSSComb_Stun", "Stun"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Snare", "Snare"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Blind", "Blind"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Charm", "Charm"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Slow", "Slow"));
            QSSComboMenu.Add(new MenuBool("QSSComb_KnockUp", "KnockUp"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Fear", "Fear"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Flee", "Flee"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Taunt", "Taunt"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Suppression", "Suppression"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Polymorph", "Polymorph"));
            QSSComboMenu.Add(new MenuBool("QSSComb_Silence", "Silence"));

            QSSMenu.Add(QSSComboMenu);
            QSSMenu.Add(new MenuBool("Always_QSS", "Use Always"));
            Menu QSSAlwaysMenu = new Menu("QSSAlways", "QSS Always");
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Stun", "Stun"));

            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Silence", "Silence"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Snare", "Snare"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Blind", "Blind"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Charm", "Charm"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Slow", "Slow"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_KnockUp", "KnockUp"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Fear", "Fear"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Flee", "Flee"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Taunt", "Taunt"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Suppression", "Suppression"));
            QSSAlwaysMenu.Add(new MenuBool("QSSAlways_Polymorph", "Polymorph"));
            QSSMenu.Add(QSSAlwaysMenu);
            Uitem.Add(QSSMenu);*/
            menu.Add(Uitem);


            misc = new Menu("misc", "Misc");
            misc.Add(new MenuBool("M_W_CC", "Auto W CC"));
            Menu WCC = new Menu("WCC", "^W List");
            WCC.Add(new MenuBool("Stun", "Stun"));
            WCC.Add(new MenuBool("Silence", "Silence"));
            WCC.Add(new MenuBool("Taunt", "Taunt"));
            WCC.Add(new MenuBool("Polymorph", "Polymorph"));
            WCC.Add(new MenuBool("Slow", "Slow"));
            WCC.Add(new MenuBool("Snare", "Snare"));
            WCC.Add(new MenuBool("Sleep", "Sleep"));
            WCC.Add(new MenuBool("Fear", "Fear"));
            WCC.Add(new MenuBool("Charm", "Charm"));
            WCC.Add(new MenuBool("Suppression", "Suppression"));
            WCC.Add(new MenuBool("Blind", "Blind"));
            WCC.Add(new MenuBool("Flee", "Flee"));
            WCC.Add(new MenuBool("Knockup", "KnockUp"));
            WCC.Add(new MenuBool("Knockback", "KnockBack"));
            WCC.Add(new MenuBool("Drowsy", "Drowsy"));
            WCC.Add(new MenuBool("Asleep", "Asleep"));
            misc.Add(WCC);
            //misc.Add(new MenuBool("M_W_Heal", "Auto W Heal"));
            //misc.Add(new MenuSlider("M_W_MaxHP", "Auto Heal If Max %HP<", 50, 0, 100));
            //misc.Add(new MenuSlider("M_W_CurrentHP", "^Only If Can Heal Current %HP ", 50, 0, 100));
            menu.Add(misc);
            menu.Attach();
        }

        #endregion
        
        private static int QCount, ECount, WCount = 0;
        private static int HDCount = 0;
        /*public static void Check()
        {
            try
            {
                bool wb = new WebClient().DownloadString("https://github.com/MR-ElDiablo/Ensoul/blob/master/DiablosRengar/Version.txt").Contains("1.0.0.0");

               

                if (wb)
                {
                    Game.Print("Rengar Oudated");
                }
                else
                    Game.Print("Rengar Updated");

            }
            catch
            {
                Game.Print("An error try again");
            }
        }
        public async Task Updater()
        {
            var client = new WebClient();
            try
            {
                await client.DownloadFileTaskAsync("", "");
                Console.WriteLine("Downloaded");
            }
            catch (Exception)
            {
                Console.WriteLine("Error When Downloading");
            }
        }*/
        static void Main(string[] args)
        {
           

            GameEvent.OnGameLoad += OnGameLoad;

        }
        private static void OnGameLoad()
        {

            if (ObjectManager.Player.CharacterName != "Rengar")
            {
                Console.WriteLine("Diablo Rengar Not Loaded");
                return;
            }

            /*try
            {
                new Program().Updater().Wait();
            }
            catch (Exception)
            {
                Console.WriteLine("Error When Updating");
            }*/

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 1000);
            E.SetSkillshot(0.25f, 140f, 1500f, true, SkillshotType.Line);
            R = new Spell(SpellSlot.R);
            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _titanic = new Items.Item(3748, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _youmuu = new Items.Item(3142, 10);
            
            OnMenuLoad();
            Game.OnUpdate += OnUpdate;
            Orbwalker.OnAction += OrbAction;
            AIBaseClient.OnDoCast += OnDoCast;
            Dash.OnDash += Dashing;
            Game.Print("<b><font Size='40' color='#FF0000'> Diablos Rengar</font></b>");

        }
        private static void OnUpdate(EventArgs args)
        {
            if (myhero.IsDead) return;
          
            if (menu["misc"].GetValue<MenuBool>("M_W_CC"))
            {
                Auto_W_CC();
            }
            else { CC_W = false; }
            

        }
        private static void OrbAction(Object sender, OrbwalkerActionArgs args)
        {

            if (myhero.HasBuff("rengaroutofcombat") && myhero.Mana == 0) { i = 0; }
            if (args.Type == OrbwalkerType.OnAttack)
            {
                
                OnAA = true;
                if ((i == 0||myhero.Mana==0)  && (RengarPassive || myhero.HasBuff("RengarR"))) { i++; }
                if (myhero.HasBuff("RengarQ")) { i++; QCount = Tok; }

            }
            else OnAA = false;
            AfterAA = (args.Type == OrbwalkerType.AfterAttack) ?  true :  false;
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    return;
                case OrbwalkerMode.Harass:
                    //Harass();
                    return;
                case OrbwalkerMode.LaneClear:
                    JClear();
                    LClear();
                    return;
            }
            
        }
        private static void Dashing(AIBaseClient sender, Dash.DashArgs args)
        {
            if (sender != myhero) { return; }
            if (myhero.Mana == 0) { i++; }
            if (myhero.HasBuff("RengarQ")) { i++; }
            dash = true;
            DelayAction.Add(args.Duration, () => dash = false);
            var newtarget = TargetSelector.GetTarget(2000);
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    
                    if ((C_IQAA || !C_QAA) && C_Q) { _DashQ(newtarget);  }
                    else if (C_QAA && C_Q && !C_IQAA)
                    {
                        DelayAction.Add(args.Duration, () => _DashQ(newtarget)); 
                    }
                    if (C_E && i<4) { _EDashCast(newtarget); }
                    break;
                case OrbwalkerMode.LaneClear:
                    break;
            }
        }
        private static void OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name;
            if (!sender.IsMe) { return; }
            else if (args.SData.Name.Contains("Rengar") && spellName.Contains("Emp") && spellName!=("RengarQEmpAttack"))
            {
                i = 0;
            } 

        }
        private static void Combo()
        {
            var newtarget = TargetSelector.GetTarget(2000);
            
            if (newtarget == null) { return; }
            if (C_GhostB && myhero.HasItem(3142) &&_youmuu.IsReady&& (int)newtarget.Position.DistanceToPlayer() <= C_GhostB_Range) { if (C_GhostB_R && RengarR) { _youmuu.Cast(); }
                else if (!C_GhostB_R) { _youmuu.Cast(); }
            }
            if (AfterAA && C_Hydra )
            {

                if (_hydra.IsReady && _hydra.IsInRange(newtarget))
                {
                    _hydra.Cast(); HDCount = Tok;
                }
                else if (_titanic.IsReady && _titanic.IsInRange(newtarget))
                {
                    _titanic.Cast(); HDCount = Tok;
                }
                else if (_tiamat.IsReady && _tiamat.IsInRange(newtarget)) { _tiamat.Cast(); HDCount = Tok; }
                

            }
            if (i < 4 && myhero.Mana!=4 && HDCount+300<Tok)
            {
                
                if (C_Q && !dash && Q.CanCast(newtarget) && newtarget.InCurrentAutoAttackRange(75) && ECount + 350 < Tok && QCount + 200 < Tok && !OnAA && myhero.Mana != 4 && i < 4 && !RengarPassive &&!RengarR)
                {
                   

                    if (!C_QAA) { Q.Cast(); }
                        else if (C_QAA && AfterAA) { Q.Cast();}
                        
                }
                if (C_W && !dash) { _WCast(newtarget); }
                if (C_E && !dash) { _ECast(newtarget); }
                
            }
            if (R.IsReady() && C_R && (int)newtarget.Position.DistanceToPlayer()>C_RRange && newtarget.HealthPercent <=C_RHP  && !dash && ECount+500<Tok)
            {
                R.Cast();
            }
            if (myhero.Mana == 4 && !C_Save &&!CC_W)
            {
                switch (C_EMP)
                {
                    case 0:
                        _QEmpCast (newtarget);
                        break;
                    case 1:
                        _WEmpCast (newtarget);
                            break;
                    case 2:
                        _EEmpCast(newtarget);
                        break;
                }
            }
            


        }
        private static void LClear()
        {

            var minons = GameObjects.EnemyMinions.OrderByDescending(m => m.Health).FirstOrDefault(m => m.IsValidTarget(E.Range));
            if (minons == null) return;
            if (AfterAA && LC_Hydra)
            {

                if (_hydra.IsReady && _hydra.IsInRange(minons))
                {
                    _hydra.Cast(); HDCount = Tok;
                }
                else if (_titanic.IsReady && _titanic.IsInRange(minons))
                {
                    _titanic.Cast(); HDCount = Tok;
                }
                else if (_tiamat.IsReady && _tiamat.IsInRange(minons)) { _tiamat.Cast(); HDCount = Tok; }

            }
            if (i < 4 && myhero.Mana != 4 && HDCount +300<Tok)
            {
                if (Q.IsReady() && LC_Q && minons.IsValidTarget(500) && i != 4 && QCount + 200 < Tok && !OnAA)
                {
                    Q.Cast();
                }
                if (W.CanCast(minons) && LC_W && minons.IsValidTarget(W.Range)  && i < 4 && WCount + 200 < Tok && ECount + 300 < Tok && !OnAA)
                {
                    i++;
                    W.Cast();
                    WCount = Tok;

                }
                if (E.CanCast(minons) && LC_E && minons.IsValidTarget(E.Range)  && i < 4 && ECount + 400 < Tok && !OnAA)

                {
                    E.Cast(minons.Position);
                    ECount = Tok;

                }
            }
            if (myhero.Mana == 4 && !LC_Save &&!CC_W)
            {
                switch (LC_EMP)
                {
                    case 0:
                        if (minons.IsValidTarget(myhero.GetCurrentAutoAttackRange()+200) && Q.CanCast(minons) && QCount + 150 < Tok &&ECount +300 < Tok) 
                        { 
                            Q.Cast();
                            QCount = Tok;
                        }
                        break; 
                    case 1:
                        if (minons.IsValidTarget(W.Range)&& W.CanCast(minons) && WCount +150 <Tok && ECount+300<Tok)
                        {
                            W.Cast();
                            WCount = Tok;
                        }
                        break;
                    case 2:
                        if (minons.IsValidTarget(E.Range) && E.CanCast(minons) && ECount +300<Tok ) { E.Cast(minons.Position); ECount = Tok; }
                        break;

                }

            }

        }
        private static void JClear()
        {
            var mob = GameObjects.Jungle.OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(E.Range));
            if (mob == null) { return; }
            if (AfterAA && J_Hydra)
            {

                if (_hydra.IsReady && _hydra.IsInRange(mob))
                {
                    _hydra.Cast(); HDCount = Tok;
                }
                else if (_titanic.IsReady && _titanic.IsInRange(mob))
                {
                    _titanic.Cast(); HDCount = Tok;
                }
                else if (_tiamat.IsReady && _tiamat.IsInRange(mob)) { _tiamat.Cast(); HDCount = Tok; }

            }
            if (myhero.Mana != 4 && i < 4 && HDCount + 300 < Tok)
            {
                if (Q.CanCast(mob) && J_Q && mob.IsValidTarget(500) && i < 4 && ECount + 300 < Tok && QCount + 200 < Tok && !OnAA && AfterAA)
                {
                    Q.Cast();
                    QCount = Tok;
                }
                if (W.CanCast(mob) && J_W && mob.IsValidTarget(W.Range) && i < 4 && WCount + 200 < Tok && ECount + 300 < Tok && !OnAA)
                {
                    W.Cast();
                    WCount = Tok;
                    i++;

                }
                if (E.CanCast(mob) && J_E && mob.IsValidTarget(E.Range) && i < 4 && ECount + 400 < Tok && !OnAA)
                {
                    E.Cast(mob.Position);
                    ECount = Tok;
                    i++;

                }
            }
            if (myhero.Mana == 4 && !J_Save &&!CC_W)
            {
                switch (J_EMP)
                {
                    case 0:
                        Q.Cast();
                        break;
                    case 1:
                        if (mob.IsValidTarget(W.Range))
                            W.Cast();
                        break;
                    case 2:
                        if (mob.IsValidTarget(E.Range)) { E.Cast(mob.Position); }
                        break;
                    
                }

            }
            
        }
        private static void _DashQ(AIBaseClient QNoramltarget)
        {
            
            if(Q.State == (SpellState.CooldownOrSealed) || Q.State == SpellState.Disabled || Q.State == SpellState.NotAvailable) return;
            if (Q.CanCast(QNoramltarget) && myhero.Mana!=4 && i<4 && QCount +170 <Tok && ECount+300<Tok )
            { Q.Cast(); QCount = Tok; if ((C_IQAA || !C_QAA) && C_Q) { i++; } }
        }
        private static void _QEmpCast (AIBaseClient QEmptarget)
        {
            
            if (Q.CanCast(QEmptarget) && QEmptarget.InCurrentAutoAttackRange(400) && ECount + 320 < Tok  && !OnAA )
            {
                
                Q.Cast();
                QCount = Tok ;
            }
            else if (E.CanCast(QEmptarget) && C_Eemp && !QEmptarget.InCurrentAutoAttackRange(400) &&!RengarPassive &&!RengarR && ECount + 380 < Tok) { E.Cast(QEmptarget.Position); ECount = Tok; }
            return;
        }
        private static void _WCast (AIBaseClient Wtarget)
        {
            
            if (W.State == (SpellState.CooldownOrSealed) || W.State == SpellState.Disabled || W.State == SpellState.NotAvailable) return;
            if (W.CanCast(Wtarget) && ECount + 300 < Tok && WCount + 150 < Tok && i<4 && !RengarR && !OnAA &&!dash)
            {
                if (!RengarPassive  && !dash)
                {
                        W.Cast();
                        WCount = Tok;
                        i++;
                    
                }
                else if (RengarPassive && !(Wtarget.DistanceToPlayer()>200f) )
                {
                    W.Cast();
                    WCount = Tok;
                    i++;
                }
               
            }
            return;
        }
        private static void _WEmpCast(AIBaseClient Wtarget)
        {

            if (W.State == (SpellState.CooldownOrSealed) || W.State == SpellState.Disabled || W.State == SpellState.NotAvailable) return;
            
            if (W.CanCast(Wtarget)  && WCount + 120 < Tok  && !RengarR && ECount+270 <Tok )
            {
                W.Cast();
                WCount = Tok;
                
            }
            return;
        }
        private static void _ECast(AIBaseClient Etarget)
        {
            if (E.State == (SpellState.CooldownOrSealed) || E.State == SpellState.Disabled || E.State == SpellState.NotAvailable) { return; }
            if (E.CanCast(Etarget) && ECount + 370 < Tok && i < 4 && !RengarR && QCount+100<Tok&& !OnAA)
            {
                var predE = E.GetPrediction(Etarget);
                if (!RengarPassive && !dash && predE.Hitchance>=EChance()&& predE.CollisionObjects.Count==0 )
                {
                    
                        E.Cast(Etarget.Position);
                        ECount = Tok;
                        i++;

                }
                else if (RengarPassive && Etarget.Distance(myhero) < 200f && !dash && predE.Hitchance >= EChance() && predE.CollisionObjects.Count == 0)
                {
                    E.Cast(Etarget.Position);
                    ECount = Tok;
                    i++;

                }
                
            }
        }
        private static void _EDashCast(AIBaseClient Etarget)
        {
            if (E.State == (SpellState.CooldownOrSealed) || E.State == SpellState.Disabled || E.State == SpellState.NotAvailable) { return; }
            if (E.CanCast(Etarget) && ECount + 370 < Tok && i < 4 )
            {
                    i++;
                    E.Cast(Etarget.Position);
                    ECount = Tok;
                    

            }
        }
        private static void _EEmpCast(AIBaseClient Etarget)
        {
            var predE = E.GetPrediction(Etarget);
            if (E.State == (SpellState.CooldownOrSealed) || E.State == SpellState.Disabled || E.State == SpellState.NotAvailable) { return; }
            if (E.CanCast(Etarget) && ECount + 370 < Tok  && !RengarR)
            {
               if (RengarPassive && Etarget.Position.DistanceToPlayer() < 200f && !dash)
                {
                    if( EEmpChance() <= predE.Hitchance && predE.CollisionObjects.Count==0) {
                    E.Cast(Etarget.Position);
                    ECount = Tok;
                    }
                }
               else if (dash)
                {
                    E.Cast(Etarget.Position);
                    ECount = Tok;
                }
               else if (!RengarPassive && EEmpChance() <= predE.Hitchance && predE.CollisionObjects.Count == 0)
                {
                    E.Cast(Etarget.Position);
                    ECount = Tok;

                }
            }
        }
        private static HitChance EChance()
        {
            switch (C_EChance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }
        private static HitChance EEmpChance()
        {
            switch (C_EEmpChance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }
        private static void Auto_W_CC()
        {
            
            foreach (var buffType in (mybuff[])Enum.GetValues(typeof(mybuff)))
            {
                if (ObjectManager.Player.HasBuffOfType((BuffType)buffType) && misc["WCC"].GetValue<MenuBool>(buffType.ToString()))
                {
                    CC_W = true;
                    if (myhero.Mana == 4)
                    {
                        W.Cast();
                        WCount = Tok;
                    }


                }
                else CC_W = false;
            }

        }
        public enum mybuff
        {
            Stun = 5,
            Silence = 7,
            Taunt = 8,
            Polymorph = 9,
            Slow = 10,
            Snare = 11,
            Sleep = 18,
            Fear = 21,
            Charm = 22,
            Suppression = 24,
            Blind = 25,
            Flee = 28,
            Knockup = 29,
            Knockback = 30,
            Drowsy = 33,
            Asleep = 34
        }

    }
}
