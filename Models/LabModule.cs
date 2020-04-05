using System;
using System.Diagnostics;

namespace Lab05.Models
{
    public class LabModule
    {
        private readonly ProcessModule _module;
        private readonly string _name;
        private readonly string _filepath;

        public String Name => _name;
        public String Filepath => _filepath;

        internal LabModule(ProcessModule module)
        {
            this._module = module;
            this._name = _module.ModuleName;
            this._filepath = _module.FileName;
        }
    }
}