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
        
        string res = "";

        if (start == end)
        {
            res = value.Insert(caret, clipboard_text);
        }
        else
        {
            res = value.Remove(start, end - start).Insert(start, clipboard_text);
        }
        
        return (res, clipboard_text.Length);
    }
}
