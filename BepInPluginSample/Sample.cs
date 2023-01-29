using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepInPluginSample
{
    [BepInPlugin("Game.Lilly.Plugin", "Lilly", "1.0")]
    public class Sample : BaseUnityPlugin
    {
        #region GUI
        public static ManualLogSource logger;

        static Harmony harmony;

        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> isGUIOnKey;
        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> isOpenKey;

        private ConfigEntry<bool> isGUIOn;
        private ConfigEntry<bool> isOpen;
        private ConfigEntry<float> uiW;
        private ConfigEntry<float> uiH;

        public int windowId = 542;
        public Rect windowRect;

        public string title = "";
        public string windowName = ""; // 변수용 
        public string FullName = "Plugin"; // 창 펼쳤을때
        public string ShortName = "P"; // 접었을때

        GUILayoutOption h;
        GUILayoutOption w;
        public Vector2 scrollPosition;
        #endregion

        #region 변수
        // =========================================================

        private static ConfigEntry<bool> noTP;
        private static ConfigEntry<bool> noDamage;
        private static ConfigEntry<bool> ultimate;
        private static ConfigEntry<bool> changeOverDrive;
        // private static ConfigEntry<float> uiW;
        // private static ConfigEntry<float> xpMulti;

        // =========================================================
        #endregion

        public void Awake()
        {
            #region GUI
            logger = Logger;
            Logger.LogMessage("Awake");

            isGUIOnKey = Config.Bind("GUI", "isGUIOnKey", new KeyboardShortcut(KeyCode.Keypad0));// 이건 단축키
            isOpenKey = Config.Bind("GUI", "isOpenKey", new KeyboardShortcut(KeyCode.KeypadPeriod));// 이건 단축키

            isGUIOn = Config.Bind("GUI", "isGUIOn", true);
            isOpen = Config.Bind("GUI", "isOpen", true);
            isOpen.SettingChanged += IsOpen_SettingChanged;
            uiW = Config.Bind("GUI", "uiW", 300f);
            uiH = Config.Bind("GUI", "uiH", 600f);

            if (isOpen.Value)
                windowRect = new Rect(Screen.width - 65, 0, uiW.Value, 800);
            else
                windowRect = new Rect(Screen.width - uiW.Value, 0, uiW.Value, 800);

            IsOpen_SettingChanged(null, null);
            #endregion

            #region 변수 설정
            // =========================================================

            noTP = Config.Bind("game", "noTP", true);
            noDamage = Config.Bind("game", "noDamage", true);
            ultimate = Config.Bind("game", "ultimate", true);
            changeOverDrive = Config.Bind("game", "changeOverDrive", true);
            // xpMulti = Config.Bind("game", "xpMulti", 2f);

            // =========================================================
            #endregion
        }

        #region GUI
        public void IsOpen_SettingChanged(object sender, EventArgs e)
        {
            logger.LogInfo($"IsOpen_SettingChanged {isOpen.Value} , {isGUIOn.Value},{windowRect.x} ");
            if (isOpen.Value)
            {
                title = isGUIOnKey.Value.ToString() + "," + isOpenKey.Value.ToString();
                h = GUILayout.Height(uiH.Value);
                w = GUILayout.Width(uiW.Value);
                windowName = FullName;
                windowRect.x -= (uiW.Value - 64);
            }
            else
            {
                title = "";
                h = GUILayout.Height(40);
                w = GUILayout.Width(60);
                windowName = ShortName;
                windowRect.x += (uiW.Value - 64);
            }
        }
        #endregion

        public void OnEnable()
        {
            Logger.LogWarning("OnEnable");
            // 하모니 패치
            try // 가급적 try 처리 해주기. 하모니 패치중에 오류나면 다른 플러그인까지 영향 미침
            {
                harmony = Harmony.CreateAndPatchAll(typeof(Sample));
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        public void Update()
        {
            #region GUI
            if (isGUIOnKey.Value.IsUp())// 단축키가 일치할때
            {
                isGUIOn.Value = !isGUIOn.Value;
            }
            if (isOpenKey.Value.IsUp())// 단축키가 일치할때
            {
                if (isGUIOn.Value)
                {
                    isOpen.Value = !isOpen.Value;
                }
                else
                {
                    isGUIOn.Value = true;
                    isOpen.Value = true;
                }
            }
            #endregion
        }

        #region GUI
        public void OnGUI()
        {
            if (!isGUIOn.Value)
                return;

            // 창 나가는거 방지
            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 4, Screen.width - 4);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 4, Screen.height - 4);
            windowRect = GUILayout.Window(windowId, windowRect, WindowFunction, windowName, w, h);
        }
        #endregion

        public virtual void WindowFunction(int id)
        {
            #region GUI
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
                                        // 라벨 추가
                                        //GUILayout.Label(windowName, GUILayout.Height(20));
                                        // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            if (isOpen.Value) GUILayout.Label(title);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { isOpen.Value = !isOpen.Value; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn.Value = false; }
            GUI.changed = false;

            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!isOpen.Value) // 닫혔을때
            {
            }
            else // 열렸을때
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                #endregion

                #region 여기에 GUI 항목 작성
                // =========================================================

                // if (GUILayout.Button($"{hpNotChg.Value}")) { hpNotChg.Value = !hpNotChg.Value; }

                // GUILayout.BeginHorizontal();
                // GUILayout.Label($"ammoMulti {ammoMulti.Value}");
                // if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))) { ammoMulti.Value += 1; }
                // if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { ammoMulti.Value -= 1; }
                // GUILayout.EndHorizontal();

                // =========================================================
                #endregion
                if (GUILayout.Button($"noTP {noTP.Value}")) { noTP.Value = !noTP.Value; }
                if (GUILayout.Button($"nnoDamageo {noDamage.Value}")) { noDamage.Value = !noDamage.Value; }
                if (GUILayout.Button($"ultimate {ultimate.Value}")) { ultimate.Value = !ultimate.Value; }
                if (GUILayout.Button($"changeOverDrive {changeOverDrive.Value}")) { changeOverDrive.Value = !changeOverDrive.Value; }

                GUILayout.Label($"---  ---");
                GUILayout.Label($"playTime  ; {GameManager.playTime}");                
                if (GUILayout.Button($"money = 999999 ; {GameManager.money}")) { GameManager.money = 999999; }
                if (GUILayout.Button($"ultimate = 100 ; {Battle.ultimate}")) { Battle.ultimate = 100; }
                if (GUILayout.Button($"ODMultiplicator = 70 ; {BattleFunctions.ODMultiplicator}")) { BattleFunctions.ODMultiplicator = 70; }
                //if (GUILayout.Button($"targetOverride = 70 ; {BattleFunctions.targetOverride}")) { BattleFunctions.targetOverride = 70; }
                //if (GUILayout.Button($"targetOverrideTurn ; {BattleFunctions.targetOverrideTurn}")) { BattleFunctions.targetOverrideTurn = !BattleFunctions.targetOverrideTurn; }

                if (BattleFunctions.instance)
                {
                }
                else
                {
                    GUILayout.Label($"--- BattleFunctions null ---");
                }
                
                //foreach (var item in Battle.partyGO)
                if (GetData.instance)
                {
                    GUILayout.Label($"--- partyMembers {GetData.instance.partyMembers.Count} ---");
                    foreach (var c in GetData.instance.partyMembers)
                    {
                        GUILayout.Label($"name ; {c.memberName}");
                        GUILayout.Label($"currentHP ; {c.currentHP}");
                        GUILayout.Label($"currentTP ; {c.currentTP}");
                    }
                }
                else
                {
                    GUILayout.Label($"--- GetData null ---");
                }
                
                //전투 이력용
                /*
                GUILayout.Label($"--- currentStats {currentStats.Count} ---");
                foreach (CurrentStats c in currentStats)
                {
                    GUILayout.Label($"name ; {c.name}");
                    GUILayout.Label($"currentHP ; {c.currentHP}");
                    GUILayout.Label($"currentTP ; {c.currentTP}");
                }                
                */

                #region GUI
                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
            #endregion
        }


        public void OnDisable()
        {
            Logger.LogWarning("OnDisable");
            harmony?.UnpatchSelf();
        }

        #region Harmony
        // ====================== 하모니 패치 샘플 ===================================

        public static List<CurrentStats> currentStats = new List<CurrentStats>();
        
        //전투이력용
        /*
        [HarmonyPatch(typeof(PartyInfoBattle), MethodType.Constructor)]
        [HarmonyPostfix]
        public static void XPPickerCtor(PartyInfoBattle __instance, List<CurrentStats> ___currentStats)
        {
            logger.LogMessage($"currentStats ;");
            Sample.currentStats = ___currentStats;
        }
        */

        [HarmonyPatch(typeof(Battle), "BattleStart")]
        [HarmonyPostfix]
        public static void BattleStart()
        {
            logger.LogMessage($"BattleStart ;");
            currentStats.Clear();
            foreach (var item in Battle.partyGO)
            {
                currentStats.Add(item.GetComponent<CurrentStats>());
               
            }
            if (ultimate.Value)
            {
                //GameManager.ultimate = 100;
                Battle.ultimate = 100;
            }
            if (changeOverDrive.Value)
                BattleFunctions.ODMultiplicator = 70;
        }

        //public static void ChangeUltimate(int mod, GameObject user = null, bool setInsteadOfRaise = false)
        [HarmonyPatch(typeof(BattleFunctions), "ChangeUltimate", typeof(int),typeof(GameObject), typeof(bool))]
        [HarmonyPostfix]
        public static void ChangeUltimate(int mod, GameObject user , bool setInsteadOfRaise)
        {
            logger.LogMessage($"ChangeUltimate ; {mod} ; {setInsteadOfRaise}");
            if (ultimate.Value)
            {
                //GameManager.ultimate = 100;
                Battle.ultimate = 100;
            }
        }

        //public static void SpendTP(int user, int skill)
        [HarmonyPatch(typeof(BattleFunctions), "SpendTP")]
        [HarmonyPostfix]
        public static void SpendTP( int user, int skill)//BattleFunctions __instance,
        {
            logger.LogMessage($"SpendTP ; {user} ; {skill}");
            if (noTP.Value)
                Battle.partyGO[user].GetComponent<CurrentStats>().currentTP = Battle.partyGO[user].GetComponent<CurrentStats>().maxTP;
        }

        [HarmonyPatch(typeof(BattleFunctions), "LoseTP")]
        [HarmonyPostfix]
        public static void LoseTP( int user, int amount)//BattleFunctions __instance,
        {
            logger.LogMessage($"LoseTP ; {user} ; {amount}");
            if (noTP.Value)
                Battle.partyGO[user].GetComponent<CurrentStats>().currentTP = Battle.partyGO[user].GetComponent<CurrentStats>().maxTP;
        }
        
        [HarmonyPatch(typeof(BattleFunctions), "Damage", typeof(GameObject), typeof(int), typeof(bool), typeof(bool))]
        [HarmonyPostfix]
        public static void Damage(GameObject target, int amount, bool dontDie = false, bool setTo1HP = false)//BattleFunctions __instance,
        {
            logger.LogMessage($"Damage ; {target.tag} ; {amount} ; {dontDie} ; {setTo1HP}");
            if (noDamage.Value)
                if ( target.tag != "Enemy")
                {
                    var c = target.GetComponent<CurrentStats>();
                    logger.LogMessage($"Damage ; {c.name} ; {c.currentHP} ; {c.maxHP} ;");
                    c.currentHP = c.maxHP;
                }


        }

        //public static void ChangeOverDrive(int mod, bool enemyTurn = false)
        [HarmonyPatch(typeof(BattleFunctions), "ChangeOverDrive", typeof(int), typeof(bool))]
        [HarmonyPostfix]
        public static void ChangeOverDrive(int mod, bool enemyTurn)//BattleFunctions __instance,
        {
            logger.LogMessage($"ChangeOverDrive ; {mod} ; {enemyTurn} ");
            if (changeOverDrive.Value)
                BattleFunctions.ODMultiplicator = 70;


        }

        /*
         
        [HarmonyPatch(typeof(XPPicker), MethodType.Constructor)]
        [HarmonyPostfix]
        public static void XPPickerCtor(XPPicker __instance, ref float ___pickupRadius)
        {
            //logger.LogWarning($"XPPicker.ctor {___pickupRadius}");
            ___pickupRadius = pickupRadius.Value;
        }
        [HarmonyPatch(typeof(AEnemy), "DamageMult", MethodType.Setter)]
        [HarmonyPrefix]
        public static void SetDamageMult(ref float __0)
        {
            if (!eMultOn.Value)
            {
                return;
            }
            __0 *= eDamageMult.Value;
        }
        */
        // =========================================================
        #endregion
    }
}
