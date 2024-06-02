namespace PKHeX.WinForms
{
    partial class SAV_UnityTower
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_SetAllLocations = new System.Windows.Forms.Button();
            B_SetAllLegalLocations = new System.Windows.Forms.Button();
            B_ClearLocations = new System.Windows.Forms.Button();
            CHK_GlobalFlag = new System.Windows.Forms.CheckBox();
            CHK_UnityTowerFlag = new System.Windows.Forms.CheckBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            DGV_Geonet = new System.Windows.Forms.DataGridView();
            DGV_UnityTower = new System.Windows.Forms.DataGridView();
            Item_CountryIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_RegionIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_Region = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_Point = new System.Windows.Forms.DataGridViewComboBoxColumn();
            Item_Floor = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            Item_UnityTowerIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_UnityTower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Geonet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGV_UnityTower).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(435, 381);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 7;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(341, 381);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 6;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_SetAllLocations
            // 
            B_SetAllLocations.Location = new System.Drawing.Point(13, 12);
            B_SetAllLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SetAllLocations.Name = "B_SetAllLocations";
            B_SetAllLocations.Size = new System.Drawing.Size(160, 27);
            B_SetAllLocations.TabIndex = 0;
            B_SetAllLocations.Text = "Set All Locations";
            B_SetAllLocations.UseVisualStyleBackColor = true;
            B_SetAllLocations.Click += B_SetAllLocations_Click;
            // 
            // B_SetAllLegalLocations
            // 
            B_SetAllLegalLocations.Location = new System.Drawing.Point(185, 12);
            B_SetAllLegalLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SetAllLegalLocations.Name = "B_SetAllLegalLocations";
            B_SetAllLegalLocations.Size = new System.Drawing.Size(160, 27);
            B_SetAllLegalLocations.TabIndex = 1;
            B_SetAllLegalLocations.Text = "Set All Legal Locations";
            B_SetAllLegalLocations.UseVisualStyleBackColor = true;
            B_SetAllLegalLocations.Click += B_SetAllLegalLocations_Click;
            // 
            // B_ClearLocations
            // 
            B_ClearLocations.Location = new System.Drawing.Point(357, 12);
            B_ClearLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearLocations.Name = "B_ClearLocations";
            B_ClearLocations.Size = new System.Drawing.Size(160, 27);
            B_ClearLocations.TabIndex = 2;
            B_ClearLocations.Text = "Clear Locations";
            B_ClearLocations.UseVisualStyleBackColor = true;
            B_ClearLocations.Click += B_ClearLocations_Click;
            // 
            // CHK_GlobalFlag
            // 
            CHK_GlobalFlag.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CHK_GlobalFlag.AutoSize = true;
            CHK_GlobalFlag.Location = new System.Drawing.Point(13, 355);
            CHK_GlobalFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_GlobalFlag.Name = "CHK_GlobalFlag";
            CHK_GlobalFlag.Size = new System.Drawing.Size(131, 19);
            CHK_GlobalFlag.TabIndex = 4;
            CHK_GlobalFlag.Text = "Whole Globe Visible";
            CHK_GlobalFlag.UseVisualStyleBackColor = true;
            // 
            // CHK_UnityTowerFlag
            // 
            CHK_UnityTowerFlag.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CHK_UnityTowerFlag.AutoSize = true;
            CHK_UnityTowerFlag.Location = new System.Drawing.Point(144, 355);
            CHK_UnityTowerFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_UnityTowerFlag.Name = "CHK_UnityTowerFlag";
            CHK_UnityTowerFlag.Size = new System.Drawing.Size(141, 19);
            CHK_UnityTowerFlag.TabIndex = 5;
            CHK_UnityTowerFlag.Text = "Unity Tower Unlocked";
            CHK_UnityTowerFlag.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.Location = new System.Drawing.Point(13, 45);
            splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(DGV_Geonet);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(DGV_UnityTower);
            splitContainer1.Size = new System.Drawing.Size(505, 304);
            splitContainer1.SplitterDistance = 336;
            splitContainer1.TabIndex = 3;
            // 
            // DGV_Geonet
            // 
            DGV_Geonet.AllowUserToAddRows = false;
            DGV_Geonet.AllowUserToDeleteRows = false;
            DGV_Geonet.AllowUserToResizeColumns = false;
            DGV_Geonet.AllowUserToResizeRows = false;
            DGV_Geonet.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_Geonet.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_Geonet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Geonet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_CountryIndex, Item_Country, Item_RegionIndex, Item_Region, Item_Point });
            DGV_Geonet.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Geonet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_Geonet.Location = new System.Drawing.Point(0, 0);
            DGV_Geonet.MultiSelect = false;
            DGV_Geonet.Name = "DGV_Geonet";
            DGV_Geonet.RowHeadersVisible = false;
            DGV_Geonet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_Geonet.Size = new System.Drawing.Size(336, 304);
            DGV_Geonet.StandardTab = true;
            DGV_Geonet.TabIndex = 0;
            // 
            // DGV_UnityTower
            // 
            DGV_UnityTower.AllowUserToAddRows = false;
            DGV_UnityTower.AllowUserToDeleteRows = false;
            DGV_UnityTower.AllowUserToResizeColumns = false;
            DGV_UnityTower.AllowUserToResizeRows = false;
            DGV_UnityTower.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_UnityTower.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_UnityTower.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_UnityTower.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_Floor, Item_UnityTowerIndex, Item_UnityTower });
            DGV_UnityTower.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_UnityTower.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_UnityTower.Location = new System.Drawing.Point(0, 0);
            DGV_UnityTower.MultiSelect = false;
            DGV_UnityTower.Name = "DGV_UnityTower";
            DGV_UnityTower.RowHeadersVisible = false;
            DGV_UnityTower.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_UnityTower.Size = new System.Drawing.Size(165, 304);
            DGV_UnityTower.StandardTab = true;
            DGV_UnityTower.TabIndex = 0;
            // 
            // Item_CountryIndex
            // 
            Item_CountryIndex.HeaderText = "CountryIndex";
            Item_CountryIndex.Name = "Item_CountryIndex";
            Item_CountryIndex.ReadOnly = true;
            Item_CountryIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Item_CountryIndex.Visible = false;
            // 
            // Item_Country
            // 
            Item_Country.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Item_Country.FillWeight = 150F;
            Item_Country.HeaderText = "Country";
            Item_Country.MinimumWidth = 50;
            Item_Country.Name = "Item_Country";
            Item_Country.ReadOnly = true;
            Item_Country.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Item_RegionIndex
            // 
            Item_RegionIndex.HeaderText = "RegionIndex";
            Item_RegionIndex.Name = "Item_RegionIndex";
            Item_RegionIndex.ReadOnly = true;
            Item_RegionIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Item_RegionIndex.Visible = false;
            // 
            // Item_Region
            // 
            Item_Region.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Item_Region.FillWeight = 150F;
            Item_Region.HeaderText = "Region";
            Item_Region.MinimumWidth = 50;
            Item_Region.Name = "Item_Region";
            Item_Region.ReadOnly = true;
            Item_Region.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Item_Point
            // 
            Item_Point.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            Item_Point.FillWeight = 50F;
            Item_Point.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Item_Point.HeaderText = "Point";
            Item_Point.MinimumWidth = 50;
            Item_Point.Name = "Item_Point";
            Item_Point.Width = 50;
            // 
            // Item_Floor
            // 
            Item_Floor.HeaderText = "Floor";
            Item_Floor.Name = "Item_Floor";
            Item_Floor.Width = 50;
            // 
            // Item_UnityTowerIndex
            // 
            Item_UnityTowerIndex.HeaderText = "CountryIndex";
            Item_UnityTowerIndex.Name = "Item_UnityTowerIndex";
            Item_UnityTowerIndex.ReadOnly = true;
            Item_UnityTowerIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Item_UnityTowerIndex.Visible = false;
            // 
            // Item_UnityTower
            // 
            Item_UnityTower.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Item_UnityTower.HeaderText = "Country";
            Item_UnityTower.Name = "Item_UnityTower";
            Item_UnityTower.ReadOnly = true;
            Item_UnityTower.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SAV_UnityTower
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(530, 417);
            Controls.Add(CHK_UnityTowerFlag);
            Controls.Add(CHK_GlobalFlag);
            Controls.Add(B_ClearLocations);
            Controls.Add(B_SetAllLegalLocations);
            Controls.Add(B_SetAllLocations);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(splitContainer1);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            Name = "SAV_UnityTower";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Unity Tower Editor";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_Geonet).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGV_UnityTower).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_SetAllLocations;
        private System.Windows.Forms.Button B_SetAllLegalLocations;
        private System.Windows.Forms.Button B_ClearLocations;
        private System.Windows.Forms.CheckBox CHK_GlobalFlag;
        private System.Windows.Forms.CheckBox CHK_UnityTowerFlag;
        private System.Windows.Forms.DataGridView DGV_Geonet;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Point;
        private System.Windows.Forms.DataGridView DGV_UnityTower;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Item_Floor;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_CountryIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_RegionIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_UnityTowerIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_Region;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_UnityTower;
    }
}
