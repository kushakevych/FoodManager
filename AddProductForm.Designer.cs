// Designer updated: txtWeight is used as default pack; removed txtPackDefault.
// Includes nutrient labels and custom pack field.
namespace FoodManager
{
    partial class AddProductForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtWeight; // used as default pack (in product unit)
        private System.Windows.Forms.TextBox txtCarbs;
        private System.Windows.Forms.TextBox txtSugars;
        private System.Windows.Forms.TextBox txtFats;
        private System.Windows.Forms.TextBox txtSat;
        private System.Windows.Forms.TextBox txtProt;
        private System.Windows.Forms.TextBox txtKcal;
        private System.Windows.Forms.TextBox txtFiber;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbUnit;
        private System.Windows.Forms.Label lblUnit;

        private System.Windows.Forms.Label lblATB;
        private System.Windows.Forms.TextBox txtPriceATB;
        private System.Windows.Forms.TextBox txtLinkATB;
        private System.Windows.Forms.TextBox txtPackATB;

        private System.Windows.Forms.Label lblSilpo;
        private System.Windows.Forms.TextBox txtPriceSilpo;
        private System.Windows.Forms.TextBox txtLinkSilpo;
        private System.Windows.Forms.TextBox txtPackSilpo;

        private System.Windows.Forms.Label lblMetro;
        private System.Windows.Forms.TextBox txtPriceMetro;
        private System.Windows.Forms.TextBox txtLinkMetro;
        private System.Windows.Forms.TextBox txtPackMetro;

        private System.Windows.Forms.Label lblAshan;
        private System.Windows.Forms.TextBox txtPriceAshan;
        private System.Windows.Forms.TextBox txtLinkAshan;
        private System.Windows.Forms.TextBox txtPackAshan;

        private System.Windows.Forms.Label lblStolich;
        private System.Windows.Forms.TextBox txtPriceStolich;
        private System.Windows.Forms.TextBox txtLinkStolich;
        private System.Windows.Forms.TextBox txtPackStolich;

        private System.Windows.Forms.Label lblVK;
        private System.Windows.Forms.TextBox txtPriceVK;
        private System.Windows.Forms.TextBox txtLinkVK;
        private System.Windows.Forms.TextBox txtPackVK;

        private System.Windows.Forms.Label lblNovus;
        private System.Windows.Forms.TextBox txtPriceNovus;
        private System.Windows.Forms.TextBox txtLinkNovus;
        private System.Windows.Forms.TextBox txtPackNovus;

        private System.Windows.Forms.Label lblCustomTitle;
        private System.Windows.Forms.TextBox txtCustomSourceName;
        private System.Windows.Forms.TextBox txtCustomPrice;
        private System.Windows.Forms.TextBox txtCustomPack;
        private System.Windows.Forms.Button btnAddCustom;
        private System.Windows.Forms.Button btnEditCustom;
        private System.Windows.Forms.ListBox lstManualSources;
        private System.Windows.Forms.Button btnRemoveCustom;

        private System.Windows.Forms.Label labelNutrition;
        private System.Windows.Forms.Label labelProt;
        private System.Windows.Forms.Label labelFats;
        private System.Windows.Forms.Label labelCarbs;
        private System.Windows.Forms.Label labelSugars;
        private System.Windows.Forms.Label labelSat;
        private System.Windows.Forms.Label labelKcal;
        private System.Windows.Forms.Label labelFiber;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtName = new TextBox();
            txtWeight = new TextBox();
            txtCarbs = new TextBox();
            txtSugars = new TextBox();
            txtFats = new TextBox();
            txtSat = new TextBox();
            txtProt = new TextBox();
            txtKcal = new TextBox();
            txtFiber = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            cmbUnit = new ComboBox();
            lblUnit = new Label();
            lblATB = new Label();
            txtPriceATB = new TextBox();
            txtLinkATB = new TextBox();
            txtPackATB = new TextBox();
            lblSilpo = new Label();
            txtPriceSilpo = new TextBox();
            txtLinkSilpo = new TextBox();
            txtPackSilpo = new TextBox();
            lblMetro = new Label();
            txtPriceMetro = new TextBox();
            txtLinkMetro = new TextBox();
            txtPackMetro = new TextBox();
            lblAshan = new Label();
            txtPriceAshan = new TextBox();
            txtLinkAshan = new TextBox();
            txtPackAshan = new TextBox();
            lblStolich = new Label();
            txtPriceStolich = new TextBox();
            txtLinkStolich = new TextBox();
            txtPackStolich = new TextBox();
            lblVK = new Label();
            txtPriceVK = new TextBox();
            txtLinkVK = new TextBox();
            txtPackVK = new TextBox();
            lblNovus = new Label();
            txtPriceNovus = new TextBox();
            txtLinkNovus = new TextBox();
            txtPackNovus = new TextBox();
            lblCustomTitle = new Label();
            txtCustomSourceName = new TextBox();
            txtCustomPrice = new TextBox();
            txtCustomPack = new TextBox();
            btnAddCustom = new Button();
            btnEditCustom = new Button();
            lstManualSources = new ListBox();
            btnRemoveCustom = new Button();
            labelNutrition = new Label();
            labelProt = new Label();
            labelFats = new Label();
            labelCarbs = new Label();
            labelSugars = new Label();
            labelSat = new Label();
            labelKcal = new Label();
            labelFiber = new Label();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new Point(12, 12);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Найменування продукту";
            txtName.Size = new Size(674, 31);
            txtName.TabIndex = 0;
            // 
            // txtWeight
            // 
            txtWeight.Location = new Point(12, 52);
            txtWeight.Name = "txtWeight";
            txtWeight.PlaceholderText = "Фасовка за замовч.";
            txtWeight.Size = new Size(200, 31);
            txtWeight.TabIndex = 1;
            // 
            // txtCarbs
            // 
            txtCarbs.Location = new Point(54, 197);
            txtCarbs.Name = "txtCarbs";
            txtCarbs.Size = new Size(117, 31);
            txtCarbs.TabIndex = 9;
            // 
            // txtSugars
            // 
            txtSugars.Location = new Point(316, 197);
            txtSugars.Name = "txtSugars";
            txtSugars.Size = new Size(117, 31);
            txtSugars.TabIndex = 11;
            // 
            // txtFats
            // 
            txtFats.Location = new Point(316, 125);
            txtFats.Name = "txtFats";
            txtFats.Size = new Size(117, 31);
            txtFats.TabIndex = 7;
            // 
            // txtSat
            // 
            txtSat.Location = new Point(316, 161);
            txtSat.Name = "txtSat";
            txtSat.Size = new Size(117, 31);
            txtSat.TabIndex = 13;
            // 
            // txtProt
            // 
            txtProt.Location = new Point(54, 125);
            txtProt.Name = "txtProt";
            txtProt.Size = new Size(117, 31);
            txtProt.TabIndex = 5;
            // 
            // txtKcal
            // 
            txtKcal.Location = new Point(516, 161);
            txtKcal.Name = "txtKcal";
            txtKcal.Size = new Size(100, 31);
            txtKcal.TabIndex = 15;
            // 
            // txtFiber
            // 
            txtFiber.Location = new Point(54, 161);
            txtFiber.Name = "txtFiber";
            txtFiber.Size = new Size(117, 31);
            txtFiber.TabIndex = 17;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(12, 670);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(100, 31);
            btnOK.TabIndex = 54;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(118, 670);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 31);
            btnCancel.TabIndex = 55;
            btnCancel.Text = "Відмінити";
            // 
            // cmbUnit
            // 
            cmbUnit.Location = new Point(230, 52);
            cmbUnit.Name = "cmbUnit";
            cmbUnit.Size = new Size(120, 33);
            cmbUnit.TabIndex = 2;
            // 
            // lblUnit
            // 
            lblUnit.Location = new Point(0, 0);
            lblUnit.Name = "lblUnit";
            lblUnit.Size = new Size(100, 23);
            lblUnit.TabIndex = 0;
            // 
            // lblATB
            // 
            lblATB.Location = new Point(12, 240);
            lblATB.Name = "lblATB";
            lblATB.Size = new Size(100, 23);
            lblATB.TabIndex = 18;
            lblATB.Text = "АТБ:";
            // 
            // txtPriceATB
            // 
            txtPriceATB.Location = new Point(186, 236);
            txtPriceATB.Name = "txtPriceATB";
            txtPriceATB.PlaceholderText = "Ціна";
            txtPriceATB.Size = new Size(80, 31);
            txtPriceATB.TabIndex = 19;
            // 
            // txtLinkATB
            // 
            txtLinkATB.Location = new Point(366, 236);
            txtLinkATB.Name = "txtLinkATB";
            txtLinkATB.PlaceholderText = "Посилання";
            txtLinkATB.Size = new Size(320, 31);
            txtLinkATB.TabIndex = 21;
            // 
            // txtPackATB
            // 
            txtPackATB.Location = new Point(272, 237);
            txtPackATB.Name = "txtPackATB";
            txtPackATB.PlaceholderText = "Фасовка";
            txtPackATB.Size = new Size(88, 31);
            txtPackATB.TabIndex = 20;
            // 
            // lblSilpo
            // 
            lblSilpo.Location = new Point(12, 276);
            lblSilpo.Name = "lblSilpo";
            lblSilpo.Size = new Size(100, 23);
            lblSilpo.TabIndex = 22;
            lblSilpo.Text = "Сільпо:";
            // 
            // txtPriceSilpo
            // 
            txtPriceSilpo.Location = new Point(186, 272);
            txtPriceSilpo.Name = "txtPriceSilpo";
            txtPriceSilpo.PlaceholderText = "Ціна";
            txtPriceSilpo.Size = new Size(80, 31);
            txtPriceSilpo.TabIndex = 23;
            // 
            // txtLinkSilpo
            // 
            txtLinkSilpo.Location = new Point(366, 272);
            txtLinkSilpo.Name = "txtLinkSilpo";
            txtLinkSilpo.PlaceholderText = "Посилання";
            txtLinkSilpo.Size = new Size(320, 31);
            txtLinkSilpo.TabIndex = 25;
            // 
            // txtPackSilpo
            // 
            txtPackSilpo.Location = new Point(272, 272);
            txtPackSilpo.Name = "txtPackSilpo";
            txtPackSilpo.PlaceholderText = "Фасовка";
            txtPackSilpo.Size = new Size(88, 31);
            txtPackSilpo.TabIndex = 24;
            // 
            // lblMetro
            // 
            lblMetro.Location = new Point(12, 312);
            lblMetro.Name = "lblMetro";
            lblMetro.Size = new Size(100, 23);
            lblMetro.TabIndex = 26;
            lblMetro.Text = "Метро:";
            // 
            // txtPriceMetro
            // 
            txtPriceMetro.Location = new Point(186, 308);
            txtPriceMetro.Name = "txtPriceMetro";
            txtPriceMetro.PlaceholderText = "Ціна";
            txtPriceMetro.Size = new Size(80, 31);
            txtPriceMetro.TabIndex = 27;
            // 
            // txtLinkMetro
            // 
            txtLinkMetro.Location = new Point(366, 308);
            txtLinkMetro.Name = "txtLinkMetro";
            txtLinkMetro.PlaceholderText = "Посилання";
            txtLinkMetro.Size = new Size(320, 31);
            txtLinkMetro.TabIndex = 29;
            // 
            // txtPackMetro
            // 
            txtPackMetro.Location = new Point(272, 308);
            txtPackMetro.Name = "txtPackMetro";
            txtPackMetro.PlaceholderText = "Фасовка";
            txtPackMetro.Size = new Size(88, 31);
            txtPackMetro.TabIndex = 28;
            // 
            // lblAshan
            // 
            lblAshan.Location = new Point(12, 348);
            lblAshan.Name = "lblAshan";
            lblAshan.Size = new Size(100, 23);
            lblAshan.TabIndex = 30;
            lblAshan.Text = "Ашан:";
            // 
            // txtPriceAshan
            // 
            txtPriceAshan.Location = new Point(186, 344);
            txtPriceAshan.Name = "txtPriceAshan";
            txtPriceAshan.PlaceholderText = "Ціна";
            txtPriceAshan.Size = new Size(80, 31);
            txtPriceAshan.TabIndex = 31;
            // 
            // txtLinkAshan
            // 
            txtLinkAshan.Location = new Point(366, 344);
            txtLinkAshan.Name = "txtLinkAshan";
            txtLinkAshan.PlaceholderText = "Посилання";
            txtLinkAshan.Size = new Size(320, 31);
            txtLinkAshan.TabIndex = 33;
            // 
            // txtPackAshan
            // 
            txtPackAshan.Location = new Point(272, 344);
            txtPackAshan.Name = "txtPackAshan";
            txtPackAshan.PlaceholderText = "Фасовка";
            txtPackAshan.Size = new Size(88, 31);
            txtPackAshan.TabIndex = 32;
            // 
            // lblStolich
            // 
            lblStolich.Location = new Point(12, 384);
            lblStolich.Name = "lblStolich";
            lblStolich.Size = new Size(171, 27);
            lblStolich.TabIndex = 34;
            lblStolich.Text = "Столичний ринок:";
            // 
            // txtPriceStolich
            // 
            txtPriceStolich.Location = new Point(186, 380);
            txtPriceStolich.Name = "txtPriceStolich";
            txtPriceStolich.PlaceholderText = "Ціна";
            txtPriceStolich.Size = new Size(80, 31);
            txtPriceStolich.TabIndex = 35;
            // 
            // txtLinkStolich
            // 
            txtLinkStolich.Location = new Point(366, 380);
            txtLinkStolich.Name = "txtLinkStolich";
            txtLinkStolich.PlaceholderText = "Найменування в системі";
            txtLinkStolich.Size = new Size(320, 31);
            txtLinkStolich.TabIndex = 37;
            // 
            // txtPackStolich
            // 
            txtPackStolich.Location = new Point(272, 380);
            txtPackStolich.Name = "txtPackStolich";
            txtPackStolich.PlaceholderText = "Фасовка";
            txtPackStolich.Size = new Size(88, 31);
            txtPackStolich.TabIndex = 36;
            // 
            // lblVK
            // 
            lblVK.Location = new Point(12, 458);
            lblVK.Name = "lblVK";
            lblVK.Size = new Size(142, 27);
            lblVK.TabIndex = 38;
            lblVK.Text = "Велика Кишеня:";
            // 
            // txtPriceVK
            // 
            txtPriceVK.Location = new Point(186, 454);
            txtPriceVK.Name = "txtPriceVK";
            txtPriceVK.PlaceholderText = "Ціна";
            txtPriceVK.Size = new Size(80, 31);
            txtPriceVK.TabIndex = 39;
            // 
            // txtLinkVK
            // 
            txtLinkVK.Enabled = false;
            txtLinkVK.Location = new Point(366, 454);
            txtLinkVK.Name = "txtLinkVK";
            txtLinkVK.PlaceholderText = "Відсутня актуалізація";
            txtLinkVK.Size = new Size(320, 31);
            txtLinkVK.TabIndex = 41;
            // 
            // txtPackVK
            // 
            txtPackVK.Location = new Point(272, 454);
            txtPackVK.Name = "txtPackVK";
            txtPackVK.PlaceholderText = "Фасовка";
            txtPackVK.Size = new Size(88, 31);
            txtPackVK.TabIndex = 40;
            // 
            // lblNovus
            // 
            lblNovus.Location = new Point(12, 421);
            lblNovus.Name = "lblNovus";
            lblNovus.Size = new Size(100, 23);
            lblNovus.TabIndex = 42;
            lblNovus.Text = "NOVUS:";
            // 
            // txtPriceNovus
            // 
            txtPriceNovus.Location = new Point(186, 417);
            txtPriceNovus.Name = "txtPriceNovus";
            txtPriceNovus.PlaceholderText = "Ціна";
            txtPriceNovus.Size = new Size(80, 31);
            txtPriceNovus.TabIndex = 43;
            // 
            // txtLinkNovus
            // 
            txtLinkNovus.Location = new Point(366, 417);
            txtLinkNovus.Name = "txtLinkNovus";
            txtLinkNovus.PlaceholderText = "Посилання";
            txtLinkNovus.Size = new Size(320, 31);
            txtLinkNovus.TabIndex = 45;
            // 
            // txtPackNovus
            // 
            txtPackNovus.Location = new Point(272, 417);
            txtPackNovus.Name = "txtPackNovus";
            txtPackNovus.PlaceholderText = "Фасовка";
            txtPackNovus.Size = new Size(88, 31);
            txtPackNovus.TabIndex = 44;
            // 
            // lblCustomTitle
            // 
            lblCustomTitle.Location = new Point(10, 496);
            lblCustomTitle.Name = "lblCustomTitle";
            lblCustomTitle.Size = new Size(256, 25);
            lblCustomTitle.TabIndex = 46;
            lblCustomTitle.Text = "Ручні джерела цін:";
            // 
            // txtCustomSourceName
            // 
            txtCustomSourceName.Location = new Point(12, 524);
            txtCustomSourceName.Name = "txtCustomSourceName";
            txtCustomSourceName.PlaceholderText = "Найменування джерела";
            txtCustomSourceName.Size = new Size(220, 31);
            txtCustomSourceName.TabIndex = 47;
            // 
            // txtCustomPrice
            // 
            txtCustomPrice.Location = new Point(240, 524);
            txtCustomPrice.Name = "txtCustomPrice";
            txtCustomPrice.PlaceholderText = "Ціна";
            txtCustomPrice.Size = new Size(100, 31);
            txtCustomPrice.TabIndex = 48;
            // 
            // txtCustomPack
            // 
            txtCustomPack.Location = new Point(346, 523);
            txtCustomPack.Name = "txtCustomPack";
            txtCustomPack.PlaceholderText = "Фасовка";
            txtCustomPack.Size = new Size(100, 31);
            txtCustomPack.TabIndex = 49;
            // 
            // btnAddCustom
            // 
            btnAddCustom.Location = new Point(452, 524);
            btnAddCustom.Name = "btnAddCustom";
            btnAddCustom.Size = new Size(116, 31);
            btnAddCustom.TabIndex = 50;
            btnAddCustom.Text = "Додати";
            btnAddCustom.Click += btnAddCustom_Click;
            // 
            // btnEditCustom
            // 
            btnEditCustom.Location = new Point(574, 524);
            btnEditCustom.Name = "btnEditCustom";
            btnEditCustom.Size = new Size(116, 31);
            btnEditCustom.TabIndex = 51;
            btnEditCustom.Text = "Редагувати";
            btnEditCustom.Click += btnEditCustom_Click;
            // 
            // lstManualSources
            // 
            lstManualSources.ItemHeight = 25;
            lstManualSources.Location = new Point(12, 560);
            lstManualSources.Name = "lstManualSources";
            lstManualSources.Size = new Size(556, 104);
            lstManualSources.TabIndex = 52;
            lstManualSources.DoubleClick += lstManualSources_DoubleClick;
            // 
            // btnRemoveCustom
            // 
            btnRemoveCustom.Location = new Point(574, 633);
            btnRemoveCustom.Name = "btnRemoveCustom";
            btnRemoveCustom.Size = new Size(116, 31);
            btnRemoveCustom.TabIndex = 53;
            btnRemoveCustom.Text = "Видалити";
            btnRemoveCustom.Click += btnRemoveCustom_Click;
            // 
            // labelNutrition
            // 
            labelNutrition.Location = new Point(12, 96);
            labelNutrition.Name = "labelNutrition";
            labelNutrition.Size = new Size(270, 25);
            labelNutrition.TabIndex = 3;
            labelNutrition.Text = "Харчова цінність продукту:";
            // 
            // labelProt
            // 
            labelProt.Location = new Point(12, 128);
            labelProt.Name = "labelProt";
            labelProt.Size = new Size(36, 27);
            labelProt.TabIndex = 4;
            labelProt.Text = "Б:";
            // 
            // labelFats
            // 
            labelFats.Location = new Point(12, 166);
            labelFats.Name = "labelFats";
            labelFats.Size = new Size(36, 27);
            labelFats.TabIndex = 6;
            labelFats.Text = "Ж:";
            // 
            // labelCarbs
            // 
            labelCarbs.Location = new Point(12, 200);
            labelCarbs.Name = "labelCarbs";
            labelCarbs.Size = new Size(36, 27);
            labelCarbs.TabIndex = 8;
            labelCarbs.Text = "В:";
            // 
            // labelSugars
            // 
            labelSugars.Location = new Point(177, 200);
            labelSugars.Name = "labelSugars";
            labelSugars.Size = new Size(133, 27);
            labelSugars.TabIndex = 10;
            labelSugars.Text = "з них цукри";
            // 
            // labelSat
            // 
            labelSat.Location = new Point(177, 164);
            labelSat.Name = "labelSat";
            labelSat.Size = new Size(133, 28);
            labelSat.TabIndex = 12;
            labelSat.Text = "з них насичені";
            // 
            // labelKcal
            // 
            labelKcal.Location = new Point(516, 135);
            labelKcal.Name = "labelKcal";
            labelKcal.Size = new Size(100, 23);
            labelKcal.TabIndex = 14;
            labelKcal.Text = "Ккал";
            // 
            // labelFiber
            // 
            labelFiber.Location = new Point(280, 128);
            labelFiber.Name = "labelFiber";
            labelFiber.Size = new Size(30, 27);
            labelFiber.TabIndex = 16;
            labelFiber.Text = "К:";
            // 
            // AddProductForm
            // 
            ClientSize = new Size(703, 711);
            Controls.Add(txtName);
            Controls.Add(txtWeight);
            Controls.Add(cmbUnit);
            Controls.Add(labelNutrition);
            Controls.Add(labelProt);
            Controls.Add(txtProt);
            Controls.Add(labelFats);
            Controls.Add(txtFats);
            Controls.Add(labelCarbs);
            Controls.Add(txtCarbs);
            Controls.Add(labelSugars);
            Controls.Add(txtSugars);
            Controls.Add(labelSat);
            Controls.Add(txtSat);
            Controls.Add(labelKcal);
            Controls.Add(txtKcal);
            Controls.Add(labelFiber);
            Controls.Add(txtFiber);
            Controls.Add(lblATB);
            Controls.Add(txtPriceATB);
            Controls.Add(txtPackATB);
            Controls.Add(txtLinkATB);
            Controls.Add(lblSilpo);
            Controls.Add(txtPriceSilpo);
            Controls.Add(txtPackSilpo);
            Controls.Add(txtLinkSilpo);
            Controls.Add(lblMetro);
            Controls.Add(txtPriceMetro);
            Controls.Add(txtPackMetro);
            Controls.Add(txtLinkMetro);
            Controls.Add(lblAshan);
            Controls.Add(txtPriceAshan);
            Controls.Add(txtPackAshan);
            Controls.Add(txtLinkAshan);
            Controls.Add(lblStolich);
            Controls.Add(txtPriceStolich);
            Controls.Add(txtPackStolich);
            Controls.Add(txtLinkStolich);
            Controls.Add(lblVK);
            Controls.Add(txtPriceVK);
            Controls.Add(txtPackVK);
            Controls.Add(txtLinkVK);
            Controls.Add(lblNovus);
            Controls.Add(txtPriceNovus);
            Controls.Add(txtPackNovus);
            Controls.Add(txtLinkNovus);
            Controls.Add(lblCustomTitle);
            Controls.Add(txtCustomSourceName);
            Controls.Add(txtCustomPrice);
            Controls.Add(txtCustomPack);
            Controls.Add(btnAddCustom);
            Controls.Add(btnEditCustom);
            Controls.Add(lstManualSources);
            Controls.Add(btnRemoveCustom);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Name = "AddProductForm";
            Text = "Додати/Редагувати продукт";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}