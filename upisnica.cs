using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace EsDnevnik
{
    public partial class upisnica : Form
    {
        DataTable dt_upisnica;
        public upisnica()
        {
            InitializeComponent();
        }

        private void cmb_godina_populate()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("Select * from skolska_godina", veza);
            DataTable dt_godina = new DataTable();
            adapter.Fill(dt_godina);
            cmb_godina.DataSource = dt_godina;
            cmb_godina.ValueMember = "id";
            cmb_godina.DisplayMember = "naziv";
            cmb_godina.SelectedValue = 2;

        }
        private void cmb_godina_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmb_godina.IsHandleCreated && cmb_godina.Focused)
            {
                cmb_odeljenje_populate();
                txt_upisnica_id.Text = "";

                while (grid_upisnica.Rows.Count > 0)
                {
                    grid_upisnica.Rows.Remove(grid_upisnica.Rows[0]);
                }

                cmb_ucenik.SelectedIndex = -1;
                cmb_odeljenje.SelectedIndex = -1;
                cmb_ucenik.Enabled = false;

            }
        }

        private void cmb_odeljenje_populate()
        {
            string godina = cmb_godina.SelectedValue.ToString();
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("Select id, str(razred) + '-' + indeks as naziv from odeljenje where godina_id =" + godina, veza);
            DataTable dt_odeljenje = new DataTable();
            adapter.Fill(dt_odeljenje);
            cmb_odeljenje.DataSource = dt_odeljenje;
            cmb_odeljenje.ValueMember = "id";
            cmb_odeljenje.DisplayMember = "naziv";
        }
        private void cmb_odeljenje_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmb_odeljenje.IsHandleCreated && cmb_odeljenje.Focused)
            {
                cmb_ucenik_populate();
                cmb_ucenik.Enabled = true;
                Grid_populate();

            }
        }

        private void cmb_ucenik_populate()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("select id, ime + ' ' +prezime as naziv from osoba where uloga = 1", veza);
            DataTable dt_ucenik = new DataTable();
            adapter.Fill(dt_ucenik);
            cmb_ucenik.DataSource = dt_ucenik;
            cmb_ucenik.ValueMember = "id";
            cmb_ucenik.DisplayMember = "naziv";

        }

        private void Grid_populate()
        {
            SqlConnection veza = Konekcija.Connect();
            SqlDataAdapter adapter = new SqlDataAdapter("Select upisnica.id as id, ime + ' ' + prezime as naziv, osoba.id as ucenik from upisnica join osoba on osoba_id=osoba.id where odeljenje_id = " + cmb_odeljenje.SelectedValue.ToString(), veza);
            dt_upisnica = new DataTable();
            adapter.Fill(dt_upisnica);
            grid_upisnica.DataSource = dt_upisnica;
            grid_upisnica.AllowUserToAddRows = false;
            grid_upisnica.Columns["ucenik"].Visible = false;
        }
        private void upisnica_Load(object sender, EventArgs e)
        {
            cmb_godina_populate();
            cmb_odeljenje_populate();
            cmb_odeljenje.SelectedIndex = -1;
            cmb_ucenik.Enabled = false;
            txt_upisnica_id.Enabled = false;
            

        }

        private void grid_upisnica_CurrentCellChanged(object sender, EventArgs e)
        {
            if (grid_upisnica.CurrentRow != null)
            {
                int broj_sloga = grid_upisnica.CurrentRow.Index;
                if (dt_upisnica.Rows.Count != 0 && broj_sloga >= 0)
                {
                    cmb_ucenik.SelectedValue = grid_upisnica.Rows[broj_sloga].Cells["ucenik"].Value.ToString();
                    txt_upisnica_id.Text = grid_upisnica.Rows[broj_sloga].Cells["id"].Value.ToString();

                }
            }
        }

        private void btn_insert_Click(object sender, EventArgs e)
        {
            StringBuilder naredba = new StringBuilder("Insert into upisnica (odeljenje_id, osoba_id) values('");
            naredba.Append(cmb_odeljenje.SelectedValue.ToString() + "', '");
            naredba.Append(cmb_ucenik.SelectedValue.ToString() + "')");
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda = new SqlCommand(naredba.ToString(), veza);
            try
            {
                veza.Open();
                komanda.ExecuteNonQuery();
                veza.Close();
                Grid_populate();
            }
            catch (Exception Greska)
            {
                MessageBox.Show(Greska.Message);
            }

        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            StringBuilder naredba = new StringBuilder("Update upisnica set ");
            naredba.Append(" osoba_id = '" + cmb_ucenik.SelectedValue.ToString() + "'");
            naredba.Append(" where id = " + txt_upisnica_id.Text);
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda = new SqlCommand(naredba.ToString(), veza);
            try
            {
                veza.Open();
                komanda.ExecuteNonQuery();
                veza.Close();
                Grid_populate();
            }
            catch (Exception Greska)
            {
                MessageBox.Show(Greska.Message);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            string naredba = "Delete from upisnica where id = " + txt_upisnica_id.Text;
            SqlConnection veza = Konekcija.Connect();
            SqlCommand komanda = new SqlCommand(naredba, veza);
            try
            {
                veza.Open();
                komanda.ExecuteNonQuery();
                veza.Close();
                Grid_populate();
            }
            catch (Exception Greska)
            {
                MessageBox.Show(Greska.Message);
            }
        }
    }
}
