using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DamasWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DesenharTabuleiro();
        }

        private void DesenharTabuleiro()
        {
            int linhas = 8;
            int colunas = 8;
            SolidColorBrush quadradoEscuro = new SolidColorBrush(Color.FromRgb(139, 69, 19));  //marrom
            SolidColorBrush quadradoClaro = new SolidColorBrush(Color.FromRgb(245, 222, 179)); //bege

            for (int row = 1; row <= linhas; row++)
            {
                for (int col = 1; col <= colunas; col++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = (row + col) % 2 == 0 ? quadradoClaro : quadradoEscuro; //Para criar o padrão alternado nas cores
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    Tabuleiro.Children.Add(rect);
                }
            }
        }
    }
}