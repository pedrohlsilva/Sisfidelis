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
    public partial class Form3 : Form
    {
        String conexao = "Data Source=Banco.db";
        SQLiteConnection conn;

        private void connex()
        {
            this.conn = 
                this.conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        public SQLiteCommand comandoInsertUsuario(String nome, String senha, SQLiteCommand cmd, SQLiteConnection conn, int adm)
        {
            cmd = new SQLiteCommand("INSERT INTO USUARIO (nome, senha, adm)" +
            "VALUES (@nome, @senha, @adm)", conn);
            cmd.Parameters.AddWithValue("nome", nome);
            cmd.Parameters.AddWithValue("senha", senha);
            cmd.Parameters.AddWithValue("adm", adm);
            return cmd;
        }

        public SQLiteCommand comandoProcuraUsuarioAdm(SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("SELECT COUNT(*) FROM USUARIO WHERE adm = @adm", conn);
            //1 é o numero para adm! Magic Numbers
            cmd.Parameters.AddWithValue("adm", "1");
            return cmd;
        }

        public SQLiteCommand comandoProcuraUsuario(SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("SELECT COUNT(*) FROM USUARIO", conn);
            return cmd;
        }

        public Form3()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connex();
            String nome = textBox1.Text.ToString();
            String senha = textBox2.Text.ToString();
            SQLiteCommand cmd = null;
            try
            {
                //checar se é primeiro acesso
                cmd = comandoProcuraUsuarioAdm(cmd, conn);
                int qtdUsuario = Convert.ToInt32(cmd.ExecuteScalar());
                if (qtdUsuario > 0)
                {
                    //0 é o numero para usuário comum
                    cmd = comandoInsertUsuario(nome, senha, cmd, conn, 0);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Inserido usuário comum!");
                }
                //Fim checagem
                else
                {
                    //1 é o numero para adm
                    cmd = comandoInsertUsuario(nome, senha, cmd, conn, 1);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Inserido usuário Adm!");
                }
                MessageBox.Show("Sucesso!");
                this.Close();
            }catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }
    }
}
