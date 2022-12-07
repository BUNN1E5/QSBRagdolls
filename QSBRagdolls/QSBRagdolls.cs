using System;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using QSB.Animation.Player;
using QSB.Player;
using UnityEngine;
using UnityEngine.InputSystem;


namespace QSBRagdolls
{
    public class QSBRagdolls : ModBehaviour{
        public static QSBRagdolls instance;

        private void Start(){
            instance = this;
            ModHelper.Console.WriteLine($"{nameof(QSBRagdolls)} is loaded!", MessageType.Success);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            
        }

        private void Update(){
            if (GetKeyDown(Key.P)){
                CreateRagdoll(QSBPlayerManager.LocalPlayer);
            }
        }

        public static void CreateRagdoll(PlayerInfo info){
            if (info.Body == null)
                return;

            string isRemote = info.IsLocalPlayer ? "" : "REMOTE_";


            var body = info.Body.transform.Find(isRemote + "Traveller_HEA_Player_v2");
            var root = GameObject.Instantiate(body.gameObject, body.transform.position, body.transform.rotation);

            //root.transform.parent = info.TransformSync.SectorDetector.GetClosestSector().Transform;

            Destroy(root.GetComponent<PlayerAnimController>());
            Destroy(root.GetComponent<Animator>());
            Destroy(root.GetComponent<AnimatorMirror>());

            root.AddComponent<CapsuleCollider>();
            root.AddComponent<OWCapsuleCollider>();
            var owRigidbody = root.AddComponent<OWRigidbody>();
            root.AddComponent<ImpactSensor>();

            var ss = root.AddComponent<SphereShape>();
            
            var dfd = root.AddComponent<DynamicForceDetector>();
            var afd = root.AddComponent<AlignmentForceDetector>();
            var fa = root.AddComponent<ForceApplier>();

            owRigidbody.OnValidate();

            owRigidbody._simulateInSector = info.TransformSync.SectorDetector.GetClosestSector().AttachedObject;
        }

        private bool GetKeyDown(Key keyCode) {
            return Keyboard.current[keyCode].wasPressedThisFrame;
        }

    }
}
