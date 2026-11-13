namespace WinFormsDemo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Button selectImageButton;
        private Label selectedFileLabel;
        private TextBox resultTextBox;
        private TableLayoutPanel layoutPanel;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            selectImageButton = new Button();
            selectedFileLabel = new Label();
            resultTextBox = new TextBox();
            layoutPanel = new TableLayoutPanel();
            layoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // selectImageButton
            // 
            selectImageButton.AutoSize = true;
            selectImageButton.Location = new Point(15, 15);
            selectImageButton.Name = "selectImageButton";
            selectImageButton.Size = new Size(112, 25);
            selectImageButton.TabIndex = 0;
            selectImageButton.Text = "Select Image...";
            selectImageButton.UseVisualStyleBackColor = true;
            selectImageButton.Click += OnSelectImageClicked;
            // 
            // selectedFileLabel
            // 
            selectedFileLabel.AutoEllipsis = true;
            selectedFileLabel.Dock = DockStyle.Fill;
            selectedFileLabel.Location = new Point(15, 43);
            selectedFileLabel.Name = "selectedFileLabel";
            selectedFileLabel.Size = new Size(770, 15);
            selectedFileLabel.TabIndex = 1;
            selectedFileLabel.Text = "No image selected.";
            selectedFileLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // resultTextBox
            // 
            resultTextBox.BackColor = SystemColors.Window;
            resultTextBox.Dock = DockStyle.Fill;
            resultTextBox.Font = new Font("Consolas", 9F);
            resultTextBox.Location = new Point(15, 61);
            resultTextBox.Multiline = true;
            resultTextBox.Name = "resultTextBox";
            resultTextBox.PlaceholderText = "ExifTool results will appear here.";
            resultTextBox.ReadOnly = true;
            resultTextBox.ScrollBars = ScrollBars.Both;
            resultTextBox.Size = new Size(770, 374);
            resultTextBox.TabIndex = 2;
            resultTextBox.WordWrap = false;
            // 
            // layoutPanel
            // 
            layoutPanel.ColumnCount = 1;
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.Controls.Add(selectImageButton, 0, 0);
            layoutPanel.Controls.Add(selectedFileLabel, 0, 1);
            layoutPanel.Controls.Add(resultTextBox, 0, 2);
            layoutPanel.Dock = DockStyle.Fill;
            layoutPanel.Location = new Point(0, 0);
            layoutPanel.Name = "layoutPanel";
            layoutPanel.Padding = new Padding(12);
            layoutPanel.RowCount = 3;
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPanel.Size = new Size(800, 450);
            layoutPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(layoutPanel);
            MinimumSize = new Size(520, 320);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SharpExifTool WinForms Demo";
            layoutPanel.ResumeLayout(false);
            layoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
