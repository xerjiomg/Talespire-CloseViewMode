using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace TemplatePlugin.Patches
{
    internal class TabPlayer
    {
        [HarmonyPatch(typeof(LocalClient), "SetSelectedCreatureId")]
        public static class PatchLocalClientSetSelectedCreatureId
        {
            public static bool Prefix()
            {
                return true;
            }

            public static void Postfix()
            {
                Debug.Log("TabPlayers: Patch: OnCreatureDataChanged");
                //MonoBehaviour.Instantiate<QuadGridHandler>().ShowGrid(new Vector3 (5f,5f,5f));              
                //QuadGridHandler.ShowGrid(new Vector3(10, 10, 10));
                // BoardToolManager.PreviewGrid.ShowGrid(true, 5f, Color.blue, Color.grey); funciona, pero es una fija

                //Shader.SetGlobalFloat("G_ShowOverride", Convert.ToInt32(false));
                //PlaceableLightManager.ShowHiddenLights = true;
                //if (GmOverlayManager.IsEnabled) { GmOverlayManager.Close(); } else { GmOverlayManager.Open(); }

                //ShowOverride.IsActive = true;
                //if (ShowOverride.IsActive == false)
                //{ 
                //ShowOverride.IsActive = true;                
                //PlaceableLightManager.ShowHiddenLights = true;
                //Shader.SetGlobalFloat("G_ShowOverride", 0);
                //}
                //else
                //{ 
                //    ShowOverride.IsActive = false;
                //    PlaceableLightManager.ShowHiddenLights = false;
                //    Shader.SetGlobalFloat("G_ShowOverride", 0);
                //}

            }
        }     
    }
}

