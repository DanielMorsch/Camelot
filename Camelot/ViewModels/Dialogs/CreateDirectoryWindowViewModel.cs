using System.Windows.Input;
using ReactiveUI;

namespace Camelot.ViewModels.Dialogs
{
    public class CreateDirectoryWindowViewModel : DialogViewModelBase<string>
    {
        private string _directoryName;

        public string DirectoryName
        {
            get => _directoryName;
            set => this.RaiseAndSetIfChanged(ref _directoryName, value);
        }

        public ICommand CreateCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateDirectoryWindowViewModel()
        {
            var canCreate = this.WhenAnyValue(x => x.DirectoryName,
                name => !string.IsNullOrWhiteSpace(name));

            CreateCommand = ReactiveCommand.Create(CreateDirectory, canCreate);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void Cancel()
        {
            Close();
        }

        private void CreateDirectory()
        {
            Close(_directoryName);
        }
    }
}