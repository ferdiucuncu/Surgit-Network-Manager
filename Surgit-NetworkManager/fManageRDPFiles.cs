﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Syncfusion.WinForms.Controls;

#region COPYRIGHT NOTICE (Surgit Network Manager - Copyright(C) 2020  Tobias Hattinger)

/* Surgit Network Manager
 * Copyright(C) 2020  Tobias Hattinger
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */
#endregion

namespace Surgit_NetworkManager
{
#pragma warning disable IDE1006
    public partial class ManageRDPFiles : SfForm
    {
        public string MACAddress { get; set; } = "";

        private readonly CSQLite sql = new CSQLite(SurgitManager.SurgitDatabaseLocation);

        public ManageRDPFiles()
        {
            InitializeComponent();
         
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ManageRDPFiles_Load(object sender, EventArgs e)
        {
            DisableInput();
            LoadEntries();
        }

        public void LoadEntries()
        {
            lbxRDPFiles.DataSource = null;
            lbxRDPFiles.Items.Clear();

            lbxRDPFiles.ValueMember = "ID";
            lbxRDPFiles.DisplayMember = "DispVal";
            lbxRDPFiles.DataSource = sql.CreateDT($"SELECT *, Name || ' - ' || Path AS DispVal FROM RDPFiles WHERE MACAddress = '{MACAddress}'");
        }

        private void lbxRDPFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            sql.connection.Open();
            using(SQLiteDataReader reader = sql.ExecuteQuery($"SELECT * FROM RDPFiles WHERE ID = '{lbxRDPFiles.SelectedValue}'"))
            {
                while(reader.Read())
                {
                    txbName.Text = Convert.ToString(reader["Name"]);
                    txbRDPPath.Text = Convert.ToString(reader["Path"]);
                }
            }
            sql.connection.Close();

            EnableInput();
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            txbRDPPath.Text = "";
            txbName.Text = "";
            lbxRDPFiles.SelectedIndex = -1;

            DisableInput();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if(ofdOpenRDPFile.ShowDialog() == DialogResult.OK)
            {
                txbRDPPath.Text = ofdOpenRDPFile.FileName;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            sql.ExecuteNonQueryA($"DELETE FROM RDPFiles WHERE ID = '{lbxRDPFiles.SelectedValue}'");
            LoadEntries();
            lbxRDPFiles.SelectedIndex = -1;
            DisableInput();
        }

        private void EnableInput()
        {
            btnDelete.Enabled = true;
            btnDiscard.Enabled = true;
            btnUpdate.Enabled = true;
            btnSelectFile.Enabled = true;
            txbName.Enabled = true;
            txbRDPPath.Enabled = true;
        }

        private void DisableInput()
        {
            btnDelete.Enabled = false;
            btnDiscard.Enabled = false;
            btnUpdate.Enabled = false;
            btnSelectFile.Enabled = false;
            txbName.Enabled = false;
            txbRDPPath.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txbName.Text))
            {
                if (!string.IsNullOrEmpty(txbRDPPath.Text))
                {
                    sql.ExecuteNonQueryA($"UPDATE RDPFiles SET Name = '{txbName.Text}', Path = '{txbRDPPath.Text}' WHERE ID = '{lbxRDPFiles.SelectedValue}'");
                    LoadEntries();
                    lbxRDPFiles.SelectedIndex = -1;
                    DisableInput();
                }
                else MessageBox.Show("Please select a valid RDP-File", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else MessageBox.Show("Please enter a Name for the RDP-Connection", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
    }
#pragma warning restore IDE1006
}
