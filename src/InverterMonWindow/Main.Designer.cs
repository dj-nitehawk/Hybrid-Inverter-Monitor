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
            this.web = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.web)).BeginInit();
            this.SuspendLayout();
            // 
            // web
            // 
            this.web.AllowExternalDrop = false;
            this.web.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.web.CreationProperties = null;
            this.web.DefaultBackgroundColor = System.Drawing.Color.White;
            this.web.Location = new System.Drawing.Point(-1, 1);
            this.web.Name = "web";
            this.web.Size = new System.Drawing.Size(577, 789);
            this.web.Source = new System.Uri("http://inverter.djnitehawk.com", System.UriKind.Absolute);
            this.web.TabIndex = 0;
            this.web.ZoomFactor = 1D;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 790);
            this.Controls.Add(this.web);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.ShowInTaskbar = false;
            this.Text = "InverterMon Window";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.web)).EndInit();
            this.ResumeLayout(false);

    }

    private Microsoft.Web.WebView2.WinForms.WebView2 web;
}
