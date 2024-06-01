using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DamasWPF
{
    public class PecaTag
    {
        public string Estado { get; set; }
        public string Cor { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image pecaSelecionada = null;

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
                            string cor = nomeImagemPeca;

                            Image img = new Image
                            {
                                Source = new BitmapImage(new Uri($"pack://application:,,,/assets/{nomeImagemPeca}.png", UriKind.Absolute)),
                                Width = 60,
                                Height = 60,
                                Tag = new PecaTag { Estado = Constantes.NaoClicado, Cor = cor }
                            };

                            img.MouseDown += img_MouseDown;

                            border.Child = img;
                        }
                    }

                    border.MouseDown += border_MouseDown;
                }
            }
        }

        private void border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null && border.Child == null && pecaSelecionada != null)
            {
                int linhaOrigem = Grid.GetRow(pecaSelecionada.Parent as UIElement);
                int colunaOrigem = Grid.GetColumn(pecaSelecionada.Parent as UIElement);
                int linhaDestino = Grid.GetRow(border);
                int colunaDestino = Grid.GetColumn(border);

                var tag = (PecaTag)pecaSelecionada.Tag;
                bool movimentoValido = false;
                
                if (tag.Cor == Constantes.Branca)
                {
                    movimentoValido = linhaDestino == linhaOrigem + 1 && (colunaDestino == colunaOrigem + 1 || colunaDestino == colunaOrigem - 1);
                }
                else if (tag.Cor == Constantes.Vermelha)
                {
                    movimentoValido = linhaDestino == linhaOrigem - 1 && (colunaDestino == colunaOrigem + 1 || colunaDestino == colunaOrigem - 1);
                }

                if (movimentoValido)
                {
                    //Move
                    (pecaSelecionada.Parent as Border).Child = null;
                    border.Child = pecaSelecionada;

                    //Volta a peça pra cor de não selecionada
                    pecaSelecionada.Opacity = Constantes.OpacidadePecaNaoSelecionada;
                    tag.Estado = Constantes.NaoClicado;
                    pecaSelecionada = null;
                }
            }
        }

        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                var tag = (PecaTag)img.Tag;
                if (pecaSelecionada != null)
                {
                    var tagSelecionada = (PecaTag)pecaSelecionada.Tag;
                    pecaSelecionada.Opacity = Constantes.OpacidadePecaNaoSelecionada;
                    tagSelecionada.Estado = Constantes.NaoClicado;
                }

                pecaSelecionada = img;
                tag.Estado = tag.Estado == Constantes.NaoClicado ? Constantes.Clicado : Constantes.NaoClicado;
                img.Opacity = tag.Estado == Constantes.Clicado ? Constantes.OpacidadePecaSelecionada : Constantes.OpacidadePecaNaoSelecionada;
            }
        }

        private string ObterNomeImagemPeca(int row)
        {
            return (row < 4) ? Constantes.Branca : Constantes.Vermelha;
        }
    }
}