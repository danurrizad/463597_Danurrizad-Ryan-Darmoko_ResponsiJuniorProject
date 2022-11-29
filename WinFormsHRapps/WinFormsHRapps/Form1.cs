using System.Data;
using NpgsqlTypes;
using Npgsql;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace WinFormsHRapps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection conn;
        string connstring = "Host=localhost;Port=2022;Username=postgres;Password=informatika;Database=dbHRapps";
        public DataTable dt;
        public static NpgsqlCommand cmd;
        private string sql = null;
        private DataGridViewRow r;

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(connstring);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                conn.Open();
                dgvData.DataSource = null;
                sql = "select id_karyawan, nama, karyawan.id_dep, nama_dep from karyawan left join departemen ON departemen.id_dep = karyawan.id_dep";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                dgvData.DataSource = dt;
                conn.Close();
            }

            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Fail to load the data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                r = dgvData.Rows[e.RowIndex];
                tbNama.Text = r.Cells["_nama"].Value.ToString();
                cbDept.Text = r.Cells["_departemen"].Value.ToString();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from st_insert(:_nama,:_departemen)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_nama", tbNama.Text);
                cmd.Parameters.AddWithValue("_departemen", cbDept.Text);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Data karyawan berhasil diinput", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    LoadData();
                    tbNama.Text = cbDept.Text = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Data karyawan gagal diinput", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon pilih baris terlebih dahulu", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                conn.Open();
                sql = @"select * from st_update(:_id_dep,:_nama,:_departemen)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_nama", tbNama.Text);
                cmd.Parameters.AddWithValue("_departemen", cbDept.Text);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Data karyawan berhasil diedit", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    LoadData();
                    tbNama.Text = cbDept.Text = null;
                    r = null;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Data karyawan tidak berhasil diedit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon pilih baris terlebih dahulu", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if(MessageBox.Show("Apakah anda yakin akan menghapus" + r.Cells["_nama"].Value.ToString() + "?" , "Konfirmasi hapus", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                try
                {
                    conn.Open();
                    sql = @"select * from st_delete(:_id_karyawan)";
                    cmd = new NpgsqlCommand(sql, conn);
                   //cmd.Parameters.AddWithValue("_id_karyawan", r.Cells["_id_karyawan"].ValueToString());
                    if ((int)cmd.ExecuteScalar() == 1)
                    {
                        MessageBox.Show("Data karyawan berhasil dihapus", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        conn.Close();
                        LoadData();
                        tbNama.Text = cbDept.Text = null;
                        r = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message, "Data karyawan tidak berhasil dihapus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}