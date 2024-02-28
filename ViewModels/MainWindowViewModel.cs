using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using ReactiveUI;

namespace ScientificCalculator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _expressionInput = "";
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

    private string _answerField = "";
    public string AnswerField
    {
        get => _answerField;
        set => this.RaiseAndSetIfChanged(ref _answerField, value);
    }

    public void CalculateBtnAction()
    {
        AnswerField = ExpressionInput;
    }

    public void CleanInputAction()
    {
        ExpressionInput = "";
    }

    public ReactiveCommand<IClipboard, Unit> CopyInputActionCmd { get; }
    public ReactiveCommand<IClipboard, Unit> PasteInputActionCmd { get; }

    public MainWindowViewModel()
    {
        CopyInputActionCmd = ReactiveCommand.CreateFromTask<IClipboard>(CopyInputAction);
        PasteInputActionCmd = ReactiveCommand.CreateFromTask<IClipboard>(PasteInputAction);
    }

    public async Task CopyInputAction(IClipboard clipboard)
    {
        if (ExpressionInputSelectionStart == ExpressionInputSelectionEnd)
        {
            await clipboard.SetTextAsync(ExpressionInput);
        }
        else
        {
            var start = Math.Min(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);
            var end = Math.Max(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);

            await clipboard.SetTextAsync(ExpressionInput[start..end]);
        }
    }
    
    public async Task PasteInputAction(IClipboard clipboard)
    {
        var clipboard_text = await clipboard.GetTextAsync() ?? "";
        if (string.IsNullOrEmpty(clipboard_text)) return;
        
        if (ExpressionInputSelectionStart == ExpressionInputSelectionEnd)
        {
            ExpressionInput = ExpressionInput.Insert(ExpressionInputCaretIndex, clipboard_text);
        }
        else
        {
            var start = Math.Min(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);
            var end = Math.Max(ExpressionInputSelectionStart, ExpressionInputSelectionEnd);

            ExpressionInput = ExpressionInput.Remove(start, end - start).Insert(start, clipboard_text);
        }
        
        ExpressionInputCaretIndex += clipboard_text.Length;
    }
}
