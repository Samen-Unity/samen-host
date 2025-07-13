using SamenHost.Core;
using SamenHost.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamenHost.Chat;

namespace SamenHost.Plugins
{
    /// <summary>
    /// An instance of a Samen plugin
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// Runs when the plugin is loaded before samen starts up.
        /// </summary>
        public virtual void OnLoad()
        {
            
        }
    }
}
