namespace InverterMonWindow;

public partial class Main : Form
{
    public Main()
    {
        InitializeComponent();
        TrayIcon.Click += TrayIcon_Click;
        Load += Main_Load;
        FormClosed += Main_FormClosed;
        Resize += Main_Resize;
    }

    private void Main_Resize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            WindowState = FormWindowState.Normal;
            Hide();
            TrayIcon.Visible = true;
        }
    }

    private void Main_FormClosed(object? sender, FormClosedEventArgs e)
    {
        Properties.Settings.Default.WindowPosition = Location;
        Properties.Settings.Default.Save();
    }

    private void Main_Load(object? sender, EventArgs e)
    {
        Location = Properties.Settings.Default.WindowPosition;
    }

    private void TrayIcon_Click(object? sender, EventArgs e)
    {
        Show();
        WindowState = FormWindowState.Normal;
        TrayIcon.Visible = false;
    }
}
