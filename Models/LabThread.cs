using System;
using System.Diagnostics;

namespace Lab05.Models
{
    public class LabThread
    {
        private readonly ProcessThread _thread;
        private readonly ThreadState _threadState;
        private readonly DateTime _launch;
        private readonly int _id;

        public int Id => _id;

        public ThreadState ThreadState => _threadState;

        public DateTime Launch => _launch;

        internal LabThread(ProcessThread thread)
        {
            this._thread = thread;
            this._threadState = _thread.ThreadState;
            this._launch = _thread.StartTime;
            this._id = _thread.Id;
        }
    }
}