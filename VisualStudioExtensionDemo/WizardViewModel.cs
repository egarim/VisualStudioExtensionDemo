using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace VisualStudioExtensionDemo
{
    /// <summary>
    /// Base class for view models implementing INotifyPropertyChanged.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// Simple relay command implementation.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object parameter) => execute(parameter);
    }

    /// <summary>
    /// Main wizard view model that manages the wizard steps.
    /// </summary>
    public class WizardViewModel : ViewModelBase
    {
        private int currentStepIndex;
        private readonly List<WizardStepViewModel> steps;

        public WizardViewModel()
        {
            steps = new List<WizardStepViewModel>
            {
                new WelcomeStepViewModel(),
                new ConfigurationStepViewModel(),
                new SummaryStepViewModel()
            };

            currentStepIndex = 0;
            UpdateCurrentStep();

            PreviousCommand = new RelayCommand(_ => GoToPreviousStep(), _ => CanGoToPreviousStep());
            NextCommand = new RelayCommand(_ => GoToNextStep(), _ => CanGoToNextStep());
        }

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public string CurrentStepTitle => CurrentStep?.Title ?? string.Empty;
        public string CurrentStepDescription => CurrentStep?.Description ?? string.Empty;
        public object CurrentStepContent => CurrentStep;
        public string NextButtonText => IsLastStep ? "Finish" : "Next";

        private WizardStepViewModel CurrentStep => currentStepIndex >= 0 && currentStepIndex < steps.Count 
            ? steps[currentStepIndex] 
            : null;

        private bool IsLastStep => currentStepIndex == steps.Count - 1;

        private bool CanGoToPreviousStep() => currentStepIndex > 0;

        private bool CanGoToNextStep() => CurrentStep?.CanMoveNext() ?? false;

        private void GoToPreviousStep()
        {
            if (CanGoToPreviousStep())
            {
                currentStepIndex--;
                UpdateCurrentStep();
            }
        }

        private void GoToNextStep()
        {
            if (!CanGoToNextStep())
                return;

            if (IsLastStep)
            {
                // Finish the wizard
                var window = System.Windows.Application.Current.Windows
                    .OfType<WizardWindow>()
                    .FirstOrDefault();
                
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            else
            {
                currentStepIndex++;
                UpdateCurrentStep();
                UpdateSummaryIfNeeded();
            }
        }

        private void UpdateCurrentStep()
        {
            OnPropertyChanged(nameof(CurrentStepTitle));
            OnPropertyChanged(nameof(CurrentStepDescription));
            OnPropertyChanged(nameof(CurrentStepContent));
            OnPropertyChanged(nameof(NextButtonText));
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateSummaryIfNeeded()
        {
            if (CurrentStep is SummaryStepViewModel summary)
            {
                var configStep = steps.OfType<ConfigurationStepViewModel>().FirstOrDefault();
                if (configStep != null)
                {
                    summary.UpdateFromConfiguration(configStep);
                }
            }
        }
    }

    /// <summary>
    /// Base class for wizard step view models.
    /// </summary>
    public abstract class WizardStepViewModel : ViewModelBase
    {
        public abstract string Title { get; }
        public abstract string Description { get; }
        public virtual bool CanMoveNext() => true;
    }

    /// <summary>
    /// Welcome step view model.
    /// </summary>
    public class WelcomeStepViewModel : WizardStepViewModel
    {
        public override string Title => "Welcome";
        public override string Description => "Step 1 of 3";
    }

    /// <summary>
    /// Configuration step view model.
    /// </summary>
    public class ConfigurationStepViewModel : WizardStepViewModel
    {
        private string projectName = "MyNewProject";
        private string projectType = "Console Application";
        private bool includeTests = true;
        private bool initializeGit = true;

        public override string Title => "Configuration";
        public override string Description => "Step 2 of 3 - Configure your project";

        public string ProjectName
        {
            get => projectName;
            set => SetProperty(ref projectName, value);
        }

        public string ProjectType
        {
            get => projectType;
            set => SetProperty(ref projectType, value);
        }

        public bool IncludeTests
        {
            get => includeTests;
            set => SetProperty(ref includeTests, value);
        }

        public bool InitializeGit
        {
            get => initializeGit;
            set => SetProperty(ref initializeGit, value);
        }

        public override bool CanMoveNext() => !string.IsNullOrWhiteSpace(ProjectName);
    }

    /// <summary>
    /// Summary step view model.
    /// </summary>
    public class SummaryStepViewModel : WizardStepViewModel
    {
        private string projectName;
        private string projectType;
        private bool includeTests;
        private bool initializeGit;

        public override string Title => "Summary";
        public override string Description => "Step 3 of 3 - Review your configuration";

        public string ProjectName
        {
            get => projectName;
            set => SetProperty(ref projectName, value);
        }

        public string ProjectType
        {
            get => projectType;
            set => SetProperty(ref projectType, value);
        }

        public bool IncludeTests
        {
            get => includeTests;
            set => SetProperty(ref includeTests, value);
        }

        public bool InitializeGit
        {
            get => initializeGit;
            set => SetProperty(ref initializeGit, value);
        }

        public void UpdateFromConfiguration(ConfigurationStepViewModel config)
        {
            ProjectName = config.ProjectName;
            ProjectType = config.ProjectType;
            IncludeTests = config.IncludeTests;
            InitializeGit = config.InitializeGit;
        }
    }
}
