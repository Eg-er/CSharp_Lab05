using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;
using Lab05.Models;
using Lab05.Tools;

namespace Lab05.ViewModels
{

    public class Lab05ViewModel : BaseViewModel
    {
        private RelayCommand<object> _endProcessCommand;
        private RelayCommand<object> _openDirectoryCommand;
        private CancellationToken _token;
        private LabProcess _selected;
        private ObservableCollection<LabProcess> _processes;
        private ObservableCollection<LabModule> _modules;
        private RelayCommand<object> _sortById;
        private RelayCommand<object> _sortByName;
        private RelayCommand<object> _sortByCpu;
        private RelayCommand<object> _sortByRam;
        private RelayCommand<object> _sortByThreads;
        private RelayCommand<object> _sortByActiveness;
        private RelayCommand<object> _sortByUser;
        private RelayCommand<object> _sortByPath;
        private RelayCommand<object> _sortByTime;
        private int _sortingType;

        public bool IsSelected => Selected != null;

        public ObservableCollection<LabModule> Modules
        {
            get => _modules;
            set => _modules = value;
        }

        public ObservableCollection<LabProcess> Processes
        {
            get => _processes;
            set
            {
                _processes = value;
            }
        }

        internal Lab05ViewModel()
        {
            _sortingType = 0;
            var tokenSource = new CancellationTokenSource();
            _token = tokenSource.Token;
            var timeFrom = TimeSpan.FromSeconds(0);
            var interval = TimeSpan.FromSeconds(5);
            RunTasks(timeFrom,interval);
        }

        public LabProcess Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
                OnPropertyChanged($"IsSelected");
            }
        }
        

        private async void RunTasks(TimeSpan timeFrom, TimeSpan interval)
        {
            await AsyncRun(ProcessesNow, timeFrom, interval, _token);
        }

        private static async Task AsyncRun(Action a,TimeSpan from,TimeSpan interval,CancellationToken token)
        {
            if (from > TimeSpan.Zero)
            {
                await Task.Delay(from, token);
            }

            while (!token.IsCancellationRequested)
            {
                a?.Invoke();
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

        private void GetProcesses()
        {
            var processes = Process.GetProcesses();
            Processes = new ObservableCollection<LabProcess>();
            foreach (var process in processes)
            {
                Processes.Add(new LabProcess(process));
            }
            Sort(Processes,_sortingType);
            OnPropertyChanged();
            OnPropertyChanged($"Processes");
        }

        private async void ProcessesNow()
        {
            await Task.Run(() =>
                {
                    GetProcesses();
                    RefreshModulesAndThreads();
                },_token);
                
        }

        private void RefreshModulesAndThreads()
        {
            foreach (var process in Processes)
            {
                process.RefreshModules();
                process.RefreshThreads();
            }
        }

        public RelayCommand<object> SortById
        {
            get { return _sortById ?? (_sortById = new RelayCommand<object>(o => Sort(o, 0))); }
        }
        public RelayCommand<object> SortByName
        {
            get { return _sortByName ?? (_sortByName = new RelayCommand<object>(o => Sort(o, 1))); }
        }
        public RelayCommand<object> SortByCpu
        {
            get { return _sortByCpu ?? (_sortByCpu = new RelayCommand<object>(o => Sort(o, 2))); }
        }
        public RelayCommand<object> SortByRam
        {
            get { return _sortByRam?? (_sortByRam = new RelayCommand<object>(o => Sort(o, 3))); }
        }
        public RelayCommand<object> SortByThreads
        {
            get { return _sortByThreads ?? (_sortByThreads = new RelayCommand<object>(o => Sort(o, 4))); }
        }
        public RelayCommand<object> SortByActiveness
        {
            get { return _sortByActiveness?? (_sortByActiveness = new RelayCommand<object>(o => Sort(o, 5))); }
        }
        public RelayCommand<object> SortByUser
        {
            get { return _sortByUser ?? (_sortByUser = new RelayCommand<object>(o => Sort(o, 6))); }
        }
        public RelayCommand<object> SortByPath
        {
            get { return _sortByPath ?? (_sortByPath = new RelayCommand<object>(o => Sort(o, 7))); }
        }
        public RelayCommand<object> SortByTime
        {
            get { return _sortByTime ?? (_sortByTime = new RelayCommand<object>(o => Sort(o, 8))); }
        }

        private  void Sort(object o, int i)
        {
            _sortingType = i;
           
            
                switch (i)
                {
                    case 0:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Id select proc);
                        OnPropertyChanged();
                        break;
                    case 1:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Name select proc);
                        OnPropertyChanged();
                        break;
                    case 2:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Cpu select proc);
                        OnPropertyChanged();
                        break;
                    case 3:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Memory select proc);
                        OnPropertyChanged();
                        break;
                    case 4:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.ThreadsNumber select proc);
                        OnPropertyChanged();
                        break;
                    case 5:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Active select proc);
                        OnPropertyChanged();
                        break;
                    case 6:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.User select proc);
                        OnPropertyChanged();
                        break;
                    case 7:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Path select proc);
                        OnPropertyChanged();
                        break;
                    default:
                        Processes = new ObservableCollection<LabProcess>(from proc in Processes orderby proc.Launch select proc);
                        OnPropertyChanged();
                        break;
                }
               
        
            
        }

        public RelayCommand<object> EndProcessCommand
        {
            get
            {
                return _endProcessCommand ?? (_endProcessCommand = new RelayCommand<object>(EndProcess));
            }
        }

        private void EndProcess(object obj)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                var process = Process.GetProcessById(Selected.Id);
                try
                {
                    process.Kill();
                    Processes.Remove(Selected);
                    GetProcesses();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        public RelayCommand<object> OpenCommand
        {
            get { return _openDirectoryCommand ?? (_openDirectoryCommand = new RelayCommand<object>(OpenDirectory)); }
        }

        private async void OpenDirectory(object obj)
        {
            await Task.Run(() =>
                {
                    var process = System.Diagnostics.Process.GetProcessById(Selected.Id);
                    try
                    {
                        string filepath = process.MainModule.FileName;
                        Process.Start("explorer.exe", filepath.Remove(filepath.LastIndexOf('\\')));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                },_token);
                
        }
    }
}