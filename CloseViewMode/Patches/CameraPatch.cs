﻿using System;
using UnityEngine;
using HarmonyLib;
using BepInEx;



namespace XJ_Nekomancer
{
    public partial class CloseViewMode : BaseUnityPlugin
    {        
        [HarmonyPatch(typeof(CameraController), "Update")]
        public static class PatchCameraControllerUpdate
        {
            public static void Postfix()
            {              
                if (CloseViewActive)
                {
                    CreatureBoardAsset asset;
                    CreaturePresenter.TryGetAsset(LocalClient.SelectedCreatureId, out asset);
                    if (asset != null)
                    {      
                        if (disablecam) { CameraController.ToggleCameraMovement(true);disablecam= false ; }
                        double yrotation = asset.HookHead.rotation.eulerAngles.y;
                        Vector3 headPosition = asset.HookHead.position;
                        headPosition.x = headPosition.x - float.Parse(Math.Cos(yrotation * Math.PI / 180).ToString()) / 3;
                        headPosition.z = headPosition.z + float.Parse(Math.Sin(yrotation * Math.PI / 180).ToString()) / 3;
                        headPosition.y = headPosition.y - 0.3f;
                        CameraController.MoveToPosition(headPosition, true, false, false);                                            
                    }
                    else
                    {
                        if (!disablecam)
                        {
                            CameraController.ToggleCameraMovement(false);
                            disablecam = true;
                        }
                    }
                }               
            }
        }
    }
}

