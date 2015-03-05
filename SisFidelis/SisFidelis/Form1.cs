using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using Excel = Microsoft.Office.Interop.Excel;

namespace SisFidelis
{
    public partial class Form1 : Form
    {   
        private static string   conexao = "Data Source=Banco.db";
        private static string   nomeBanco = "Banco.db";
        private static int      idRegistro = 0;
        public  static Boolean  logado = false;
        public  static String   dataCad = "";
        public  static Boolean  adm = false;
        Boolean primeiroAcesso = false;
        DateTime data = DateTime.Now;

        public SQLiteCommand comandoProcuraUsuarioPrimeiroAcesso(SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("SELECT COUNT(*) FROM USUARIO", conn);
            return cmd;
        }

        public SQLiteCommand comandoProcuraUsuarioAdm(SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("SELECT COUNT(*) FROM USUARIO WHERE ", conn);
            return cmd;
        }
        
        public void pegarDadosClienteDoDatagrid (Cliente cliente)
        {
            if (textBox1.Text != String.Empty)
                cliente.nome = textBox1.Text.ToString();
            if (textBox2.Text != String.Empty)
                cliente.dia = Convert.ToInt32(textBox2.Text.ToString());
            if (textBox3.Text != String.Empty)
                cliente.mes = Convert.ToInt32(textBox3.Text.ToString());
            if (textBox8.Text != String.Empty)
                cliente.cpf = textBox8.Text.ToString();
            if (textBox4.Text != String.Empty)
                cliente.cidade = textBox4.Text.ToString();
            if (checkBox1.Checked == true)
                cliente.tamanho = 1;
            //caso o cliente tenha tamanho especial, ao marcar o checkBox, terá valor 1, referencia para tamanho grande
            else
                cliente.tamanho = 0;
            //caso o cliente tenha tamanho especial, ao marcar o checkBox, terá valor 0, referencia para tamanho comum
            if (textBox7.Text != String.Empty)
                cliente.email = textBox7.Text.ToString();
            if (textBox6.Text != String.Empty)
                cliente.fixo = textBox6.Text.ToString();
            if (textBox5.Text != String.Empty)
                cliente.celular = textBox5.Text.ToString();
        }

        public void gravarExcel()
        {
            SaveFileDialog salvar = new SaveFileDialog();

            Excel.Application   app;
            Excel.Workbook      workbook;
            Excel.Worksheet     worksheet;

            object misValue = System.Reflection.Missing.Value;

            app         = new Excel.Application();
            workbook    = app.Workbooks.Add(misValue);
            worksheet   = (Excel.Worksheet)workbook.Worksheets.get_Item(1);

            //percorrendo o dataGridview
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    DataGridViewCell cell = dataGridView1[j, i];
                    worksheet.Cells[i + 1, j + 1] = cell.Value;
                }
            }

            salvar.Title = "Exportar para excel";
            salvar.Filter = "Arquivo do excel *.xls | *.xls";
            salvar.ShowDialog();

            //salvando de fato
            try
            {
                workbook.SaveAs(salvar.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
                                Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                workbook.Close(true, misValue, misValue);
                app.Quit();
                MessageBox.Show("Exportado com sucesso!");
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBox.Show("Não foi salvo o xls!");
            }
        }

        public void fazerBackup()
        {
            string fileName = "test.txt";
            string sourcePath = @"C:\Users\Public\TestFolder";
            string targetPath = @"C:\Users\Public\TestFolder\SubDir";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public Boolean comparaData()
        {
            if (dataCad.Equals(data.ToString("yyyy-MM-dd")))
                return true;
            return false;
        }
        //comando de manipulação de cliente
        public SQLiteCommand comandoInsertCliente(Cliente cliente, SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("INSERT INTO CLIENTES (nome, dia, mes, cidade, celular, fixo, tamanho, email, cpf, datacadastro)" +
            "VALUES (@nome, @dia, @mes, @cidade, @celular, @fixo, @tamanho, @email, @cpf, @data)", conn);
            cmd.Parameters.AddWithValue("nome", cliente.nome);
            cmd.Parameters.AddWithValue("dia", cliente.dia);
            cmd.Parameters.AddWithValue("mes", cliente.mes);
            cmd.Parameters.AddWithValue("cidade", cliente.cidade);
            cmd.Parameters.AddWithValue("celular", cliente.celular);
            cmd.Parameters.AddWithValue("fixo", cliente.fixo);
            cmd.Parameters.AddWithValue("tamanho", cliente.tamanho);
            cmd.Parameters.AddWithValue("email", cliente.email);
            cmd.Parameters.AddWithValue("cpf", cliente.cpf);
            cmd.Parameters.AddWithValue("data", data.ToString("yyyy-MM-dd"));
            return cmd;
        }
        public SQLiteCommand comandoDeletarCliente (SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("DELETE FROM CLIENTES where id = @id", conn);
            cmd.Parameters.AddWithValue("id", Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));
            return cmd;
        }
        public SQLiteCommand comandoAtualizarCliente(Cliente cliente, SQLiteCommand cmd, SQLiteConnection conn)
        {
            cmd = new SQLiteCommand("UPDATE CLIENTES SET nome = @nome, dia = @dia," +
                                    "mes = @mes, cidade = @cidade, celular = @celular," +
                                    "fixo = @fixo, tamanho = @tamanho," + 
                                    "email = @email, cpf = @cpf " +
                                    "WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("nome", cliente.nome);
            cmd.Parameters.AddWithValue("dia", cliente.dia);
            cmd.Parameters.AddWithValue("mes", cliente.mes);
            cmd.Parameters.AddWithValue("cidade", cliente.cidade);
            cmd.Parameters.AddWithValue("celular", cliente.celular);
            cmd.Parameters.AddWithValue("fixo", cliente.fixo);
            cmd.Parameters.AddWithValue("tamanho", cliente.tamanho);
            cmd.Parameters.AddWithValue("email", cliente.email);
            cmd.Parameters.AddWithValue("cpf", cliente.cpf);
            cmd.Parameters.AddWithValue("id", idRegistro);
            return cmd;
        }
        
        public class Cliente
        {
            public int      id              { get; set; }
            public string   nome            { get; set; }
            public int      dia             { get; set; }
            public int      mes             { get; set; }
            public String   cidade          { get; set; }
            public String   cpf             { get; set; }
            public String   celular         { get; set; }
            public String   fixo            { get; set; }
            public String   email           { get; set; }
            public int      tamanho         { get; set; }
            public int      sequencia       { get; set; }
            public String   dataCadastro    { get; set; }

            public Cliente()
            {
                this.id             = 0;
                this.nome           = "";
                this.dia            = 0;
                this.mes            = 0;
                this.cidade         = "";
                this.cpf            = "";
                this.celular        = "";
                this.fixo           = "";
                this.email          = "";
                this.tamanho        = 0;
                this.dataCadastro   = "";
                this.sequencia      = 0;
            }

            public Boolean vazio()
            {
                if(this.id == 0 && this.nome.Equals("")
                    && this.dia == 0 && this.mes == 0 
                    && this.cidade.Equals("") && this.cpf.Equals("") 
                    && this.celular.Equals("") && this.fixo.Equals("")
                    && this.email.Equals("") && this.cpf.Equals(""))
                        return true;
                return false;
            }

        }

        public List<Cliente> sqlListaCliente(SQLiteConnection conn)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM CLIENTES ", conn);
            try
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
            
                List<Cliente> lista = new List<Cliente>();
                while (dr.Read())
                {
                    DateTime date = (DateTime)(dr["datacadastro"]);
                    lista.Add(new Cliente
                    {
                        nome = dr["nome"].ToString(),
                        id = Convert.ToInt32(dr["id"]),
                        dia = Convert.ToInt32(dr["dia"]),
                        mes = Convert.ToInt32(dr["mes"]),
                        cidade = dr["cidade"].ToString(),
                        cpf = dr["cpf"].ToString(),
                        celular = dr["celular"].ToString(),
                        fixo = dr["fixo"].ToString(),
                        email = dr["email"].ToString(),
                        tamanho = Convert.ToInt32(dr["tamanho"]),
                        sequencia = Convert.ToInt32(dr["sequencia"]),
                        dataCadastro = String.Format("{0:yyyy-MM-dd}",date)
                    
                    });
                }
                return lista;
                }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                if (ex.Message.Equals("System.Data.SQLite.SQLiteException"))
                    MessageBox.Show("erro: Banco danificado!");
            }
            return null;
        }
        public List<Cliente> sqlListaClienteWhere(SQLiteConnection conn, String where)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM CLIENTES WHERE mes = @mes", conn);
            cmd.Parameters.AddWithValue("mes", where);
            SQLiteDataReader dr = cmd.ExecuteReader();
            List<Cliente> lista = new List<Cliente>();
            while (dr.Read())
            {
                lista.Add(new Cliente
                {
                    nome = dr["nome"].ToString(),
                    id = Convert.ToInt32(dr["id"]),
                    dia = Convert.ToInt32(dr["dia"]),
                    mes = Convert.ToInt32(dr["mes"]),
                    cidade = dr["cidade"].ToString(),
                    cpf = dr["cpf"].ToString(),
                    celular = dr["celular"].ToString(),
                    fixo = dr["fixo"].ToString(),
                    email = dr["email"].ToString(),
                    tamanho = Convert.ToInt32(dr["tamanho"])
                });
            }
            return lista;
        }

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Text = "Cadastrar";

            if (!File.Exists(nomeBanco))
            {

                SQLiteConnection.CreateFile(nomeBanco);
                SQLiteConnection conn = new SQLiteConnection(conexao);

                conn.Open();

                String sql = "CREATE TABLE [CLIENTES] ("+
                                  "[id] INTEGER PRIMARY KEY AUTOINCREMENT,"+
                                  "[nome] VARCHAR(50), "+
                                  "[dia] INTEGER, "+
                                  "[mes] INTEGER, "+
                                  "[cidade] VARCHAR(50), "+
                                  "[celular] VARCHAR(20), "+
                                  "[fixo] VARCHAR(20), "+
                                  "[tamanho] INTEGER, "+
                                  "[email] VARCHAR(50), "+
                                  "[cpf] VARCHAR(20), "+
                                  "[sequencia] INTEGER, "+
                                  "[datacadastro] DATE); "+

                                "CREATE TRIGGER update_sequencia "+
                                "AFTER INSERT ON CLIENTES "+
                                "FOR EACH ROW "+
                                "WHEN (NEW.sequencia IS NULL) "+
                                "BEGIN "+
                                "UPDATE CLIENTES SET sequencia = mes || id WHERE id = NEW.id; "+
                                "END;"+

                                "CREATE TABLE USUARIO"+
                                " (  [id] INTEGER PRIMARY KEY AUTOINCREMENT,"+
                                "    [nome] VARCHAR(50), [senha] VARCHAR(50),"+
                                "    [adm] INT(1) DEFAULT 0);";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }catch(Exception ex){
                    MessageBox.Show(ex.Message);
                }
            }
            Carregar();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Cliente cliente = new Cliente();

            Boolean erro = false;
            try{
                pegarDadosClienteDoDatagrid(cliente);
            }catch(Exception){
                MessageBox.Show("O campo dia e mês deverão ser do tipo numerico!");
                erro = true;
            }

            if (!erro)
            {
                label11.Text = "";
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SQLiteCommand cmdCadastrar = null;
                cliente.id = idRegistro;
                if (idRegistro == 0) //cadastrar cliente novo sem id
                {
                    cmdCadastrar = comandoInsertCliente(cliente, cmdCadastrar, conn);
                }
                else //atualizar registro de cliente
                {
                    cmdCadastrar = comandoAtualizarCliente(cliente, cmdCadastrar, conn);
                }
                if (!cliente.vazio())
                {
                    try
                    {
                        cmdCadastrar.ExecuteNonQuery();
                        label11.Text = "Registro Salvo com sucesso!";
                        limparCampo();
                        Carregar();
                        idRegistro = 0;
                    }
                    catch(SQLiteException)
                    {
                        label11.Text = "Banco corrompido!";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        label11.Text = "Erro ao salvar o registro "+ex;
                    }
                }
                else
                {
                    label11.Text = "Não existem dados para cadastrar!";
                    limparCampo();
                }
                button1.Text = "Cadastrar";
            }
        }


        private void Carregar()
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            List<Cliente> lista = sqlListaCliente(conn);

            dataGridView1.DataSource = lista;
        }
        private void CarregarWhere(List<Cliente> lista)
        {
            dataGridView1.DataSource = lista;
        }
        private void limparCampo()
        {
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            textBox3.Text = String.Empty;
            textBox4.Text = String.Empty;
            textBox5.Text = String.Empty;
            textBox6.Text = String.Empty;
            textBox7.Text = String.Empty;
            textBox8.Text = String.Empty;
            checkBox1.Checked = false;
        }
        private void povoarCampo()
        {
            textBox1.Text       = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox2.Text       = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox3.Text       = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox4.Text       = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox5.Text       = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            textBox6.Text       = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            textBox7.Text       = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            textBox8.Text       = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            checkBox1.Checked   = Convert.ToBoolean(dataGridView1.CurrentRow.Cells[9].Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
            dataCad = dataGridView1.CurrentRow.Cells[11].Value.ToString().Substring(0,10);
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SQLiteCommand cmdDeletar = null;

            if (logado || comparaData())
            {
                try
                {
                    cmdDeletar = comandoDeletarCliente(cmdDeletar, conn);
                    cmdDeletar.ExecuteNonQuery();
                    label11.Text = "Registro removido com sucesso!";
                    limparCampo();
                    Carregar();
                }
                catch (System.NullReferenceException)
                {
                    label11.Text = "Não há registro a ser removido!";
                }
                catch (Exception ex)
                {
                    label11.Text = "Erro ao remover: " + ex.Message;
                }
            }
            else
                logar();
            }
            catch (NullReferenceException)
            {
                label11.Text = "Não é possível excluir um objeto que não existe!";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (idRegistro > 0)
            {
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                SQLiteCommand cmdAtualizar = null;
                Cliente cliente = new Cliente();
                pegarDadosClienteDoDatagrid(cliente);
                cmdAtualizar = comandoAtualizarCliente(cliente ,cmdAtualizar, conn);
                try
                {
                    cmdAtualizar.ExecuteNonQuery();
                    label11.Text = "Registro atualizado com sucesso!";
                    limparCampo();
                    Carregar();
                    idRegistro = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao Atualizar registro: " + ex.Message);
                }
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            button1.Text = "Atualizar";
            try
            {
                idRegistro  = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
                dataCad     = dataGridView1.CurrentRow.Cells[11].Value.ToString();
                povoarCampo();
            }
            catch (Exception ex)
            {
                label11.Text = "Nenhum registro foi selecionado!"+ex.Message;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            idRegistro = 0;
            button1.Text = "Cadastrar";
            limparCampo();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (logado)
            {
                SaveFileDialog salvar = new SaveFileDialog();
                salvar.Title = "Salvando o banco de dados";
                salvar.ShowDialog();
                if(salvar.FileName != ""){
                    File.Copy(nomeBanco, salvar.FileName, true);
                    MessageBox.Show("O arquivo foi salvo em: "+salvar.FileName);
                }
            }
            else
            {
                logar();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int mesBusca = 0;
            try
            {
                mesBusca = Convert.ToInt32(textBox9.Text.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("correct format"))
                    label11.Text = "Filtro deverá ser do tipo numerico!";
                
            }

            if (mesBusca != 0 && textBox9.Text.ToString() != "")
            {
                label11.Text = "";
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                List<Cliente> lista = sqlListaClienteWhere(conn, mesBusca.ToString());
                CarregarWhere(lista);
            }
            else
            {
                Carregar();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            gravarExcel();
        }

        private void logar()
        {
            Form2 form = new Form2();
            form.ShowDialog(this);
            if (logado)
            {
                label13.Text = "Logado!";
                if (adm)
                    label13.Text = "Adm Logado!";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            logado = false;
            adm = false;
            label13.Text = "Não Logado!";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (logado && adm)
            {

                Stream myStream = null;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if ((myStream = openFileDialog1.OpenFile()) != null)
                        {
                            using (myStream)
                            {
                                // Insert code to read the stream here.
                                File.Copy(openFileDialog1.FileName.ToString(), nomeBanco, true);
                                label11.Text = "O arquivo foi importado com sucesso!";
                            }
                        }
                    }
                    catch (Exception)
                    {
                        label11.Text = "Erro: Não foi possível abrir o arquivo.";
                    }
                }
            }
            else
            {
                logar();
            }

            /*
            SaveFileDialog salvar = new SaveFileDialog();
            salvar.Title = "Importando o banco de dados";
            salvar.ShowDialog();
            if (salvar.FileName != "")
            {
                File.Copy(salvar.FileName, nomeBanco, true);
                MessageBox.Show("O arquivo foi importado com sucesso!");
            }
            else
            {
                MessageBox.Show("Contate o administrador!");
            }
             */
            Carregar();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SQLiteCommand cmd = null;
            SQLiteConnection conn = new SQLiteConnection(conexao);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            //checa se é o primeiro acesso ao sistema
            try
            {
                cmd = comandoProcuraUsuarioPrimeiroAcesso(cmd, conn);
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    primeiroAcesso = true;
                }
                else
                {
                    primeiroAcesso = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //fim checagem do primeiro acesso

            if (logado && adm || primeiroAcesso)
            {
                Form3 form = new Form3();
                form.ShowDialog();
            }
            else
            {
                logar();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (logado && adm)
            {
                Form4 form = new Form4();
                form.ShowDialog(this);
            }
            else
            {
                label11.Text = "Somente administradores!";
                logar();
            }
        }

    }
}