using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;

namespace Lab05.Models
{
    public class LabProcess
    {
        private Process _process;
        private float _cpu;
        private ObservableCollection<LabModule> _modules;
        private ObservableCollection<LabThread> _threads;

        public string Name => _process.ProcessName;
        
        public int Id => _process.Id;

        public float Cpu => _cpu;

        public long Memory => _process.PrivateMemorySize64/1024;

        public bool Active => _process.Responding;

        public int ThreadsNumber => _process.Threads.Count;

        public string User => _process.MachineName;
        

        public string Path
        {
            get
            {
                try
                {
                    return _process.MainModule.FileName;
                }
                catch (Exception e)
                {
                    return "Access denied";
                }
            }
        }
        //slow
        private static string GetProcessOwner(Process process)
        {
            ObjectQuery objQuery32 = new ObjectQuery("Select * From Win32_Process where ProcessId='" + process.Id + "'");
            ManagementObjectSearcher mos32 = new ManagementObjectSearcher(objQuery32);
            string processOwner = string.Empty;
            foreach (ManagementObject mo in mos32.Get())
            {
                string[] s = new string[2];
                mo.InvokeMethod("GetOwner", (object[])s);
                try
                {
                    processOwner = s[0];
                }
                catch (Exception)
                {
                    //ignore
                }
                break;
            }
            return processOwner;
        }
        
        public DateTime Launch
        {
            get
            {
                try
                {
                    return _process.StartTime;
                }
                catch (Exception e)
                {
                    return new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day);
                }
            }
        }
        

        internal LabProcess(Process process)
        {
            this._process = process;
            // it doesnt work because of my bad code(((
            /*PerformanceCounter ProcessCPUCounter = new PerformanceCounter();
            ProcessCPUCounter.CategoryName = "Process";
            ProcessCPUCounter.CounterName = "% Processor Time";
            ProcessCPUCounter.InstanceName = "TestServiceName"; 
            ProcessCPUCounter.ReadOnly = true;
            ProcessCPUCounter.NextValue();*/
        }
        




        public ObservableCollection<LabModule> Modules
        {
            get
            {
                if (_modules == null)
                {
                    RefreshModules();
                }

                return _modules;
            }
        }

       internal void RefreshModules()
        {
           _modules = new ObservableCollection<LabModule>();
           try
           {
               foreach (ProcessModule module in _process.Modules)
               {
                   _modules.Add(new LabModule(module));
               }
           }
           catch (Exception e)
           {
               
           }
        }

        public ObservableCollection<LabThread> Threads
        {
            get
            {
                if (_threads == null)
                {
                    RefreshThreads();
                }

                return _threads;
            }
        }

        internal void RefreshThreads()
        {
           _threads = new ObservableCollection<LabThread>();
           try
           {
               foreach (ProcessThread thread in _process.Threads)
               {
                   _threads.Add(new LabThread(thread));
               }
           }
           catch (Exception e)
           {
               
           }
        }
    }
}