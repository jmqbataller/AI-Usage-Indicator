using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UsageIndicator.Models;
using UsageIndicator.Services;

namespace UsageIndicator;

public partial class MainWindow : Window
{
    private UsageSettings _settings;
    private Point _pressPoint;
    private bool _dragStarted;
    private bool _isExpanded;

    public MainWindow()
    {
        InitializeComponent();
        _settings = SettingsStore.Load();
        Left = _settings.Left;
        Top = _settings.Top;
        Render();
    }

    private void Render()
    {
        CodexBar.Value = _settings.CodexPercent;
        WorkBar.Value = _settings.WorkPercent;
        CodexLabel.Text = $"{_settings.CodexPercent}% remaining";
        WorkLabel.Text = $"{_settings.WorkPercent}% remaining";
        CompactStatus.Text = $"Codex {_settings.CodexPercent}%  •  Work {_settings.WorkPercent}%";
        CodexResetLabel.Text = $"Reset: {_settings.CodexReset}";
        WorkResetLabel.Text = $"Reset: {_settings.WorkReset}";
    }

    private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsInsideButton(e.OriginalSource as DependencyObject)) return;
        _pressPoint = e.GetPosition(this);
        _dragStarted = false;
        MainCard.CaptureMouse();
    }

    private void Card_MouseMove(object sender, MouseEventArgs e)
    {
        if (!MainCard.IsMouseCaptured || e.LeftButton != MouseButtonState.Pressed || _dragStarted) return;
        var point = e.GetPosition(this);
        if (Math.Abs(point.X - _pressPoint.X) < 5 && Math.Abs(point.Y - _pressPoint.Y) < 5) return;
        _dragStarted = true;
        MainCard.ReleaseMouseCapture();
        DragMove();
    }

    private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (MainCard.IsMouseCaptured) MainCard.ReleaseMouseCapture();
        if (_dragStarted || IsInsideButton(e.OriginalSource as DependencyObject)) return;
        ToggleExpanded();
    }

    private static bool IsInsideButton(DependencyObject? element)
    {
        while (element is not null)
        {
            if (element is Button) return true;
            element = VisualTreeHelper.GetParent(element);
        }
        return false;
    }

    private void ToggleExpanded()
    {
        _isExpanded = !_isExpanded;
        UsagePanel.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
        FooterText.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
        EditButton.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
        Height = _isExpanded ? 212 : 64;
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new UpdateWindow(_settings) { Owner = this };
        if (dialog.ShowDialog() == true)
        {
            _settings = dialog.Settings;
            SettingsStore.Save(_settings);
            Render();
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        _settings.Left = Left; _settings.Top = Top; SettingsStore.Save(_settings);
        base.OnClosed(e);
    }
}

public sealed class UpdateWindow : Window
{
    private readonly TextBox _codex = new() { MinWidth = 120 };
    private readonly TextBox _codexReset = new() { MinWidth = 150 };
    private readonly TextBox _work = new() { MinWidth = 120 };
    private readonly TextBox _workReset = new() { MinWidth = 150 };
    public UsageSettings Settings { get; }

    public UpdateWindow(UsageSettings current)
    {
        Settings = new UsageSettings { CodexPercent = current.CodexPercent, CodexReset = current.CodexReset, WorkPercent = current.WorkPercent, WorkReset = current.WorkReset, Left = current.Left, Top = current.Top };
        Title = "Update usage"; Width = 410; Height = 285; WindowStartupLocation = WindowStartupLocation.CenterOwner; ResizeMode = ResizeMode.NoResize;
        _codex.Text = Settings.CodexPercent.ToString(); _codexReset.Text = Settings.CodexReset; _work.Text = Settings.WorkPercent.ToString(); _workReset.Text = Settings.WorkReset;
        var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(18) };
        panel.Children.Add(new System.Windows.Controls.TextBlock { Text = "Enter the remaining % shown in the official usage dashboard.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 14) });
        AddRow(panel, "Codex remaining (%)", _codex); AddRow(panel, "Codex reset", _codexReset); AddRow(panel, "ChatGPT Work remaining (%)", _work); AddRow(panel, "Work reset", _workReset);
        var save = new System.Windows.Controls.Button { Content = "Save", HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 14, 0, 0) }; save.Click += Save_Click; panel.Children.Add(save); Content = panel;
    }
    private static void AddRow(System.Windows.Controls.Panel p, string label, System.Windows.Controls.TextBox box)
    { var row = new System.Windows.Controls.Grid { Margin = new Thickness(0, 0, 0, 8) }; row.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(190) }); row.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition()); var t = new System.Windows.Controls.TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center }; System.Windows.Controls.Grid.SetColumn(box, 1); row.Children.Add(t); row.Children.Add(box); p.Children.Add(row); }
    private void Save_Click(object sender, RoutedEventArgs e)
    { if (!TryPercent(_codex.Text, out var c) || !TryPercent(_work.Text, out var w)) { MessageBox.Show("Use a number from 0 to 100 for both remaining values.", "Check values", MessageBoxButton.OK, MessageBoxImage.Warning); return; } Settings.CodexPercent = c; Settings.CodexReset = _codexReset.Text.Trim(); Settings.WorkPercent = w; Settings.WorkReset = _workReset.Text.Trim(); DialogResult = true; }
    private static bool TryPercent(string input, out int value) { var m = Regex.Match(input, @"\d+"); return int.TryParse(m.Value, out value) && value is >= 0 and <= 100; }
}
