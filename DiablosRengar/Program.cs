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
using System.Security.Permissions;
namespace DiablosRengar
{

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class Program

    {

        #region Decler
        private static Menu menu, combo, harass, laneclear, jungle, Uitem, misc, smite;
        private static Items.Item _youmuu, _tiamat, _hydra, _titanic, _blade, _bilge, _rand, _lotis, _QSS, _QSS_MC;
        private static string[] jungleMinions = new string[] //stolen list from sko :D
                               {
                                        "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_RiftHerald",
                                        "SRU_Red", "SRU_Krug", "SRU_Dragon_Air", "SRU_Dragon_Water", "SRU_Dragon_Fire",
                                        "SRU_Dragon_Elder", "SRU_Baron", "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf"
                               };

        private static Spell Q, W, E, R;
        private static Spell _smite { get; set; }
        private static bool dash =false , AfterAA =false, OnAA=false, QSSReady = false;
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
      


        public static AIHeroClient myhero => ObjectManager.Player;

        #endregion 
        #region Menu
        private static void OnMenuLoad()
        {
            menu = new Menu("DRengar", "Diablos Rengar", true)
            {
                new MenuSeparator("DiablosRengar", "Diablos Rengar v1.0")
            };
            combo = new Menu("combo", "combo")
            {
                new MenuBool("CSAVE", "Save Empowerd", false),
                new MenuList("CEMP", "Empowerd", new string[] { "Q", "W", "E" }),
                new MenuBool("CQ", "Use Q"),
                new MenuBool("CQAA", "^- After AA (On=MoreDPS)"),
                new MenuBool("ICQAA", "^- Ignore on Dash(On=Burst, Off=MoreDPS)"),
                new MenuBool("CW", "Use W"),
                new MenuBool("CE", "Use E"),
                new MenuList("EChance", " E Hitchance", new string[] { "Low", "Medium", "High", "Very High" }, 1),
                new MenuList("EEmpChance", "Empwerd E Hitchance", new string[] { "Low", "Medium", "High", "Very High" }, 1),
                new MenuBool("CEemp", "Use E Empowerd If Out Of Q Range"),
                new MenuBool("CR", "Use R", false),
                new MenuSlider("CR_HP", "^If Enemy Hp <", 50, 1, 100),
                new MenuSlider("CR_Range", "Min Range To Ult <", 1000, 1, 2000)
            };
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


            laneclear = new Menu("laneclear", "LaneClear")
            {
                new MenuList("LCEMP", "Empowerd", new string[] { "Q", "W", "E" }),
                new MenuBool("LCSAVE", "Save Empowerd", false),
                new MenuBool("LCQ", "Use Q"),
                new MenuBool("LCW", "Use W"),
                new MenuBool("LCE", "Use E")
            };
            menu.Add(laneclear);

            jungle = new Menu("Jungle", "JungleClear")
            {
                new MenuList("JEMP", "Use Empowerd", new string[] { "Q", "W", "E" }),
                new MenuBool("JSAVE", "Save Empowerd", false),
                new MenuBool("JQ", "Use Q"),
                new MenuBool("JW", "Use W"),
                new MenuBool("JE", "Use E")
            };
            menu.Add(jungle);

            smite = new Menu("smite", "Use Smite")
            {
                new MenuBool("UseSmite", "Use Smite"),
                new MenuBool("UseSmiteJungle", "Use Smite On Jungle (big monsters)"),
                //new MenuBool("UseSmiteChamp", "Use Smite On Enemy Champs")
            };
            menu.Add(smite);

            Uitem = new Menu("Uitem", "Use Items");
            Menu GhostBMenu = new Menu("GH", "Ghost Blade")
            {
                new MenuBool("C_GH", "Use On Combo"),
                new MenuSlider("C_GH_Range", "Min Range To Use <", 1000, 1, 2000),
                new MenuBool("C_GH_Ult", "^Use Only With R"),
                new MenuBool("H_GH", "Use On Harass")
            };
            Uitem.Add(GhostBMenu);
            Menu HydraMenu = new Menu("HD", "Hydra/Titanic")
            {
                new MenuBool("C_HD", "Use On Combo"),
                new MenuBool("H_HD", "Use On Harass"),
                new MenuBool("LC_HD", "Use In LaneClear"),
                new MenuBool("J_HD", "Use In Jungle")
            };
            Uitem.Add(HydraMenu);

            Menu QSS = new Menu("QSS", "QSS")
            {
                new MenuBool("Use_QSS", "Use QSS"),
                new MenuBool("C_QSS_Only", "QSS Only In Combo")
            };
            Uitem.Add(QSS);
            Menu C_QSS = new Menu("C_QSS", "Combo QSS list")
            {
                new MenuBool("Stun", "Stun"),
                new MenuBool("Silence", "Silence"),
                new MenuBool("Taunt", "Taunt"),
                new MenuBool("Polymorph", "Polymorph"),
                new MenuBool("Slow", "Slow"),
                new MenuBool("Snare", "Snare"),
                new MenuBool("Sleep", "Sleep"),
                new MenuBool("Fear", "Fear"),
                new MenuBool("Charm", "Charm"),
                new MenuBool("Suppression", "Suppression"),
                new MenuBool("Blind", "Blind"),
                new MenuBool("Flee", "Flee"),
                new MenuBool("Knockup", "KnockUp"),
                new MenuBool("Knockback", "KnockBack"),
                new MenuBool("Drowsy", "Drowsy"),
                new MenuBool("Asleep", "Asleep")
            };
            QSS.Add(C_QSS);
            /*Menu BorkMenu = new Menu("BORK", "BORK");
            BorkMenu.Add(new MenuBool("C_BORK", "Use On Combo"));
            BorkMenu.Add(new MenuBool("H_BORK", "Use On Harass"));
            BorkMenu.Add(new MenuSlider("BorkEnemyhp", "If Enemy Hp <", 50, 1, 100));
            BorkMenu.Add(new MenuSlider("Borkmyhp", "Or Your Hp <", 85, 1, 100));
            Uitem.Add(BorkMenu);
*/
            menu.Add(Uitem);


            misc = new Menu("misc", "Misc")
            {
                new MenuBool("M_W_CC", "Auto W CC"),
                new MenuBool("QSS_W", "Prefer W Over QSS")
            };
            Menu WCC = new Menu("WCC", "^W List")
            {
                new MenuBool("Stun", "Stun"),
                new MenuBool("Silence", "Silence"),
                new MenuBool("Taunt", "Taunt"),
                new MenuBool("Polymorph", "Polymorph"),
                new MenuBool("Slow", "Slow"),
                new MenuBool("Snare", "Snare"),
                new MenuBool("Sleep", "Sleep"),
                new MenuBool("Fear", "Fear"),
                new MenuBool("Charm", "Charm"),
                new MenuBool("Suppression", "Suppression"),
                new MenuBool("Blind", "Blind"),
                new MenuBool("Flee", "Flee"),
                new MenuBool("Knockup", "KnockUp"),
                new MenuBool("Knockback", "KnockBack"),
                new MenuBool("Drowsy", "Drowsy"),
                new MenuBool("Asleep", "Asleep")
            };
            misc.Add(WCC);
            //misc.Add(new MenuBool("M_W_Heal", "Auto W Heal"));
            //misc.Add(new MenuSlider("M_W_MaxHP", "Auto Heal If Max %HP<", 50, 0, 100));
            //misc.Add(new MenuSlider("M_W_CurrentHP", "^Only If Can Heal Current %HP ", 50, 0, 100));
            menu.Add(misc);
            menu.Attach();
        }

        #endregion
        private static int QCount=0, ECount=0, WCount = 0;
        private static int HDCount = 0;
        public static void Check()
        {
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                bool wb = new WebClient().DownloadString("https://github.com/MR-ElDiablo/Ensoul/blob/master/DiablosRengar/Version.txt").Contains("1.0.0.1");
                if (!wb)
                {
                    Game.Print("<b><font Size='25' color='#0000b2'> Newer Version Avalible For Diablos Rengar</font></b>");
                }
                else
                    Game.Print("<b><font Size='35' color='#FF0000'> Diablos Rengar Loaded</font></b>");

            }
            catch (Exception E)
            {
                Console.WriteLine("An error try again " +E);
            }
        }
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
            Check(); //if UPD
          

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 1000);
            E.SetSkillshot(0.25f, 140f, 1500f, true, SkillshotType.Line);
            R = new Spell(SpellSlot.R);
            _smite = new Spell(myhero.GetSpellSlot("summonersmite"), 500);
            _bilge = new Items.Item(3144, 450f);
            _QSS = new Items.Item(3140, 10);
            _QSS_MC = new Items.Item(3139, 10);
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

        }
        private static void OnUpdate(EventArgs args)
        {
            if (myhero.IsDead) return;
          
            if (menu["misc"].GetValue<MenuBool>("M_W_CC"))
            {
                Auto_W_CC();
            }
            else { CC_W = false; }

            if (!Uitem["QSS"].GetValue<MenuBool>("C_QSS_Only") && _QSS.IsOwned() && _QSS.IsReady)
            {
                Auto_QSS();
            }
            else if(!Uitem["QSS"].GetValue<MenuBool>("C_QSS_Only") && _QSS_MC.IsOwned() && _QSS_MC.IsReady)
                {
                    Auto_QSS_MC();
                }
            Auto_Smite();
        }
        private static void OrbAction(Object sender, OrbwalkerActionArgs args)
        {
            if (Uitem["QSS"].GetValue<MenuBool>("C_QSS_Only") && _QSS.IsOwned() && _QSS.IsReady)
            {
                Auto_QSS();
            }
            else if (Uitem["QSS"].GetValue<MenuBool>("C_QSS_Only") && _QSS_MC.IsOwned() && _QSS_MC.IsReady)
                {
                    Auto_QSS_MC();
                }
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

                    if (Uitem["QSS"].GetValue<MenuBool>("C_QSS_Only") && _QSS.IsOwned() && _QSS.IsReady)
                    {
                        Auto_QSS();
                    }
                    else if (Uitem["QSS"].GetValue<MenuBool>("C_QSS_Only") && _QSS_MC.IsOwned() && _QSS_MC.IsReady)
                    {
                        Auto_QSS_MC();
                    }
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
            
            if (C_Hydra  &&  AfterAA && ECount + 300<Tok)
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
            if (i < 4 && myhero.Mana!=4 )
            {
                
                if (C_Q && !dash && Q.CanCast(newtarget) && newtarget.InCurrentAutoAttackRange(75) && ECount + 300 < Tok && QCount + 170 < Tok && !OnAA && myhero.Mana != 4 && i < 4 && !(RengarPassive&& myhero.Mana==0) && !RengarR && HDCount + 300 < Tok)
                {
                   

                    if (!C_QAA ) { Q.Cast(); }
                    else if (C_QAA && AfterAA) { Q.Cast();}
                        
                }
                if (C_W && HDCount +300 <Tok &&!dash) { _WCast(newtarget); }
                if (C_E && !dash && HDCount + 300 < Tok) { _ECast(newtarget); }
                
            }
            
            if (R.IsReady() && C_R && (int)newtarget.Position.DistanceToPlayer()>C_RRange && newtarget.HealthPercent <=C_RHP  && !dash && ECount+500<Tok)
            {
                R.Cast();
            }
            if (myhero.Mana == 4 && !C_Save && !CC_W)
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
            if (!OnAA && LC_Hydra && ECount + 300 < Tok && !dash )
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
            if (myhero.Mana != 4 && i < 4)
            {
                if (Q.CanCast(minons) && LC_Q && minons.InCurrentAutoAttackRange(200) && i < 4 && ECount + 300 < Tok && QCount + 200 < Tok)
                {
                    if (!dash && AfterAA)
                    {
                        Q.Cast();
                        QCount = Tok;
                    }
                    else if (dash)
                    {
                        Q.Cast();
                        QCount = Tok;
                        i++;
                    }
                }
                if (W.CanCast(minons) && LC_W && minons.IsValidTarget(W.Range) && i < 4 && WCount + 200 < Tok && ECount + 300 < Tok && !OnAA && HDCount + 300 < Tok && myhero.Mana != 4)
                {
                    if (!(RengarPassive && myhero.Mana == 0))
                    {
                        W.Cast();
                        WCount = Tok;
                        i++;
                    }
                    else if (!RengarPassive)
                    {
                        W.Cast();
                        WCount = Tok;
                        i++;
                    }

                }
                if (LC_E && !dash && myhero.Mana != 4) { _ECast(minons); }
                else if (LC_E && HDCount + 300 < Tok && i < 4 && E.CanCast(minons) && ECount + 370 < Tok && myhero.Mana != 4) { if (E.State == (SpellState.CooldownOrSealed) || E.State == SpellState.Disabled || E.State == SpellState.NotAvailable) { return; } E.Cast(minons.Position); i++; ECount = Tok; }
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
            if (!OnAA && J_Hydra && ECount + 300 <Tok &&!dash)
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
            if (myhero.Mana != 4 && i < 4)
            {
                if (Q.CanCast(mob) && J_Q && mob.InCurrentAutoAttackRange(200)&& i < 4 && ECount + 300 < Tok && QCount + 200 < Tok )
                {
                    if (!dash && AfterAA) 
                    {
                        Q.Cast();
                        QCount = Tok;
                    }
                    else if(dash)
                    {
                        Q.Cast();
                        QCount = Tok;
                        i++;
                    }
                }
                if (W.CanCast(mob) && J_W && mob.IsValidTarget(W.Range) && i < 4 && WCount + 200 < Tok && ECount + 300 < Tok && !OnAA &&HDCount+300<Tok &&myhero.Mana!=4)
                {
                    if (!(RengarPassive && myhero.Mana == 0))
                    {
                        W.Cast();
                        WCount = Tok;
                        i++;
                    }
                    else if (!RengarPassive)
                    {
                        W.Cast();
                        WCount = Tok;
                        i++;
                    }

                }
                if (J_E &&!dash && myhero.Mana != 4) { _ECast(mob); }
                else if (J_E && HDCount + 300 < Tok && i < 4 && E.CanCast(mob) &&ECount+370<Tok && myhero.Mana != 4) { if (E.State == (SpellState.CooldownOrSealed) || E.State == SpellState.Disabled || E.State == SpellState.NotAvailable) { return; } E.Cast(mob.Position); i++; ECount = Tok; }
           
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
            
            if (Q.CanCast(QEmptarget) && QEmptarget.InCurrentAutoAttackRange(400) )
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
            if (W.CanCast(Wtarget) && ECount + 300 < Tok && WCount + 150 < Tok && i<4 && !RengarR && myhero.Mana!=4)
            {
                if (!RengarPassive)
                {
                        W.Cast();
                        WCount = Tok;
                        i++;
                    
                }
                else if (RengarPassive && myhero.Mana!=0 )
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
            if (E.CanCast(Etarget) && HDCount + 300 < Tok && ECount + 380 < Tok && i < 4 && !RengarR && !OnAA && myhero.Mana != 4 && !dash)
            {
                
                var predE = E.GetPrediction(Etarget);
                
                if (!RengarPassive  && predE.Hitchance >=EChance() && predE.CollisionObjects.Count==0)
                {
                    
                        E.Cast(Etarget.Position);
                        ECount = Tok;
                        i++;
                }
                else if (RengarPassive && myhero.Mana !=0 && predE.Hitchance >= EChance() && predE.CollisionObjects.Count == 0)
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
               if (RengarPassive && Etarget.Position.DistanceToPlayer() <= 200f && !dash)
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
                    return HitChance.Low;
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
        private static void Auto_Smite()// credit kappa HD
        {
            if (_smite==null || !_smite.IsReady() || !menu["smite"].GetValue<MenuBool>("UseSmite")) {  return; }
            if (menu["smite"].GetValue<MenuBool>("UseSmiteJungle"))
            {
                var mob = GameObjects.Jungle.OrderByDescending(j=>j.Health).FirstOrDefault(j=>j.IsValidTarget(500)); 
                if (mob == null) {  return; }
                if (mob.Health<myhero.GetSummonerSpellDamage(mob,SummonerSpell.Smite)) 
                {
                    foreach (string s in jungleMinions)
                    {
                        if (mob.Name.Contains(s)) { _smite.Cast(mob); }

                    }
                }
                  
                   
                
            }
        }
        private static void Auto_W_CC()
        {

            if (!menu["misc"].GetValue<MenuBool>("QSS_W")&&(_QSS.IsOwned()&&_QSS.IsReady))
            { return; }
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
        private static void Auto_QSS()
        {
            
            if (menu["misc"].GetValue<MenuBool>("QSS_W") && myhero.Mana ==4 )
            { return; }
            foreach (var buffType in (mybuff[])Enum.GetValues(typeof(mybuff)))
            {
                if (ObjectManager.Player.HasBuffOfType((BuffType)buffType) && Uitem["QSS"]["C_QSS"].GetValue<MenuBool>(buffType.ToString()))
                {
                     _QSS.Cast(); 
                }

            }

        }
        private static void Auto_QSS_MC()
        {

            if (menu["misc"].GetValue<MenuBool>("QSS_W") && myhero.Mana == 4)
            { return; }
            foreach (var buffType in (mybuff[])Enum.GetValues(typeof(mybuff)))
            {
                if (ObjectManager.Player.HasBuffOfType((BuffType)buffType) && Uitem["QSS"]["C_QSS"].GetValue<MenuBool>(buffType.ToString()))
                {
                    _QSS_MC.Cast();
                }

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
