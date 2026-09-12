using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using UsageIndicator.Models;
using UsageIndicator.Services;

namespace UsageIndicator;

public partial class MainWindow : Window
{
    private readonly List<AccountProfile> _accounts;
    private readonly System.Windows.Threading.DispatcherTimer _pollTimer;
    private WebView2? _browser;
    private Point _pressPoint;
    private bool _dragStarted;
    private bool _isExpanded;
    private bool _suppressAccountChange;
    private bool _usageDashboardRequested;

    public MainWindow()
    {
        InitializeComponent();
        _accounts = ProfileStore.Load();
        _suppressAccountChange = true;
        AccountPicker.ItemsSource = _accounts;
        _suppressAccountChange = false;
        if (_accounts.Count > 0) AccountPicker.SelectedIndex = 0;
        Render();
        _pollTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
        _pollTimer.Tick += async (_, _) => await RefreshUsageAsync();
        _pollTimer.Start();
    }

    private AccountProfile? ActiveAccount => AccountPicker.SelectedItem as AccountProfile;

    private void AddAccount_Click(object sender, RoutedEventArgs e)
    {
        var account = new AccountProfile { Name = $"Account {_accounts.Count + 1}" };
        _accounts.Add(account); ProfileStore.Save(_accounts);
        AccountPicker.Items.Refresh(); AccountPicker.SelectedItem = account;
    }

    private async void AccountPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressAccountChange || ActiveAccount is null) return;
        await ActivateAccountAsync(ActiveAccount);
    }

    private async Task ActivateAccountAsync(AccountProfile account)
    {
        _usageDashboardRequested = false;
        if (_browser is not null)
        {
            _browser.Dispose(); BrowserHost.Children.Clear(); _browser = null;
        }
        SyncStatus.Text = "Creating this account's secure browser profile…";
        _browser = new WebView2(); BrowserHost.Children.Add(_browser);
        var environment = await CoreWebView2Environment.CreateAsync(null, ProfileStore.BrowserProfilePath(account.Id));
        await _browser.EnsureCoreWebView2Async(environment);
        _browser.CoreWebView2.NavigationCompleted += async (_, _) =>
        {
            await TryOpenUsageDashboardAsync();
            await RefreshUsageAsync();
        };
        _browser.CoreWebView2.Navigate("https://chatgpt.com/");
        Render();
    }

    private async void OpenChatGpt_Click(object sender, RoutedEventArgs e)
    {
        if (_browser is null && ActiveAccount is not null) await ActivateAccountAsync(ActiveAccount);
        _browser?.CoreWebView2.Navigate("https://chatgpt.com/");
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await RefreshUsageAsync();

    private async Task RefreshUsageAsync()
    {
        if (_browser?.CoreWebView2 is null || ActiveAccount is null) return;
        try
        {
            var result = await _browser.CoreWebView2.ExecuteScriptAsync("document.body ? document.body.innerText : ''");
            var pageText = System.Text.Json.JsonSerializer.Deserialize<string>(result) ?? string.Empty;
            ActiveAccount.Usage = UsageDetector.Detect(pageText);
            ProfileStore.Save(_accounts); Render();
        }
        catch { SyncStatus.Text = "Could not refresh yet. Keep the signed-in ChatGPT page open."; }
    }

    private async Task TryOpenUsageDashboardAsync()
    {
        if (_usageDashboardRequested || _browser?.CoreWebView2 is null) return;
        try
        {
            const string script = """
                (() => {
                  const candidates = [...document.querySelectorAll('a,button')];
                  const usage = candidates.find(el => /^(usage|usage dashboard)$/i.test((el.innerText || '').trim()));
                  if (!usage) return false;
                  usage.click();
                  return true;
                })()
                """;
            var result = await _browser.CoreWebView2.ExecuteScriptAsync(script);
            _usageDashboardRequested = result == "true";
            if (_usageDashboardRequested) SyncStatus.Text = "Opening the signed-in ChatGPT usage dashboard…";
        }
        catch { }
    }

    private void Render()
    {
        var usage = ActiveAccount?.Usage;
        ChatLabel.Text = usage?.ChatPercent is int chat ? $"{chat}% remaining" : "Waiting…";
        WorkLabel.Text = usage?.WorkPercent is int work ? $"{work}% remaining" : "Waiting…";
        ChatResetLabel.Text = "Reset: " + (usage?.ChatReset ?? "Not shown");
        WorkResetLabel.Text = "Reset: " + (usage?.WorkReset ?? "Not shown");
        TokenStatus.Text = "Token information: " + (usage?.TokenInfo ?? "Not provided by ChatGPT");
        SyncStatus.Text = usage?.Status ?? "Add an account and sign in to ChatGPT.";
        CompactStatus.Text = usage is null ? "Add an account" : $"Chat {UsageText(usage.ChatPercent)} • Work {UsageText(usage.WorkPercent)}";
    }

    private static string UsageText(int? value) => value is int percent ? percent + "%" : "—";

    private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsInsideButton(e.OriginalSource as DependencyObject) || e.OriginalSource is ComboBox) return;
        _pressPoint = e.GetPosition(this); _dragStarted = false; MainCard.CaptureMouse();
    }

    private void Card_MouseMove(object sender, MouseEventArgs e)
    {
        if (!MainCard.IsMouseCaptured || e.LeftButton != MouseButtonState.Pressed || _dragStarted) return;
        var point = e.GetPosition(this);
        if (Math.Abs(point.X - _pressPoint.X) < 5 && Math.Abs(point.Y - _pressPoint.Y) < 5) return;
        _dragStarted = true; MainCard.ReleaseMouseCapture(); DragMove();
    }

    private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (MainCard.IsMouseCaptured) MainCard.ReleaseMouseCapture();
        if (_dragStarted || IsInsideButton(e.OriginalSource as DependencyObject)) return;
        ToggleExpanded();
    }

    private static bool IsInsideButton(DependencyObject? element)
    {
        while (element is not null) { if (element is Button) return true; element = VisualTreeHelper.GetParent(element); }
        return false;
    }

    private async void ToggleExpanded()
    {
        _isExpanded = !_isExpanded;
        ExpandedPanel.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
        AddAccountButton.Visibility = _isExpanded ? Visibility.Visible : Visibility.Collapsed;
        Height = _isExpanded ? 650 : 64;
        if (_isExpanded && ActiveAccount is not null && _browser is null) await ActivateAccountAsync(ActiveAccount);
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        _pollTimer.Stop(); _browser?.Dispose(); ProfileStore.Save(_accounts); base.OnClosed(e);
    }
}
