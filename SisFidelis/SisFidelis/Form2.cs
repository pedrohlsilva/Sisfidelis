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
    public partial class Form2 : Form
    {
        String conexao = "Data Source=Banco.db";
        public class Usuario
        {
            public int id { get; set; }
            public String nome { get; set; }
            public String senha { get; set; }

            public Boolean vazio()
            {
                if (this.nome == "" || this.senha == "")
                {
                    return true;
                }
                return false;
            }
        }

        //comando de manipulação de usuário do sistema
        /*public SQLiteCommand comandoInsertUsuario(SisFidelis.Form1.Usuario usuario, SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("INSERT INTO USUARIOS (nome, senha) " +
            "VALUES (@nome, @senha)", conn);
            cmd.Parameters.AddWithValue("nome", usuario.nome);
            cmd.Parameters.AddWithValue("", usuario.senha);
            return cmd;
        }
        public SQLiteCommand comandoDeletarUsuario(SisFidelis.Form1.Usuario usuario, SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("DELETE FROM USUARIOS where id = @id", conn);
            cmd.Parameters.AddWithValue("id", usuario.id);
            return cmd;
        }*/

        public SQLiteCommand comandoProcuraUsuario(SQLiteCommand cmd, Usuario usuario, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("SELECT COUNT(*) FROM USUARIO WHERE nome = @nome AND senha = @senha", conn);
            cmd.Parameters.AddWithValue("nome", usuario.nome);
            cmd.Parameters.AddWithValue("senha", usuario.senha );
            return cmd;
        }

        public SQLiteCommand comandoProcuraUsuarioAdm(SQLiteCommand cmd, Usuario usuario, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("SELECT COUNT(*) FROM USUARIO WHERE nome = @nome AND senha = @senha AND adm = @adm", conn);
            cmd.Parameters.AddWithValue("nome", usuario.nome);
            cmd.Parameters.AddWithValue("senha", usuario.senha);
            //1 é o numero para adm! Magic Numbers
            cmd.Parameters.AddWithValue("adm", "1");
            return cmd;
        }

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            Usuario usuario = new Usuario();
            usuario.nome = textBox1.Text.ToString();
            usuario.senha = textBox2.Text.ToString();

            SQLiteCommand cmd = new SQLiteCommand();
            cmd = comandoProcuraUsuario(cmd, usuario, conn);
            
            try
            {
                cmd = comandoProcuraUsuario(cmd, usuario, conn);

                int qtdUsuario = Convert.ToInt32(cmd.ExecuteScalar());
                
                if(qtdUsuario > 0)
                {
                    Form1.logado = true;
                    cmd = comandoProcuraUsuarioAdm(cmd, usuario, conn);
                    qtdUsuario = 0;
                    qtdUsuario = Convert.ToInt32(cmd.ExecuteScalar());
                    if (qtdUsuario > 0)
                        Form1.adm = true;
                    else
                        MessageBox.Show("Usuário sem permissão para cadastro de novos usuários!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cadastre o usuário inicialmente!");
                }
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch(SQLiteException ex){
                MessageBox.Show(ex.Message);
            }
        }
    }
}
