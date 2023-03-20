using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;

namespace TelecomNevaSyazS1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string code;
        int countTime;

        DispatcherTimer disTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            BaseClass.BD = new Entities();
            pbPassword.IsEnabled = false;
            tbCode.IsEnabled = false;
            btnLogin.IsEnabled = false;
            disTimer.Interval = new TimeSpan(0, 0, 1);
            disTimer.Tick += new EventHandler(DisTimer_Tick);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tbNomer.Text = "";
            pbPassword.Password = "";
            tbCode.Text = "";
            code = "";
            tbRemainingTime.Text = "";
            pbPassword.IsEnabled = false;
            tbCode.IsEnabled = false;
            btnLogin.IsEnabled = false;
        }

        private void DisTimer_Tick(object sender, EventArgs e)
        {
            if (countTime == 0) 
            {
                disTimer.Stop();
                code = "";
                tbRemainingTime.Text = "Код не действителен. Запросите повторную отправку кода";

            }
            else
                tbRemainingTime.Text = "Код перестанет быть действительным через " + countTime;
            countTime--;
        }

        private void tbNomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Employees employee = BaseClass.BD.Employees.FirstOrDefault(x => x.Nomer == tbNomer.Text);
                if (employee != null)
                {
                    pbPassword.IsEnabled = true;
                    pbPassword.Focus();
                }
                else
                {
                    pbPassword.IsEnabled = false;
                    pbPassword.Password = "";
                    MessageBox.Show("Произошла ошибка! Сотрудник  с таким номером не найден!");
                }
            }
        }

        private void pbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                GetNewCode();
        }


        private void GetNewCode()
        {
            Employees employee = BaseClass.BD.Employees.FirstOrDefault(x => x.Nomer == tbNomer.Text && x.Password == pbPassword.Password);
            if (employee != null)
            {
                Random rand = new Random();
                Regex regex = new Regex($"^[0-9a-zA-Z`~!@#$%^&*()_\\-+={{}}\\[\\]\\|:;\"'<>,.?\\/]{{8}}$");
                while (true)
                {
                    code = "";
                    for (int i = 0; i < 8; i++)
                    {
                        int j = rand.Next(4); // Выбор 0 - число; 1, 2 - буква; 2 - спецсимвол
                        if (j == 0)
                            code = code + rand.Next(9).ToString();
                        else if (j == 1 || j == 2)
                        {
                            int l = rand.Next(2); // Выбор 0 - заглавная; 1 - маленькая буква
                            if (l == 0)
                                code = code + (char)rand.Next('A', 'Z' + 1);
                            else
                                code = code + (char)rand.Next('a', 'z' + 1);
                        }
                        else
                        {
                            int l = rand.Next(4); // Выбор диапозона
                            if (l == 0)
                                code = code + (char)rand.Next(33, 48);
                            else if (l == 1)
                                code = code + (char)rand.Next(58, 65);
                            else if (l == 2)
                                code = code + (char)rand.Next(91, 97);
                            else if (l == 3)
                                code = code + (char)rand.Next(123, 127);
                        }
                    }
                    if (regex.IsMatch(code)) // КОПИРКА
                    {
                        Clipboard.SetText(code);
                        break;
                    }
                }

                MessageBox.Show("Код для доступа " + code + "\nУ вас будет дано 10 секунд, чтобы ввести код");
                tbCode.IsEnabled = true;
                tbCode.Text = "";
                btnLogin.IsEnabled = true;
                tbCode.Focus();
                countTime = 10;
                disTimer.Start();
            }
            else
            {
                MessageBox.Show("Сотрудник с таким номером и паролем не найден!");
                disTimer.Stop();
                code = "";
                tbRemainingTime.Text = "";
                tbCode.IsEnabled = false;
                tbCode.Text = "";
            }
        }

        private void tbCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login();
        }

        private void Login()
        {
            if (code != "")
            {
                if (tbCode.Text == code)
                {
                    disTimer.Stop();
                    tbRemainingTime.Text = "";
                    code = "";
                    Employees employee = BaseClass.BD.Employees.FirstOrDefault(x => x.Nomer == tbNomer.Text && x.Password == pbPassword.Password);
                    if (employee != null)
                        MessageBox.Show("Вы успешно авторизовались с ролью \"" + employee.Roles.Role + '\"');
                    else
                        MessageBox.Show("Сотрудник с таким номером и паролем не найден!");
                }
                else
                    MessageBox.Show("Код введён не верно!");
            }
            else
                MessageBox.Show("Код утратил свою действительность!");
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetNewCode();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }
    }
}
