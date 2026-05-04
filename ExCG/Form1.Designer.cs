namespace ExCG
{
    partial class FrmPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            pictureBox1 = new PictureBox();
            btAbrirImg = new Button();
            button1 = new Button();
            openFileDialog = new OpenFileDialog();
            btLimpar = new Button();
            checkBox1 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(974, 352);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btAbrirImg
            // 
            btAbrirImg.Location = new Point(8, 370);
            btAbrirImg.Name = "btAbrirImg";
            btAbrirImg.Size = new Size(104, 23);
            btAbrirImg.TabIndex = 1;
            btAbrirImg.Text = "Abrir Imagem";
            btAbrirImg.UseVisualStyleBackColor = true;
            btAbrirImg.Click += btAbrirImg_Click;
            // 
            // button1
            // 
            button1.Location = new Point(138, 370);
            button1.Name = "button1";
            button1.Size = new Size(98, 23);
            button1.TabIndex = 2;
            button1.Text = "Luminância";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btLuminancia_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // btLimpar
            // 
            btLimpar.Location = new Point(21, 399);
            btLimpar.Name = "btLimpar";
            btLimpar.Size = new Size(75, 23);
            btLimpar.TabIndex = 4;
            btLimpar.Text = "Limpar Imagem";
            btLimpar.UseVisualStyleBackColor = true;
            btLimpar.Click += btLimpar_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(138, 399);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(87, 19);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "Conversões";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // FrmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(998, 438);
            Controls.Add(checkBox1);
            Controls.Add(btLimpar);
            Controls.Add(button1);
            Controls.Add(btAbrirImg);
            Controls.Add(pictureBox1);
            Name = "FrmPrincipal";
            Text = "Editor de Imagem";
            Load += FrmPrincipal_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btAbrirImg;
        private System.Windows.Forms.Button button1;
        private OpenFileDialog openFileDialog;
        private Button btLimpar;
        private CheckBox checkBox1;
    }
}
