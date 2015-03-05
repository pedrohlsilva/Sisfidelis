using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SisFidelis
{
    public partial class Form4 : Form
    {
        private static string conexao = "Data Source=Banco.db";
        public Form4()
        {
            InitializeComponent();
            carregarUsuarios("");
        }
        public class Usuario
        {
            public int      id      { get; set; }
            public String   nome    { get; set; }
            public Boolean  adm     { get; set; }

            public Usuario()
            {
                this.nome = "";
                this.adm = false;
                this.id = 0;
            }
        }

        public SQLiteCommand comandoDeletarUsuario(SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("DELETE FROM USUARIO where id = @id", conn);
            cmd.Parameters.AddWithValue("id", Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
            return cmd;
        }

        public List<Usuario> sqlListaUsuario(SQLiteConnection conn, string where)
        {
            SQLiteCommand cmd;
            if (where == "")
                cmd = new SQLiteCommand("SELECT * FROM USUARIO ", conn);
            else
            {
                cmd = new SQLiteCommand("SELECT * FROM USUARIO where nome like @where", conn);
                cmd.Parameters.AddWithValue("where", "%" + where + "%");
            }
            try
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                List<Usuario> lista = new List<Usuario>();
                while (dr.Read())
                {
                    lista.Add(new Usuario
                    {
                        id = Convert.ToInt32(dr["id"]),
                        nome = dr["nome"].ToString(),
                        adm = Convert.ToBoolean(dr["adm"])
                    });
                }
                return lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        private void carregarUsuarios(String where)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            List<Usuario> lista = sqlListaUsuario(conn, where);
            dataGridView1.DataSource = lista;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string busca = textBox1.Text.ToString();
            carregarUsuarios(busca);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmd = null;
            string where = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            try
            {
                cmd = comandoDeletarUsuario(cmd, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Usuário excluido com sucesso!");
                carregarUsuarios("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
        }
    }
}
