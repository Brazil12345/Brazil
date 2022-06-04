using BepInEx;
using System;
using UnityEngine;
using Utilla;
using UnityEngine.XR;
using System.IO;
using System.Reflection;

namespace MonkeGoToBrazil
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;

        GameObject Brazil;

        GameObject _BrazilFlag;

        GameObject HandR;

        private readonly XRNode rNode = XRNode.RightHand;

        bool isgrip;

        bool cangrip = true;

        public float expForce = 500, radius = 5;

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeGoToBrazil.Assets.brazil");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            GameObject BrazilGameObject = bundle.LoadAsset<GameObject>("_Brazil");
            Brazil = Instantiate(BrazilGameObject);
            
            Stream _str = Assembly.GetExecutingAssembly().GetManifestResourceStream("MonkeGoToBrazil.Assets.brazilflag");
            AssetBundle _bundle = AssetBundle.LoadFromStream(_str);
            GameObject BrazilFlagObject = _bundle.LoadAsset<GameObject>("BrazilFlag");
            _BrazilFlag = Instantiate(BrazilFlagObject);
            
            HandR = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/");
            _BrazilFlag.transform.SetParent(HandR.transform, false);
            _BrazilFlag.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
            _BrazilFlag.transform.localRotation = Quaternion.Euler(0f, 335f, 90f);
            _BrazilFlag.transform.localPosition = new Vector3(-0.1f, -0.1f, -0.02f);
        }

        void Update()
        {
            InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isgrip);

            if (isgrip)
            {
                if (inRoom && cangrip)
                {

                    Brazil.GetComponent<AudioSource>().Play();
                    Debug.Log("YOUR GOING TO BRAZIL");
                    Rigidbody PlayerRigidbody = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>();
                    PlayerRigidbody.AddExplosionForce(2500f * expForce, PlayerRigidbody.position, 5 + (0.75f * radius));
                    cangrip = false;
                }
            }
            else
            {
                // This is where grip is not pressed so here we will make it so u can grip
                cangrip = true;
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}
