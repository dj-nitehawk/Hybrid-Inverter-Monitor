namespace InverterMonWindow;

partial class Main
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
        web = new Microsoft.Web.WebView2.WinForms.WebView2();
        TrayIcon = new NotifyIcon(components);
        ((System.ComponentModel.ISupportInitialize)web).BeginInit();
        SuspendLayout();
        // 
        // web
        // 
        web.AllowExternalDrop = false;
        web.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        web.CreationProperties = null;
        web.DefaultBackgroundColor = Color.White;
        web.Location = new Point(-1, 1);
        web.Name = "web";
        web.Size = new Size(642, 556);
        web.Source = new Uri("http://inverter.djnitehawk.com", UriKind.Absolute);
        web.TabIndex = 0;
        web.ZoomFactor = 1D;
        // 
        // TrayIcon
        // 
        TrayIcon.Icon = (Icon)resources.GetObject("TrayIcon.Icon");
        TrayIcon.Text = "TrayIcon";
        // 
        // Main
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(639, 558);
        Controls.Add(web);
        MaximizeBox = false;
        Name = "Main";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "InverterMon Window";
        TopMost = true;
        ((System.ComponentModel.ISupportInitialize)web).EndInit();
        ResumeLayout(false);
    }

    private Microsoft.Web.WebView2.WinForms.WebView2 web;
    private NotifyIcon TrayIcon;
}