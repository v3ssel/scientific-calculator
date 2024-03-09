using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using OxyPlot;
using ReactiveUI;
using ScientificCalculator.Models;
using ScientificCalculator.Services.Calculation;

namespace ScientificCalculator.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        public delegate void GraphPlottedEventHander(CalculationStatus status, HistoryRecord record);
        public event GraphPlottedEventHander? GraphPlottedEvent;

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
            set 
            {
                CheckDx(value);
                this.RaiseAndSetIfChanged(ref _dxMin, value);
            }
        }

        private string _dxMax = "100";
        public string DxMax
        {
            get => _dxMax;
            set 
            {
                CheckDx(value);
                this.RaiseAndSetIfChanged(ref _dxMax, value);
            }
        }

        #endregion

        public IList<DataPoint> Points { get; set; }

        
        private readonly ICalculationService CalculationService;

        public GraphViewModel(ICalculationService calculationService)
        {
            CalculationService = calculationService;
            Points = new List<DataPoint>();
        }

        // design
        public GraphViewModel()
        {
            CalculationService = new DllCalculationService();
            Points = new List<DataPoint>();
        }

        public void PlotGraphCommand(TextBox expression_box)
        {
            CalculationStatus status = CalculationStatus.GRAPH;

            try
            {
                var x_min = Convert.ToInt32(DxMin);
                var x_max = Convert.ToInt32(DxMax);

                var range_result = CalculationService.CalculateRange(Math.Min(x_min, x_max), Math.Max(x_min, x_max), ExpressionInput);
                
                Points = new List<DataPoint>();
                for (int i = 0; i < range_result.Count; i++)
                {
                    Points.Add(new DataPoint(i, range_result[i]));
                }

                DataValidationErrors.ClearErrors(expression_box);

            }
            catch (Exception e)
            {
                status = CalculationStatus.ERROR;
                
                DataValidationErrors.SetError(expression_box, new DataValidationException($"Error appeared during calculation.\n{e.Message}\nCheck your expression."));
            }
            finally
            {
                GraphPlottedEvent?.Invoke(status,
                    new HistoryRecord()
                    {
                        CalculationTime = DateTime.Now,
                        Expression = ExpressionInput,
                        Answer = "",
                        XValue = "",
                    });
            }

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

        private void CheckDx(string value)
        {
            if (!int.TryParse(value, out var res))
            {
                throw new DataValidationException("value must be a number");
            }

            if (res > 1e6)
            {
                throw new DataValidationException("value must be less than 1e6");
            }

            if (res < -1e6)
            {
                throw new DataValidationException("value must be more than -1e6");
            }
        }
    }
}