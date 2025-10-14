using System.Windows.Input;

using Sharpnado.Tasks;

namespace Mvvm.Flux.Maui.Presentation.Commands
{
    public abstract class TaskMonitorCommandBase : BindableBase, ICommand
    {
        private readonly Func<bool> _canExecute;

        private bool _canBeExecuted;

        protected TaskMonitorCommandBase(Func<bool> canExecute = null)
        {
            _canExecute = canExecute;

            if (_canExecute != null)
            {
                CanBeExecuted = CanExecute(null);
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanBeExecuted
        {
            get => _canBeExecuted;
            set => SetProperty(ref _canBeExecuted, value);
        }

        protected abstract bool IsExecuting { get; }

        public abstract void Execute(object parameter);

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && (_canExecute?.Invoke() ?? true);
        }

        public void RaiseCanExecuteChanged()
        {
            CanBeExecuted = CanExecute(null);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class TaskMonitorCommand : TaskMonitorCommandBase
    {
        private readonly Func<ITaskMonitor> _taskSource;

        public TaskMonitorCommand(Func<ITaskMonitor> taskSource, Func<bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = taskSource;
        }

        public TaskMonitorCommand(Func<Task> taskSource, Func<bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = () => TaskMonitor.Create(taskSource);
        }

        protected ITaskMonitor Monitor { get; set; }

        protected override bool IsExecuting => Monitor != null && !Monitor.IsCompleted;

        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            Monitor = _taskSource.Invoke();
        }
    }

    public class TaskMonitorCommand<T> : TaskMonitorCommandBase
    {
        private readonly Func<T, ITaskMonitor> _taskSource;

        public TaskMonitorCommand(Func<T, ITaskMonitor> taskSource, Func<bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = taskSource;
        }

        public TaskMonitorCommand(Func<T, Task> taskSource, Func<bool> canExecute = null)
            : base(canExecute)
        {
            _taskSource = (parameter) => TaskMonitor.Create(taskSource(parameter));
        }

        protected ITaskMonitor Monitor { get; set; }

        protected override bool IsExecuting => Monitor != null && !Monitor.IsCompleted;

        public override void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            Monitor = _taskSource.Invoke((T)parameter);
        }
    }
}
