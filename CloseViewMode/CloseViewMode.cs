using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System;
using LegacyDataModel.Beta;
using LordAshes;
using static Unity.Physics.SimulationCallbacks;

namespace XJ_Nekomancer
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.FileAccessPlugin.Guid)]
    //[BepInDependency(LordAshes.StatMessaging.Guid)]
    [BepInDependency(RadialUI.RadialUIPlugin.Guid)]
    [BepInDependency(AssetDataPlugin.Guid)]

    public partial class CloseViewMode : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "CloseViewMode Plugin";                      // Update plugin name (and give the same name under Project | Properties)
        public const string Guid = "org.XJ_Nekomancer.plugins.CloseViewMode";       // Update user name and plugin id (usually changing 'template' to name of plugin)
        public const string Version = "1.0.0.0";                            // Update version as appropriate (and use same version under Project | Properties | Assembly Information)

        // Configuration
        //private ConfigEntry<KeyboardShortcut> triggerKey { get; set; }      // Sample configuration for triggering a plugin via keyboard
        // private ConfigEntry<UnityEngine.Color> baseColor { get; set; }      // Sample configuration for storing/retrieving a color
        // private ConfigEntry<string> baseText { get; set; }                  // Sample configuration for storing/retrieving a string (could be most other data types)

        public enum DiagnosticMode
        {
            none = 0,
            low = 1,
            high = 2,
            ultra = 3
        }

        public static DiagnosticMode diagnostics;


        // Reference to the TaleSpire_CustomData folder used by a lot a plugins.
        // This reference is usually no longer needed if you are using the FileAccessPlugin (which is highly recommended) 
        private string dir = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("/")) + "/TaleSpire_CustomData/";

        public static bool CloseViewActive;
        public static bool GMCloseViewActive;
        
        static RootTargetCameraMode instance = null;
        //static RootTargetCameraMode instance = GameObject.FindObjectOfType<RootTargetCameraMode>();
        FieldInfo minFI;
        FieldInfo maxFI;
        Vector3 minFibackup = Vector3.zero;
        Vector3 maxFibackup = Vector3.zero;


        void Awake()    
        {
      
            // Not required but good idea to log this state for troubleshooting purpose
            UnityEngine.Debug.Log("CloseViewMode: CloseViewMode Is Active.");
            diagnostics = Config.Bind("Troubleshooting", "Diagnostic Mode", DiagnosticMode.ultra).Value;

            if (diagnostics >= DiagnosticMode.ultra) { Debug.Log("CloseViewMode:  Active"); }

            AssetDataPlugin.Subscribe(CloseViewMode.Guid + ".CloseON", CallbackCloseon);                                                                                       
            AssetDataPlugin.Subscribe(CloseViewMode.Guid + ".CloseOFF", CallbackCloseoff);      

            minFI = typeof(RootTargetCameraMode).GetRuntimeFields().Where(f => f.Name == "minZoomPos").ElementAt(0);          
            maxFI = typeof(RootTargetCameraMode).GetRuntimeFields().Where(f => f.Name == "maxZoomPos").ElementAt(0);
             
            RadialUI.RadialSubmenu.EnsureMainMenuItem(CloseViewMode.Guid+".MainMenuCV", RadialUI.RadialSubmenu.MenuType.character, "CloseView Menu", LordAshes.FileAccessPlugin.Image.LoadSprite("Magic.png"));

            RadialUI.RadialSubmenu.CreateSubMenuItem(CloseViewMode.Guid + ".MainMenuCV",
              "Close View On",
              null,
              (cid, obj, mi) => CViewOn(cid),
              true,
              () => { return true; }
          );
            RadialUI.RadialSubmenu.CreateSubMenuItem(CloseViewMode.Guid + ".MainMenuCV",
              "Close View Off",
              null,
              (cid, obj, mi) => CViewOff(cid),
              true,
              () => { return true; }
          );
            RadialUI.RadialSubmenu.CreateSubMenuItem(CloseViewMode.Guid + ".MainMenuCV",
              "(GM only) Force Close View on all players",
              null,
              (cid, obj, mi) => CViewOnGM(cid),
              true,
              () => { return true; }
          );
            RadialUI.RadialSubmenu.CreateSubMenuItem(CloseViewMode.Guid + ".MainMenuCV",
              "(GM only) Disable Close View on all players",
              null,
              (cid, obj, mi) => CViewOffGM(cid),
              true,
              () => { return true; }
          );
         

            var harmony = new Harmony(Guid);
            harmony.PatchAll();

            Utility.PostOnMainPage(this.GetType());
        }


        void Update()
        {
            //if (Utility.isBoardLoaded())
            //{
            //    if (instance == null)
        
            //}
        }

        public void CViewOn(CreatureGuid a = new CreatureGuid())
        {
            if (diagnostics >= DiagnosticMode.low) { Debug.Log("CloseViewMode: CViewOn"); }
            if (CloseViewActive == false)
            {
                CloseViewActive = true;      

                if (instance == null)
                {               
                    try { instance = SingletonBehaviour<RootTargetCameraMode>.Instance; }
                    catch (Exception ex) { instance = SingletonBehaviour<RootTargetCameraMode>.Instance; 
                    if (diagnostics >= DiagnosticMode.ultra) { Debug.Log("CloseViewMode: catch ex:" + ex.Message); } }
                }

                if (minFibackup == Vector3.zero)
                {
                    object oMinFI = minFI.GetValue(SingletonBehaviour<RootTargetCameraMode>.Instance);
                    object oMaxFi = maxFI.GetValue(SingletonBehaviour<RootTargetCameraMode>.Instance);         
                    minFibackup = (Vector3)oMinFI;
                    maxFibackup = (Vector3)oMaxFi;
                }

                Vector3 minZoomPos = new Vector3(0, 0, -0.8f);
                Vector3 maxZoomPos = new Vector3(0, 0, -3);
                minFI.SetValue(SingletonBehaviour<RootTargetCameraMode>.Instance, minZoomPos);
                maxFI.SetValue(SingletonBehaviour<RootTargetCameraMode>.Instance, maxZoomPos);
            }
        }

        public void CViewOff(CreatureGuid a = new CreatureGuid())
        {
            if (diagnostics >= DiagnosticMode.low) { Debug.Log("CloseViewMode: CViewOff"); }                  

            if (!GMCloseViewActive && CloseViewActive)
            {
                CloseViewActive = false;
                minFI.SetValue(SingletonBehaviour<RootTargetCameraMode>.Instance, minFibackup);
                maxFI.SetValue(SingletonBehaviour<RootTargetCameraMode>.Instance, maxFibackup);
            }           
        }
        public void CViewOnGM(CreatureGuid a)
        {
            if (diagnostics >= DiagnosticMode.high) { Debug.Log("CloseViewMode: CViewOnGM"); }            
            AssetDataPlugin.SendInfo(CloseViewMode.Guid + ".CloseON", null);

        }
        public void CViewOffGM(CreatureGuid a)
        {
            if (diagnostics >= DiagnosticMode.high) { Debug.Log("CloseViewMode: CViewOffGM"); }
            AssetDataPlugin.SendInfo(CloseViewMode.Guid + ".CloseOFF", null);
        }
        private void CallbackCloseon(AssetDataPlugin.DatumChange change)
        {
            if (!LocalClient.IsInGmMode)
            {
                CViewOn();
                GMCloseViewActive = true;
            }
        }
        private void CallbackCloseoff(AssetDataPlugin.DatumChange change)
        {
            if (!LocalClient.IsInGmMode)
            {
                CViewOff();
                GMCloseViewActive = false;
            }
        }        
    }
}

