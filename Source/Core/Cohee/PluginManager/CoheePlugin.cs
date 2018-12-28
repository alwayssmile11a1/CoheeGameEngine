using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cohee
{
    /// <summary>
    /// A wrapper for an assembly
    /// </summary>
    public abstract class CoheePlugin
    {
        private bool disposed = false;
        private Assembly assembly = null;
        private string asmName = null;
        private string filePath = null;
        private int fileHash = 0;

        public bool Disposed
        {
            get { return this.disposed; }
        }
        public Assembly PluginAssembly
        {
            get { return this.assembly; }
        }
        public string AssemblyName
        {
            get { return this.asmName; }
        }
        public string FilePath
        {
            get { return this.filePath; }
            internal set { this.filePath = value; }
        }
        public int FileHash
        {
            get { return this.fileHash; }
            internal set { this.fileHash = value; }
        }

        protected CoheePlugin()
        {
            this.assembly = this.GetType().GetTypeInfo().Assembly;
            this.asmName = this.assembly.GetShortAssemblyName();
        }
        internal void Dispose()
        {
            if (this.disposed) return;

            this.OnDisposePlugin();

            this.disposed = true;
        }
        /// <summary>
        /// Called when unloading / disposing the plugin.
        /// </summary>
        protected virtual void OnDisposePlugin() { }
    }
}
