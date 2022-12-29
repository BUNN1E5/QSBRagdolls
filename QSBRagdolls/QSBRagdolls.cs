using System;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using QSB.Animation.Player;
using QSB.Player;
using QSB.WorldSync;
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

            if (GetKeyDown(Key.L)){
                CreateRagdoll(QSBPlayerManager.LocalPlayer);
            }
        }

        public static void CreateRagdoll(PlayerInfo info){
            if (info.Body == null)
                return;

            string isRemote = info.IsLocalPlayer ? "" : "REMOTE_";

            var playerColl = info.Body.GetComponent<Collider>();
            
            var body = info.Body.transform.Find(isRemote + "Traveller_HEA_Player_v2");
            var body_copy = GameObject.Instantiate(body.gameObject, body.transform.position, body.transform.rotation);
            
            var p_fld = info.Body.GetComponentInChildren<DynamicFluidDetector>();
            
            // root.layer = LayerMask.NameToLayer("PhysicalDetector");
            // root.tag = "DynamicPropDetector";
            
            Destroy(body_copy.GetComponent<PlayerAnimController>());
            Destroy(body_copy.GetComponent<Animator>());
            Destroy(body_copy.GetComponent<AnimatorMirror>());

            // var capsuleCollider = root.AddComponent<CapsuleCollider>();
            // capsuleCollider.height = 17f;
            // capsuleCollider.radius = 2f;
            // capsuleCollider.center = new Vector3(0, 8f, 0);

            var owRigidbody = body_copy.AddComponent<OWRigidbody>();
            //
            //root.AddComponent<ImpactSensor>();
            //
            // var dfd = root.AddComponent<DynamicForceDetector>();
            // var fld = root.AddComponent<DynamicFluidDetector>();
            //
            //
            // fld._dragFactor = p_fld._dragFactor;
            // fld._buoyancy = p_fld._buoyancy;
            // fld._angularDragFactor = p_fld._angularDragFactor;
            

            owRigidbody.OnValidate();
            
            owRigidbody.SetVelocity(info.Body.GetAttachedOWRigidbody().GetPointVelocity(info.Body.transform.position));
            owRigidbody._simulateInSector = (Sector)((IWorldObject)info.TransformSync.SectorDetector.GetClosestSector()).AttachedObject;

            var parentObject = body_copy.transform.Find("Traveller_Rig_v01:Traveller_Trajectory_Jnt");

            Transform[] children = body_copy.GetComponentsInChildren<Transform>();
            
            var mass = Locator.GetPlayerBody().GetMass() / children.Length;

            foreach (Transform child in children)
            {
                //TODO :: GENERATE A BRAND NEW SKELETON
                //And just set the positions of the joints to the skeleton joints
                
                
                // Don't add a rigidbody or CharacterJoint to the parent object
                if (child == parentObject.transform)
                    continue;

                if (!child.transform.name.Contains("Traveller_Rig_v01")) //Oof a string compare
                    continue;

                var parent = child.transform.parent;

                // Add a rigidbody to the child object
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.mass = mass;
                
                // Add a CharacterJoint to the child object
                //CharacterJoint cj = child.gameObject.AddComponent<CharacterJoint>();
                HingeJoint cj = child.gameObject.AddComponent<HingeJoint>();
                cj.connectedBody = parent.GetComponent<Rigidbody>();
                cj.enableCollision = false;

                var cc = child.gameObject.AddComponent<CapsuleCollider>();
                cc.height = 10 * Vector3.Distance(cc.transform.position, parent.position) / 2;
                cc.radius = Mathf.Sqrt(cc.height);
                
                Physics.IgnoreCollision(playerColl, cc);
                Physics.IgnoreCollision(playerColl, cc);
                
                var owr = child.gameObject.AddComponent<OWRigidbody>();
                owr.OnValidate();

                child.GetComponent<CenterOfTheUniverseOffsetApplier>().enabled = false;

                var ic = child.gameObject.AddComponent<IgnoreCollision>();
                ic._ignorePlayer = true;

                var c_dfd = cc.gameObject.AddComponent<DynamicForceDetector>();
                var c_fld = cc.gameObject.AddComponent<DynamicFluidDetector>();

                c_fld._dragFactor = p_fld._dragFactor;
                c_fld._buoyancy = p_fld._buoyancy;
                c_fld._angularDragFactor = p_fld._angularDragFactor;

                owr.transform.parent = parent;
                
                owr.SetVelocity(info.Body.GetAttachedOWRigidbody().GetVelocity());
                owr.SetAngularVelocity(info.Body.GetAttachedOWRigidbody().GetAngularVelocity());
                owr._simulateInSector = (Sector)((IWorldObject)info.TransformSync.SectorDetector.GetClosestSector()).AttachedObject;

                child.gameObject.layer = LayerMask.NameToLayer("PhysicalDetector");
                child.tag = "DynamicPropDetector";
                
                // Set the anchor and connected anchor of the CharacterJoint
                // to the center of the child object
            }

            //parentObject.GetComponent<CenterOfTheUniverseOffsetApplier>().enabled = true;
        }

        private bool GetKeyDown(Key keyCode) {
            return Keyboard.current[keyCode].wasPressedThisFrame;
        }
        
        private bool GetKey(Key keyCode) {
            return Keyboard.current[keyCode].isPressed;
        }

    }
}
