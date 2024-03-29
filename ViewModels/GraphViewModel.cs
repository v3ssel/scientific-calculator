using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Data;
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

        private Avalonia.Media.Color _secondBackgroundColor = Colors.Silver;
        public Avalonia.Media.Color SecondBackgroundColor
        {
            get => _secondBackgroundColor;
            set => this.RaiseAndSetIfChanged(ref _secondBackgroundColor, value);
        }

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

        public ObservableCollection<DataPoint> Points { get; private set; }

        
        private readonly ICalculationService CalculationService;

        public GraphViewModel(ICalculationService calculationService)
        {
            CalculationService = calculationService;
            Points = new ObservableCollection<DataPoint>();
        }

        public void PlotGraphCommand(TextBox expression_box)
        {
            CalculationStatus status = CalculationStatus.GRAPH;

            try
            {
                CheckDx(DxMax);
                CheckDx(DxMin);

                var x_min = Convert.ToInt32(DxMin);
                var x_max = Convert.ToInt32(DxMax);
                (x_min, x_max) = (Math.Min(x_min, x_max), Math.Max(x_min, x_max));

                var range_result = CalculationService.CalculateRange(x_min, x_max, ExpressionInput);

                Points.Clear();

                int shift = x_min < 0 ? Math.Abs(x_min) : -x_min;
                for (int x = x_min; x < x_max; x++)
                {
                    Points.Add(new DataPoint(x, range_result[x + shift]));
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

        public static void CheckDx(string value)
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