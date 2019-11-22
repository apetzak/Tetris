using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Data.SQLite;

namespace Tetris
{
    public class Score
    {
        public int Value { get; set; }
        public int Lines { get; set; }
        public int Level { get; set; }
        public int Time { get; set; }
        public int ID { get; set; }
        public int DelayTicks;
        public MainWindow MW;

        public Score()
        {
            Reset();
        }

        public void Reset()
        {
            Value = 0;
            Lines = 0;
            Level = 0;
            Time = 0;
            if (ID > 0)
                ID += 1;
            DelayTicks = 30;
            if (MW == null)
                return;
            MW.lblScore.Content = "Score: 000000";
            MW.lblLevel.Content = "Level: 000000";
            MW.lblLines.Content = "Lines: 000000";
            MW.lblTime.Content = "Time: 000000";
        }

        public void Update(int lines)
        {
            Lines += lines;
            int prevLevel = Level;
            Level = Lines / 10;
            if (prevLevel != Level && DelayTicks > 4)
                DelayTicks -= 3;
            int amount = lines == 1 ? 40 : lines == 2 ? 100 : lines == 3 ? 300 : lines == 4 ? 400 : 0;        
            amount *= (Level + 1);
            Value += amount;
            UpdateLabel(MW.lblScore, Value);
            UpdateLabel(MW.lblLevel, Level);
            UpdateLabel(MW.lblLines, Lines);
        }

        public void UpdateLabel(Label lbl, int value)
        {
            lbl.Content = string.Format("{0}: {1}{2}", lbl.Name.ToString().Remove(0, 3), "000000".Remove(0, value.ToString().Length), value++);
        }

        public static SQLiteConnection dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
        public static SQLiteCommand command = new SQLiteCommand();
        public static string Sql = "";

        public void Save()
        {
            try
            {
                string sql = string.Format("INSERT INTO Scores (Score, Lines, Level, Time) VALUES ({0}, {1}, {2}, {3})", Value, Lines, Level, Time - 1);
                dbConnection.Open();
                command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }

        public static List<Score> LoadAll()
        { 
            dbConnection.Open();
            Sql = "select * from Scores";
            command = new SQLiteCommand(Sql, dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            var list = new List<Score>();
            while (reader.Read())
            {
                Score s = new Score();
                s.ID = reader.GetInt32(0);
                s.Value = reader.GetInt32(1);
                s.Lines = reader.GetInt32(2);
                s.Level = reader.GetInt32(3);
                s.Time = reader.GetInt32(4);
                list.Add(s);
            }
            dbConnection.Close();
            return list.OrderByDescending(o => o.Value).ToList();
        }
    }
}
