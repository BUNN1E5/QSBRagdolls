using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;


namespace QSBRagdolls
{
    public class QSBRagdolls : ModBehaviour{
        public static QSBRagdolls instance;

        private void Start(){
            instance = this;
            ModHelper.Console.WriteLine($"{nameof(QSBRagdolls)} is loaded!", MessageType.Success);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

    }
}
