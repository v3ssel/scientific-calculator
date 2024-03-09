using System;
using System.Collections.Generic;
using Avalonia.Media;
using OxyPlot;
using ReactiveUI;

namespace ScientificCalculator.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        #region ColorsProperties

        private IBrush _foregroundBrush = Brushes.Black;
        public IBrush ForegroundBrush
        {
            get => _foregroundBrush;
            set => this.RaiseAndSetIfChanged(ref _foregroundBrush, value);
        }

        private IBrush _firstBackgroundBrush = Brushes.White;
        public IBrush FirstBackgroundBrush
        {
            get => _firstBackgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _firstBackgroundBrush, value);
        }

        private IBrush _secondBackgroundBrush = Brushes.Silver;
        public IBrush SecondBackgroundBrush
        {
            get => _secondBackgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _secondBackgroundBrush, value);
        }

        private Avalonia.Media.Color _secondBackgroundColor = Colors.Silver;
        public Avalonia.Media.Color SecondBackgroundColor
        {
            get => _secondBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _secondBackgroundColor, value);
        }

        #endregion

        #region DataProperties

        private string _expressionInput = string.Empty;
        public string ExpressionInput
        {
            get => _expressionInput;
            set => this.RaiseAndSetIfChanged(ref _expressionInput, value);
        }
        
        private string _dxMin = "-100";
        public string DxMin
        {
            get => _dxMin;
            set => this.RaiseAndSetIfChanged(ref _dxMin, value);
        }

        private string _dxMax = "100";
        public string DxMax
        {
            get => _dxMax;
            set => this.RaiseAndSetIfChanged(ref _dxMax, value);
        }

        #endregion

        public IList<DataPoint> Points { get; set; }

        public GraphViewModel()
        {
            this.Points = new List<DataPoint>();
            var r = new Random(13);
            for (int i = 0; i < 10; i++)
            {
                this.Points.Add(new DataPoint(i, r.NextDouble()));
            }
        }

        public void PlotGraphCommand()
        {

        }

        #region EventHandlers

        public void ForegroundBrushChangedAction(IBrush brush)
        {
            ForegroundBrush = brush;
        }

        public void FirstBackgroundBrushChangedAction(IBrush brush)
        {
            FirstBackgroundBrush = brush;
        }

        public void SecondBackgroundBrushChangedAction(IBrush brush)
        {
            SecondBackgroundBrush = brush;
            SecondBackgroundColor = Color.Parse(brush.ToString()!);
        }

        #endregion
    }
}