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
        public int Linha { get; set; }
        public int Coluna { get; set; }
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

        private void ResetarJogo_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja realmente resetar o jogo?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                pecaSelecionada = null;
                Tabuleiro.Children.Clear();
                DesenharTabuleiro();
            }
        }

        private void DesenharTabuleiro()
        {
            int quantidadeLinhas = 8;
            int quantidadeColunas = 8;
            SolidColorBrush quadradoEscuro = new SolidColorBrush(Color.FromRgb(139, 69, 19));
            SolidColorBrush quadradoClaro = new SolidColorBrush(Color.FromRgb(245, 222, 179));
            SolidColorBrush borda = new SolidColorBrush(Color.FromRgb(59, 42, 24)); 

            for (int linha = 1; linha <= quantidadeLinhas; linha++)
            {
                for (int coluna = 1; coluna <= quantidadeColunas; coluna++)
                {
                    Border border = new Border
                    {
                        //Para criar o padrão alternado no tabuleiro
                        Background = (linha + coluna) % 2 == 0 ? quadradoClaro : quadradoEscuro,
                        BorderThickness = new Thickness(1),
                        BorderBrush = borda
                    };

                    Grid.SetRow(border, linha);
                    Grid.SetColumn(border, coluna);
                    Tabuleiro.Children.Add(border);

                    //Regrinha pra adicionar as peças nos lugares certos seguindo o padrão do tabuleiro de damas
                    //As peças devem ser colocadas apenas nos quadrados impares.
                    //Nas linhas 1 a 3 e 5 a 8
                    if ((linha + coluna) % 2 != 0 && (linha < 4 || linha > 5))
                    {
                        string nomeImagemPeca = ObterNomeImagemPeca(linha);
                        if (!string.IsNullOrEmpty(nomeImagemPeca))
                        {
                            string cor = nomeImagemPeca;

                            Image img = new Image
                            {
                                Source = new BitmapImage(new Uri($"pack://application:,,,/assets/{nomeImagemPeca}.png", UriKind.Absolute)),
                                Width = 60,
                                Height = 60,
                                Tag = new PecaTag { Estado = Constantes.NaoClicado, Cor = cor, Linha = linha, Coluna = coluna }
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
                int linhaDestino = Grid.GetRow(border);
                int colunaDestino = Grid.GetColumn(border);

                var tagPecaSelecionada = (PecaTag)pecaSelecionada.Tag;

                //Verifica se o movimento é válido, nesse caso se mover pra diagonal no tabuleiro, uma posição
                if (PodeMover(tagPecaSelecionada, linhaDestino, colunaDestino))
                {
                    MoverPeca(tagPecaSelecionada, linhaDestino, colunaDestino, border);
                }
                // Verifica se a peça está "comendo" outra peça
                else if (PodeComer(tagPecaSelecionada, linhaDestino, colunaDestino))
                {
                    ComerPeca(tagPecaSelecionada, linhaDestino, colunaDestino, border);
                }
            }
        }

        private Border ObterBorderPorPosicao(int linha, int coluna)
        {
            foreach (UIElement element in Tabuleiro.Children)
            {
                if (Grid.GetRow(element) == linha && Grid.GetColumn(element) == coluna)
                {
                    return element as Border;
                }
            }
            return null;
        }

        //Atualiza o estado da peça selecionada
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

        private bool PodeMover(PecaTag tagPecaOrigem, int linhaDestino, int colunaDestino)
        {
            //As peças brancas estão em cima e só podem se mover para frente (descendo) e apenas uma casa, então linhaDestino deve ser linhaOrigem + 1.
            //Como o movimento é diagonal então a coluna destino vai ser a coluna origem+1 ou origem-1
            if (tagPecaOrigem.Cor == Constantes.Branca)
            {
                return linhaDestino == tagPecaOrigem.Linha + 1 && (colunaDestino == tagPecaOrigem.Coluna + 1 || colunaDestino == tagPecaOrigem.Coluna - 1);
            }
            //As peças vermelhas estão em baixo e só podem se mover para frente (subindo) e apenas uma casa, então linhaDestino deve ser linhaOrigem - 1.
            //Como o movimento é diagonal então a coluna destino vai ser a coluna origem+1 ou origem-1
            else if (tagPecaOrigem.Cor == Constantes.Vermelha)
            {
                return linhaDestino == tagPecaOrigem.Linha - 1 && (colunaDestino == tagPecaOrigem.Coluna + 1 || colunaDestino == tagPecaOrigem.Coluna - 1);
            }
            return false;
        }

        private bool PodeComer(PecaTag tagPecaOrigem, int linhaDestino, int colunaDestino)
        {
            if (Math.Abs(linhaDestino - tagPecaOrigem.Linha) == 2 && Math.Abs(colunaDestino - tagPecaOrigem.Coluna) == 2)
            {
                int linhaMeio = (tagPecaOrigem.Linha + linhaDestino) / 2;
                int colunaMeio = (tagPecaOrigem.Coluna + colunaDestino) / 2;
                Border borderMeio = ObterBorderPorPosicao(linhaMeio, colunaMeio);
                //Pega a peça que vai ser comida, ou seja, a que está entre a linha e coluna origem e o linha e coluna destino
                if (borderMeio != null && borderMeio.Child is Image pecaAdversaria)
                {
                    var tagPecaAdversaria = (PecaTag)pecaAdversaria.Tag;
                    if (tagPecaAdversaria.Cor != tagPecaOrigem.Cor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void MoverPeca(PecaTag tagPecaOrigem, int linhaDestino, int colunaDestino, Border borderDestino)
        {
            //Move a peça pro novo quadrado
            (pecaSelecionada.Parent as Border).Child = null;
            borderDestino.Child = pecaSelecionada;

            //Atualiza a posição da peça
            tagPecaOrigem.Linha = linhaDestino;
            tagPecaOrigem.Coluna = colunaDestino;

            //Reseta a seleção
            pecaSelecionada.Opacity = Constantes.OpacidadePecaNaoSelecionada;
            tagPecaOrigem.Estado = Constantes.NaoClicado;
            pecaSelecionada = null;
        }

        private void ComerPeca(PecaTag tagPecaOrigem, int linhaDestino, int colunaDestino, Border borderDestino)
        {
            int linhaMeio = (tagPecaOrigem.Linha + linhaDestino) / 2;
            int colunaMeio = (tagPecaOrigem.Coluna + colunaDestino) / 2;
            Border borderMeio = ObterBorderPorPosicao(linhaMeio, colunaMeio);

            //Remove a peça adversária
            if (borderMeio != null)
            {
                borderMeio.Child = null;
            }

            //Move a peça para o novo quadrado
            MoverPeca(tagPecaOrigem, linhaDestino, colunaDestino, borderDestino);
        }

        private string ObterNomeImagemPeca(int row)
        {
            return (row < 4) ? Constantes.Branca : Constantes.Vermelha;
        }
    }
}