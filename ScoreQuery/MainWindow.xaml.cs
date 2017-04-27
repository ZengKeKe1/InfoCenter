using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace ScoreQuery
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string server;
        public string Server
        {
            get => server;
            set
            {
                var regex = new System.Text.RegularExpressions.Regex(@"^(?:(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]\d|\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]\d|\d)$");        
                if (!regex.IsMatch(value))
                {
                    throw new ArgumentException("invalid ip");
                }
                else
                {
                    server = value;
                }
                
            }
        }
        public string Username { get; set; }
        public string Password { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var connString = "Database='高数成绩';Data Source=SERVER;User ID=root;Password=123456;CharSet=utf8";
            connString = connString.Replace("SERVER", Server);
            var sqlConnection = new MySqlConnection(connString);
            try
            {
                await sqlConnection.OpenAsync();
            }
            catch (Exception)
            {
                MessageBox.Show("服务器连接失败。");
                sqlConnection.Close();
                return;
            }
            

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "SELECT * FROM admin where 学号=@user and 密码=@pass";
            sqlCommand.Parameters.Add(new MySqlParameter("@user", Username));
            sqlCommand.Parameters.Add(new MySqlParameter("@pass", pwd.Password));

            var reader = sqlCommand.ExecuteReader();
            if (!reader.HasRows)
            {
                MessageBox.Show("密码错误!");
                sqlConnection.Close();
            }
            else
            {
                sqlConnection.Close();
                
                var window = new Window1();
                window.Server = Server;
                window.Username = Username.Substring(Username.Length - 4, 4);
                
                window.Show();
                this.Close();
            }
            
        }
    }
}
