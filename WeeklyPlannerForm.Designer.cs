// WeeklyPlannerForm.Designer.cs
namespace FoodManager
{
    partial class WeeklyPlannerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbDay;
        private System.Windows.Forms.ComboBox cmbRecipe;
        private System.Windows.Forms.NumericUpDown numServings;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dgvPlan;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblPreview;

        // new buttons
        private System.Windows.Forms.Button btnOpenShopping;
        private System.Windows.Forms.Button btnExport;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            cmbDay = new ComboBox();
            cmbRecipe = new ComboBox();
            numServings = new NumericUpDown();
            btnAdd = new Button();
            dgvPlan = new DataGridView();
            День = new DataGridViewTextBoxColumn();
            ID = new DataGridViewTextBoxColumn();
            Страва = new DataGridViewTextBoxColumn();
            Порцій = new DataGridViewTextBoxColumn();
            btnCompute = new Button();
            txtResult = new TextBox();
            btnClose = new Button();
            lblPreview = new Label();
            btnOpenShopping = new Button();
            btnExport = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            textBox6 = new TextBox();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            textBox7 = new TextBox();
            label7 = new Label();
            ((System.ComponentModel.ISupportInitialize)numServings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPlan).BeginInit();
            SuspendLayout();
            // 
            // cmbDay
            // 
            cmbDay.Location = new Point(750, 46);
            cmbDay.Name = "cmbDay";
            cmbDay.Size = new Size(141, 33);
            cmbDay.TabIndex = 0;
            // 
            // cmbRecipe
            // 
            cmbRecipe.Location = new Point(897, 46);
            cmbRecipe.Name = "cmbRecipe";
            cmbRecipe.Size = new Size(301, 33);
            cmbRecipe.TabIndex = 1;
            // 
            // numServings
            // 
            numServings.Location = new Point(1204, 48);
            numServings.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numServings.Name = "numServings";
            numServings.Size = new Size(61, 31);
            numServings.TabIndex = 2;
            numServings.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(1093, 354);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(172, 36);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "Додати";
            btnAdd.Click += btnAdd_Click;
            // 
            // dgvPlan
            // 
            dgvPlan.ColumnHeadersHeight = 34;
            dgvPlan.Columns.AddRange(new DataGridViewColumn[] { День, ID, Страва, Порцій });
            dgvPlan.Location = new Point(12, 48);
            dgvPlan.Name = "dgvPlan";
            dgvPlan.RowHeadersVisible = false;
            dgvPlan.RowHeadersWidth = 62;
            dgvPlan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPlan.Size = new Size(732, 300);
            dgvPlan.TabIndex = 4;
            // 
            // День
            // 
            День.HeaderText = "День";
            День.MinimumWidth = 8;
            День.Name = "День";
            День.Width = 150;
            // 
            // ID
            // 
            ID.HeaderText = "ID";
            ID.MinimumWidth = 8;
            ID.Name = "ID";
            ID.Visible = false;
            ID.Width = 150;
            // 
            // Страва
            // 
            Страва.HeaderText = "Страва";
            Страва.MinimumWidth = 8;
            Страва.Name = "Страва";
            Страва.Width = 150;
            // 
            // Порцій
            // 
            Порцій.HeaderText = "Порцій";
            Порцій.MinimumWidth = 8;
            Порцій.Name = "Порцій";
            Порцій.Width = 150;
            // 
            // btnCompute
            // 
            btnCompute.Location = new Point(12, 354);
            btnCompute.Name = "btnCompute";
            btnCompute.Size = new Size(160, 36);
            btnCompute.TabIndex = 5;
            btnCompute.Text = "Розрахувати";
            btnCompute.Click += btnCompute_Click;
            // 
            // txtResult
            // 
            txtResult.Location = new Point(12, 408);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.Size = new Size(1253, 180);
            txtResult.TabIndex = 6;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(1145, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(120, 36);
            btnClose.TabIndex = 7;
            btnClose.Text = "Закрити";
            btnClose.Click += btnClose_Click;
            // 
            // lblPreview
            // 
            lblPreview.Location = new Point(750, 82);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(308, 38);
            lblPreview.TabIndex = 9;
            lblPreview.Text = "Інформація про страву";
            // 
            // btnOpenShopping
            // 
            btnOpenShopping.Location = new Point(180, 354);
            btnOpenShopping.Name = "btnOpenShopping";
            btnOpenShopping.Size = new Size(200, 36);
            btnOpenShopping.TabIndex = 8;
            btnOpenShopping.Text = "Закупитися";
            btnOpenShopping.Click += btnOpenShopping_Click;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(584, 354);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(160, 36);
            btnExport.TabIndex = 9;
            btnExport.Text = "Експорт XLSX";
            btnExport.Click += btnExport_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(788, 115);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(150, 31);
            textBox1.TabIndex = 10;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(788, 152);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(150, 31);
            textBox2.TabIndex = 11;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(788, 189);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(150, 31);
            textBox3.TabIndex = 12;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(1094, 115);
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.Size = new Size(150, 31);
            textBox4.TabIndex = 13;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(1094, 152);
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.Size = new Size(150, 31);
            textBox5.TabIndex = 14;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(1094, 189);
            textBox6.Name = "textBox6";
            textBox6.ReadOnly = true;
            textBox6.Size = new Size(150, 31);
            textBox6.TabIndex = 15;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(760, 118);
            label3.Name = "label3";
            label3.Size = new Size(22, 25);
            label3.TabIndex = 19;
            label3.Text = "Б";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(760, 192);
            label1.Name = "label1";
            label1.Size = new Size(22, 25);
            label1.TabIndex = 20;
            label1.Text = "В";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(760, 155);
            label2.Name = "label2";
            label2.Size = new Size(28, 25);
            label2.TabIndex = 21;
            label2.Text = "Ж";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(944, 155);
            label4.Name = "label4";
            label4.Size = new Size(142, 25);
            label4.TabIndex = 22;
            label4.Text = "– з них насичені";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(944, 192);
            label5.Name = "label5";
            label5.Size = new Size(122, 25);
            label5.TabIndex = 23;
            label5.Text = "– з них цукри";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(1018, 118);
            label6.Name = "label6";
            label6.Size = new Size(70, 25);
            label6.TabIndex = 24;
            label6.Text = "Калорії";
            // 
            // textBox7
            // 
            textBox7.Location = new Point(1093, 226);
            textBox7.Name = "textBox7";
            textBox7.ReadOnly = true;
            textBox7.Size = new Size(150, 31);
            textBox7.TabIndex = 25;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(1038, 229);
            label7.Name = "label7";
            label7.Size = new Size(48, 25);
            label7.TabIndex = 26;
            label7.Text = "Ціна";
            // 
            // WeeklyPlannerForm
            // 
            ClientSize = new Size(1277, 600);
            Controls.Add(btnExport);
            Controls.Add(btnOpenShopping);
            Controls.Add(label7);
            Controls.Add(textBox7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(textBox6);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(cmbDay);
            Controls.Add(cmbRecipe);
            Controls.Add(numServings);
            Controls.Add(btnAdd);
            Controls.Add(dgvPlan);
            Controls.Add(btnCompute);
            Controls.Add(txtResult);
            Controls.Add(btnClose);
            Controls.Add(lblPreview);
            Name = "WeeklyPlannerForm";
            Text = "Планування меню";
            ((System.ComponentModel.ISupportInitialize)numServings).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPlan).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
        private TextBox textBox6;
        private Label label3;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox textBox7;
        private Label label7;

        private DataGridViewTextBoxColumn День;
        private DataGridViewTextBoxColumn ID;
        private DataGridViewTextBoxColumn Страва;
        private DataGridViewTextBoxColumn Порцій;
    }
}