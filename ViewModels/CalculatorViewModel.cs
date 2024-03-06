using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Media;
using ReactiveUI;

namespace ScientificCalculator.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {
        public delegate void CalculationCompleteEventHander(bool error, string expression, string? answer);
        public event CalculationCompleteEventHander? CalculationCompleteEvent;

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

        private string _expressionInput = string.Empty;
        public string ExpressionInput
        {
            get => _expressionInput;
            set => this.RaiseAndSetIfChanged(ref _expressionInput, value);
        }

        private int _expressionInputCaretIndex;

        public int ExpressionInputCaretIndex
        {
            get => _expressionInputCaretIndex;
            set => this.RaiseAndSetIfChanged(ref _expressionInputCaretIndex, value);
        }
        private int _expressionInputSelectionStart;

        public int ExpressionInputSelectionStart
        {
            get => _expressionInputSelectionStart;
            set => this.RaiseAndSetIfChanged(ref _expressionInputSelectionStart, value);
        }
        private int _expressionInputSelectionEnd;

        public int ExpressionInputSelectionEnd
        {
            get => _expressionInputSelectionEnd;
            set => this.RaiseAndSetIfChanged(ref _expressionInputSelectionEnd, value);
        }

        private string _xValue = string.Empty;
        public string XValue
        {
            get => _xValue;
            set => this.RaiseAndSetIfChanged(ref _xValue, value);
        }

        private string _answerField = string.Empty;
        public string AnswerField
        {
            get => _answerField;
            set => this.RaiseAndSetIfChanged(ref _answerField, value);
        }

        public ReactiveCommand<IClipboard, Unit> CopyInputActionCmd { get; }
        public ReactiveCommand<IClipboard, Unit> PasteInputActionCmd { get; }

        public CalculatorViewModel()
        {
            CopyInputActionCmd = ReactiveCommand.CreateFromTask<IClipboard>(CopyInputAction);
            PasteInputActionCmd = ReactiveCommand.CreateFromTask<IClipboard>(PasteInputAction);
        }

        public void CalculateBtnClicked()
        {
            AnswerField = ExpressionInput;

            CalculationCompleteEvent?.Invoke(false, ExpressionInput, AnswerField);
        }
        
        public void AllClearBtnClicked()
        {
            ExpressionInput = string.Empty;
            AnswerField = string.Empty;
        }

        public void SinBtnClicked()  => InsertAndMoveCaret("sin()", 4);
        public void CosBtnClicked()  => InsertAndMoveCaret("cos()", 4);
        public void TanBtnClicked()  => InsertAndMoveCaret("tan()", 4);
        public void LogBtnClicked()  => InsertAndMoveCaret("log()", 4);
        public void LnBtnClicked()   => InsertAndMoveCaret("ln()", 3);
        public void AsinBtnClicked() => InsertAndMoveCaret("asin()", 5);
        public void AcosBtnClicked() => InsertAndMoveCaret("acos()", 5);
        public void AtanBtnClicked() => InsertAndMoveCaret("atan()", 5);
        public void SqrtBtnClicked() => InsertAndMoveCaret("sqrt()", 5);

        public void OneBtnClicked()   => InsertAndMoveCaret("1");
        public void TwoBtnClicked()   => InsertAndMoveCaret("2");
        public void ThreeBtnClicked() => InsertAndMoveCaret("3");
        public void FourBtnClicked()  => InsertAndMoveCaret("4");
        public void FiveBtnClicked()  => InsertAndMoveCaret("5");
        public void SixBtnClicked()   => InsertAndMoveCaret("6");
        public void SevenBtnClicked() => InsertAndMoveCaret("7");
        public void EightBtnClicked() => InsertAndMoveCaret("8");
        public void NineBtnClicked()  => InsertAndMoveCaret("9");
        public void ZeroBtnClicked()  => InsertAndMoveCaret("0");
        public void DotBtnClicked()   => InsertAndMoveCaret(".");
        public void XBtnClicked()     => InsertAndMoveCaret("x");
        
        public void PlusBtnClicked()  => InsertAndMoveCaret("+");
        public void MinusBtnClicked() => InsertAndMoveCaret("-");
        public void MultBtnClicked()  => InsertAndMoveCaret("*");
        public void DivBtnClicked()   => InsertAndMoveCaret("/");
        public void PowBtnClicked()   => InsertAndMoveCaret("^");
        public void ModBtnClicked()   => InsertAndMoveCaret("mod", 3);
        public void LeftBracketBtnClicked()  => InsertAndMoveCaret("(");
        public void RightBracketBtnClicked() => InsertAndMoveCaret(")");
        
        public void CreditBtnClicked() {/* TODO */}  
        public void DepositBtnClicked() {/* TODO */}

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
        }

        private void InsertAndMoveCaret(string value, int caret_shift = 1)
        {
            ExpressionInput = ExpressionInput.Insert(ExpressionInputCaretIndex, value);
            ExpressionInputCaretIndex += caret_shift;
        }

        public void CleanInputAction()
        {
            ExpressionInput = string.Empty;
        }

        public async Task CopyInputAction(IClipboard clipboard)
        {
            var start = Math.Min(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);
            var end = Math.Max(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);

            await CopyToClipboard(clipboard, ExpressionInput, start, end);
        }
        
        public async Task PasteInputAction(IClipboard clipboard)
        {
            var start = Math.Min(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);
            var end = Math.Max(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);

            var (res, len) = await PasteFromClipboard(clipboard, ExpressionInput, ExpressionInputCaretIndex, start, end);
            
            ExpressionInput = res;
            ExpressionInputCaretIndex += len;
        }

        private static async Task CopyToClipboard(IClipboard clipboard, string value, int start = 0, int end = 0)
        {
            if (start == end)
            {
                await clipboard.SetTextAsync(value);
            }
            else
            {
                await clipboard.SetTextAsync(value[start..end]);
            }
        }

        private static async Task<(string, int)> PasteFromClipboard(IClipboard clipboard, string value, int caret = 0, int start = 0, int end = 0)
        {
            var clipboard_text = await clipboard.GetTextAsync() ?? "";
            if (string.IsNullOrEmpty(clipboard_text)) return ("", 0);
            

            if (start == end)
            {
                return (value.Insert(caret, clipboard_text), clipboard_text.Length);
            }
            
            return (value.Remove(start, end - start).Insert(start, clipboard_text), clipboard_text.Length);
        }
    }
}
