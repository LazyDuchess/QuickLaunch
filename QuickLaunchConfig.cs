using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;

namespace QuickLaunch
{
    internal class QuickLaunchConfig
    {
        public QuickLaunchMode Mode => _mode.Value;
        private ConfigEntry<QuickLaunchMode> _mode = null;
        public QuickLaunchConfig(ConfigFile configFile)
        {
            _mode = configFile.Bind(
                "General",
                "Mode",
                QuickLaunchMode.LoadLastSave,
                "What to do when starting up the game. If loading or creating a save fails, you will simply be sent to the main menu."
            );
        }
    }
}
