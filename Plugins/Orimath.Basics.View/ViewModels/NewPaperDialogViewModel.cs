using System.Windows.Input;
using Orimath.Controls;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.Basics.View.ViewModels
{
    public class NewPaperDialogViewModel : NotifyPropertyChanged
    {
        private readonly IMessenger _messenger;
        private readonly IDispatcher _dispatcher;
        private readonly NewPaperExecutor _executor;

        public NewPaperDialogViewModel(IMessenger messenger, IDispatcher dispatcher, NewPaperExecutor executor)
        {
            _messenger = messenger;
            _dispatcher = dispatcher;
            _executor = executor;
            ExecuteCommand = Prop.True.ToCommand(_ => Execute());
            CloseCommand = messenger.CloseDialogCommand;

            if (_executor.NewPaperType.IsSquare)
            {
                _isSquareSelected = true;
            }
            else if (_executor.NewPaperType is NewPaperType.Rectangle rectangle)
            {
                _isRectangleSelected = true;
                _width = rectangle.width;
                _height = rectangle.height;
            }
            else if (_executor.NewPaperType is NewPaperType.RegularPolygon polygon)
            {
                _isPolygonSelected = true;
                _numberOfPolygon = polygon.number;
            }
        }

        private bool _isSquareSelected;
        public bool IsSquareSelected
        {
            get => _isSquareSelected;
            set
            {
                if (SetValue(ref _isSquareSelected, value) && value)
                    SetSquare();
            }
        }

        private bool _isRectangleSelected;
        public bool IsRectangleSelected
        {
            get => _isRectangleSelected;
            set
            {
                if (SetValue(ref _isRectangleSelected, value) && value)
                    SetRectangle();
            }
        }

        private bool _isPolygonSelected;
        public bool IsPolygonSelected
        {
            get => _isPolygonSelected;
            set
            {
                if (SetValue(ref _isPolygonSelected, value) && value)
                    SetPolygon();
            }
        }

        private double _width = 1.0;
        public double Width
        { 
            get => _width;
            set
            {
                if (SetValue(ref _width, value) && IsRectangleSelected)
                    SetRectangle();
            }
        }

        private double _height = 1.0;
        public double Height
        {
            get => _height;
            set
            {
                if (SetValue(ref _height, value) && IsRectangleSelected)
                    SetRectangle();
            }
        }

        private int _numberOfPolygon = 3;
        public int NumberOfPolygon
        { 
            get => _numberOfPolygon;
            set
            {
                if (SetValue(ref _numberOfPolygon, value) && IsPolygonSelected)
                    SetPolygon();
            }
        }

        private void SetSquare()
        {
            _executor.NewPaperType = NewPaperType.Square;
        }

        private void SetRectangle()
        {
            _executor.NewPaperType = NewPaperType.NewRectangle(Width, Height);
        }

        private void SetPolygon()
        {
            _executor.NewPaperType = NewPaperType.NewRegularPolygon(NumberOfPolygon);
        }

        public async void Execute()
        {
            await _dispatcher.OnBackgroundAsync(_executor.NewPaper);
            _messenger.CloseDialog();
        }

        public ICommand ExecuteCommand { get; }

        public ICommand CloseCommand { get; }
    }
}
