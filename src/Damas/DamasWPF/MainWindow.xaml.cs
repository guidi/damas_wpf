using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            SolidColorBrush quadradoEscuro = new SolidColorBrush(Color.FromRgb(139, 69, 19));
            SolidColorBrush quadradoClaro = new SolidColorBrush(Color.FromRgb(245, 222, 179));
            SolidColorBrush borda = new SolidColorBrush(Color.FromRgb(59, 42, 24)); 

            for (int row = 1; row <= linhas; row++)
            {
                for (int col = 1; col <= colunas; col++)
                {
                    Border border = new Border
                    {
                        Background = (row + col) % 2 == 0 ? quadradoClaro : quadradoEscuro,
                        BorderThickness = new Thickness(1),
                        BorderBrush = borda
                    };

                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);
                    Tabuleiro.Children.Add(border);

                    if ((row + col) % 2 != 0 && (row < 4 || row > 5)) // Regrinha pra adicionar as peças nos lugares certos.
                    {
                        string nomeImagemPeca = ObterNomeImagemPeca(row);
                        if (!string.IsNullOrEmpty(nomeImagemPeca))
                        {
                            Image img = new Image
                            {
                                Source = new BitmapImage(new Uri($"pack://application:,,,/assets/{nomeImagemPeca}.png", UriKind.Absolute)),
                                Width = 60,
                                Height = 60
                            };

                            border.Child = img;
                        }
                    }
                }
            }
        }

        private string ObterNomeImagemPeca(int row)
        {
            return (row < 4) ? "branca" : "vermelha";
        }
    }
}