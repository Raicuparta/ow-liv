﻿using System.IO;
using LIV.SDK.Unity;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using Valve.VR;

namespace OwLiv
{
    public class ModTemplate : ModBehaviour
    {
        private LIV.SDK.Unity.LIV liv;
        private AssetBundle shaderBundle;
        
        private void Start()
        {
            shaderBundle = LoadBundle("liv-shaders");
            SDKShaders.LoadFromAssetBundle(shaderBundle);
        }
        
        private void Update()
        {
            if (OWInput.IsNewlyPressed(InputLibrary.map))
            {
                SetUpLiv();
            }
        }

        private void SetUpLiv()
        {
            ModHelper.Console.WriteLine($"Setting up LIV...");
            
            if (liv)
            {
                ModHelper.Console.WriteLine($"LIV instance already exists. Destroying it.");
                Destroy(liv);
            }
            
            var camera = Locator.GetPlayerCamera()._mainCamera;
            var cameraParent = camera.transform.parent;

            var steamVrPose = cameraParent.GetComponentInChildren<SteamVR_Behaviour_Pose>();
            
            var stage = steamVrPose ? steamVrPose.transform.parent : cameraParent;

            liv = cameraParent.gameObject.AddComponent<LIV.SDK.Unity.LIV>();
            liv.stage = stage;
            liv.HMDCamera = camera;
            liv.fixPostEffectsAlpha = true;
            liv.spectatorLayerMask = camera.cullingMask;
            liv.spectatorLayerMask &= ~(1 << LayerMask.NameToLayer("UI"));

            ModHelper.Console.WriteLine($"LIV created successfully with stage {stage}");
        }
        
        private AssetBundle LoadBundle(string assetName)
        {
            var bundle = AssetBundle.LoadFromFile($"{ModHelper.Manifest.ModFolderPath}/Assets/{assetName}");

            if (bundle == null)
            {
                ModHelper.Console.WriteLine($"Failed to load AssetBundle {assetName}", MessageType.Error);
            }

            return bundle;
        }
    }
}
