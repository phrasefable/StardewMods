using StardewModdingAPI;

namespace PhraseLib {

    public class Logging {
        public static void Trace(string msg) {
            Mod.Monitor.Log();
        }
    }

}