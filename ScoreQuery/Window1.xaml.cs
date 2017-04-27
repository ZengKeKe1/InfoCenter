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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ScoreQuery
{
    public class Subject
    {
        public string SubjectName { get; set; }
        public string Server { get; set; }
        
        public async Task<List<Score>> GetScore(String username)
        {
            var connString = "Database='高数成绩';Data Source=SERVER;User ID=usr;Password=user123456;CharSet=utf8";
            connString = connString.Replace("SERVER", Server);
            
            var sqlConnection = new MySqlConnection(connString);

            
            try
            {
                await sqlConnection.OpenAsync();
            }
            catch (Exception)
            {
                sqlConnection.Close();
                throw;
            }
            
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "SELECT * FROM 高数成绩 WHERE 学号=@user";
            sqlCommand.Parameters.Add(new MySqlParameter("@user", username));
            //sqlCommand.Parameters.Add(new MySqlParameter("@table", SubjectName));

            DbDataReader reader = null;

            try
            {
                reader = await sqlCommand.ExecuteReaderAsync();
                var a = reader.HasRows;
                await reader.ReadAsync();
            }
            catch (Exception)
            {
                sqlConnection.Close();
                throw;
            }

            if (reader == null) return null;

            var count = reader.FieldCount;
            var scores = new List<Score>();
            int i;
            for (i = 3; i < count; i++)
            {
                if (reader.IsDBNull(i)) continue;
                var score = new Score()
                {
                    Name = reader.GetString(1),
                    Description = reader.GetName(i),
                    Points = reader.GetInt32(i)
                };

                scores.Add(score);
            }
            sqlConnection.Close();
            return scores;
        }
    }
    public class Score
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
    }
    
    
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public Window1()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var subject = new Subject()
            {
                Server = this.Server,
                SubjectName = "高数成绩"
            };

            List<Score> scores = null;

            try
            {
                scores = await subject.GetScore(Username); 
            }
            catch (DbException)
            {
                MessageBox.Show("服务器连接失败！");
                return;
            }

            if (scores == null || scores.Count ==0)
            {
                MessageBox.Show("您好像没有成绩...");
            }
            else
            {
                List.ItemsSource = scores;
                this.DataContext = scores[1];
            }
            
        }
    }
}
