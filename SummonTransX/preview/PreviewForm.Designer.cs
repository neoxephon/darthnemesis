/*
 * Created by SharpDevelop.
 * User: DarthNemesis
 * Date: 11/14/2009
 * Time: 2:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SummonTransX
{
    partial class PreviewForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
                if (this.dialogBuffer != null) {
                    this.dialogBuffer.Dispose();
                }
                if (this.backBuffer != null) {
                    this.backBuffer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewForm));
        	this.textBoxLine = new System.Windows.Forms.TextBox();
        	this.label2 = new System.Windows.Forms.Label();
        	this.textBoxPadding = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.textBoxLineLength = new System.Windows.Forms.TextBox();
        	this.panelDialogBox = new System.Windows.Forms.Panel();
        	this.label3 = new System.Windows.Forms.Label();
        	this.comboBoxSize = new System.Windows.Forms.ComboBox();
        	this.SuspendLayout();
        	// 
        	// textBoxLine
        	// 
        	this.textBoxLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.textBoxLine.Location = new System.Drawing.Point(12, 32);
        	this.textBoxLine.MaxLength = 999999;
        	this.textBoxLine.Multiline = true;
        	this.textBoxLine.Name = "textBoxLine";
        	this.textBoxLine.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        	this.textBoxLine.Size = new System.Drawing.Size(300, 131);
        	this.textBoxLine.TabIndex = 3;
        	this.textBoxLine.TextChanged += new System.EventHandler(this.TextBoxLineTextChanged);
        	// 
        	// label2
        	// 
        	this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
        	this.label2.Location = new System.Drawing.Point(94, 9);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(50, 13);
        	this.label2.TabIndex = 10;
        	this.label2.Text = "Padding:";
        	// 
        	// textBoxPadding
        	// 
        	this.textBoxPadding.Anchor = System.Windows.Forms.AnchorStyles.Top;
        	this.textBoxPadding.Location = new System.Drawing.Point(150, 7);
        	this.textBoxPadding.MaxLength = 10;
        	this.textBoxPadding.Name = "textBoxPadding";
        	this.textBoxPadding.Size = new System.Drawing.Size(32, 20);
        	this.textBoxPadding.TabIndex = 9;
        	this.textBoxPadding.Text = "0";
        	this.textBoxPadding.TextChanged += new System.EventHandler(this.TextBoxPaddingTextChanged);
        	this.textBoxPadding.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
        	// 
        	// label1
        	// 
        	this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
        	this.label1.Location = new System.Drawing.Point(188, 10);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(86, 13);
        	this.label1.TabIndex = 8;
        	this.label1.Text = "Max line length:";
        	// 
        	// textBoxLineLength
        	// 
        	this.textBoxLineLength.Anchor = System.Windows.Forms.AnchorStyles.Top;
        	this.textBoxLineLength.Location = new System.Drawing.Point(280, 6);
        	this.textBoxLineLength.MaxLength = 10;
        	this.textBoxLineLength.Name = "textBoxLineLength";
        	this.textBoxLineLength.Size = new System.Drawing.Size(32, 20);
        	this.textBoxLineLength.TabIndex = 7;
        	this.textBoxLineLength.Text = "175";
        	this.textBoxLineLength.TextChanged += new System.EventHandler(this.TextBoxLineLengthTextChanged);
        	this.textBoxLineLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
        	// 
        	// panelDialogBox
        	// 
        	this.panelDialogBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.panelDialogBox.Location = new System.Drawing.Point(12, 169);
        	this.panelDialogBox.Name = "panelDialogBox";
        	this.panelDialogBox.Size = new System.Drawing.Size(300, 178);
        	this.panelDialogBox.TabIndex = 11;
        	this.panelDialogBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelDialogBoxPaint);
        	this.panelDialogBox.Resize += new System.EventHandler(this.PanelDialogBoxResize);
        	// 
        	// label3
        	// 
        	this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
        	this.label3.Location = new System.Drawing.Point(12, 9);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(30, 13);
        	this.label3.TabIndex = 12;
        	this.label3.Text = "Size:";
        	// 
        	// comboBoxSize
        	// 
        	this.comboBoxSize.Anchor = System.Windows.Forms.AnchorStyles.Top;
        	this.comboBoxSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBoxSize.FormattingEnabled = true;
        	this.comboBoxSize.Items.AddRange(new object[] {
        	        	        	"12"});
        	this.comboBoxSize.Location = new System.Drawing.Point(48, 6);
        	this.comboBoxSize.Name = "comboBoxSize";
        	this.comboBoxSize.Size = new System.Drawing.Size(40, 21);
        	this.comboBoxSize.TabIndex = 15;
        	this.comboBoxSize.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSizeSelectedIndexChanged);
        	// 
        	// PreviewForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(324, 360);
        	this.Controls.Add(this.comboBoxSize);
        	this.Controls.Add(this.label3);
        	this.Controls.Add(this.panelDialogBox);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.textBoxPadding);
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.textBoxLineLength);
        	this.Controls.Add(this.textBoxLine);
        	this.DoubleBuffered = true;
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MinimumSize = new System.Drawing.Size(332, 300);
        	this.Name = "PreviewForm";
        	this.Text = "Text Preview";
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreviewFormFormClosing);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.ComboBox comboBoxSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelDialogBox;
        private System.Windows.Forms.TextBox textBoxLineLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPadding;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLine;
    }
}
