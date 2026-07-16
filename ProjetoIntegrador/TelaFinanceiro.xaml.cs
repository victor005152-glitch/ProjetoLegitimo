using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace ProjetoIntegrador
{
    public partial class TelaFinanceiro : Page
    {
        private ObservableCollection<GastoFinanceiro> gastos =
            new ObservableCollection<GastoFinanceiro>();

        private string caminhoDocumentoSelecionado = "";
        private string nomeDocumentoSelecionado = "";

        public TelaFinanceiro()
        {
            InitializeComponent();

            DGastos.ItemsSource = gastos;

            ResetarBotoes();

            BotaoFinanceiro.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));

            ComboMes.SelectedIndex = DateTime.Now.Month - 1;
            TxtAno.Text = DateTime.Now.Year.ToString();
            DpDataGasto.SelectedDate = DateTime.Today;

            LabelDocumento.Content = "Nenhum documento anexado";

            CarregarFinanceiro();
        }

        public class GastoFinanceiro
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
            public string Categoria { get; set; }
            public decimal Valor { get; set; }
            public DateTime DataGasto { get; set; }
            public string Observacao { get; set; }
            public string DocumentoNome { get; set; }
            public string DocumentoPath { get; set; }

            public string DataFormatada
            {
                get { return DataGasto.ToString("dd/MM/yyyy"); }
            }

            public string ValorFormatado
            {
                get { return "R$ " + Valor.ToString("N2"); }
            }
        }

        private void CarregarFinanceiro()
        {
            CarregarCardsFinanceiros();
            CarregarGastos();
        }

        private int ObterMes()
        {
            if (ComboMes.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                return Convert.ToInt32(item.Tag.ToString());
            }

            return DateTime.Now.Month;
        }

        private int ObterAno()
        {
            if (int.TryParse(TxtAno.Text, out int ano))
            {
                return ano;
            }

            return DateTime.Now.Year;
        }

        private void CarregarCardsFinanceiros()
        {
            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                int mes = ObterMes();
                int ano = ObterAno();

                DateTime inicio = new DateTime(ano, mes, 1);
                DateTime fim = inicio.AddMonths(1);

                string sql = @"
    SELECT
        COALESCE((
            SELECT SUM(v.total)
            FROM vendas v
            WHERE v.data_venda >= @inicio
              AND v.data_venda < @fim
        ), 0) AS total_vendas,

        COALESCE((
            SELECT SUM(vi.lucro)
            FROM venda_itens vi
            INNER JOIN vendas v2 ON v2.id = vi.venda_id
            WHERE v2.data_venda >= @inicio
              AND v2.data_venda < @fim
        ), 0) AS lucro_bruto,

        COALESCE((
            SELECT SUM(g.valor)
            FROM gastos g
            WHERE g.data_gasto >= @inicio
              AND g.data_gasto < @fim
        ), 0) AS gastos_manuais,

        COALESCE((
            SELECT SUM(vi.custo_unitario * vi.quantidade)
            FROM venda_itens vi
            INNER JOIN vendas v3 ON v3.id = vi.venda_id
            WHERE v3.data_venda >= @inicio
              AND v3.data_venda < @fim
        ), 0) AS custo_produtos_vendidos,

        COALESCE((
            SELECT SUM(p.valor_custo * p.quantidade_estoque)
            FROM produtos p
        ), 0) AS custo_produtos_estoque,

        COALESCE((
            SELECT mf.meta_vendas
            FROM metas_financeiras mf
            WHERE mf.mes = @mes
              AND mf.ano = @ano
            ORDER BY mf.id DESC
            LIMIT 1
        ), 0) AS meta_vendas,

        COALESCE((
            SELECT mf.meta_lucro
            FROM metas_financeiras mf
            WHERE mf.mes = @mes
              AND mf.ano = @ano
            ORDER BY mf.id DESC
            LIMIT 1
        ), 0) AS meta_lucro,

        COALESCE((
            SELECT mf.observacao
            FROM metas_financeiras mf
            WHERE mf.mes = @mes
              AND mf.ano = @ano
            ORDER BY mf.id DESC
            LIMIT 1
        ), '') AS observacao_meta";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@inicio", inicio);
                    comando.Parameters.AddWithValue("@fim", fim);
                    comando.Parameters.AddWithValue("@mes", mes);
                    comando.Parameters.AddWithValue("@ano", ano);

                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        if (leitor.Read())
                        {

                            decimal totalVendas = Convert.ToDecimal(leitor["total_vendas"]);
                            decimal lucroBruto = Convert.ToDecimal(leitor["lucro_bruto"]);
                            decimal gastosManuais = Convert.ToDecimal(leitor["gastos_manuais"]);
                            decimal custoProdutosVendidos = Convert.ToDecimal(leitor["custo_produtos_vendidos"]);
                            decimal custoProdutosEstoque = Convert.ToDecimal(leitor["custo_produtos_estoque"]);
                            decimal metaVendas = Convert.ToDecimal(leitor["meta_vendas"]);
                            decimal metaLucro = Convert.ToDecimal(leitor["meta_lucro"]);
                            string observacaoMeta = leitor["observacao_meta"].ToString();

                            decimal gastosTotais = gastosManuais + custoProdutosVendidos + custoProdutosEstoque;
                            decimal lucroLiquido = totalVendas - gastosTotais;

                            CardVendas.Content = "R$ " + totalVendas.ToString("N2");
                            CardLucroBruto.Content = "R$ " + lucroBruto.ToString("N2");
                            CardGastos.Content = "R$ " + gastosTotais.ToString("N2");
                            CardLucroLiquido.Content = "R$ " + lucroLiquido.ToString("N2");

                            AtualizarMetas(totalVendas, lucroLiquido, metaVendas, metaLucro);

                            TxtMetaVendas.Text = metaVendas > 0 ? metaVendas.ToString("N2") : "";
                            TxtMetaLucro.Text = metaLucro > 0 ? metaLucro.ToString("N2") : "";
                            TxtObservacaoMeta.Text = observacaoMeta;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar financeiro: " + ex.Message);
            }
        }

        private void AtualizarMetas(decimal totalVendas, decimal lucroLiquido, decimal metaVendas, decimal metaLucro)
        {
            double percentualVendas = 0;
            double percentualLucro = 0;

            if (metaVendas > 0)
            {
                percentualVendas = Convert.ToDouble((totalVendas / metaVendas) * 100);
            }

            if (metaLucro > 0)
            {
                percentualLucro = Convert.ToDouble((lucroLiquido / metaLucro) * 100);
            }

            if (percentualVendas < 0)
            {
                percentualVendas = 0;
            }

            if (percentualLucro < 0)
            {
                percentualLucro = 0;
            }

            BarraMetaVendas.Value = Math.Min(percentualVendas, 100);
            BarraMetaLucro.Value = Math.Min(percentualLucro, 100);

            LabelMetaVendas.Content = percentualVendas.ToString("N1") + "%";
            LabelMetaLucro.Content = percentualLucro.ToString("N1") + "%";
        }

        private void CarregarGastos()
        {
            gastos.Clear();

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                int mes = ObterMes();
                int ano = ObterAno();

                DateTime inicio = new DateTime(ano, mes, 1);
                DateTime fim = inicio.AddMonths(1);

                string sql = @"
                    SELECT
                        id,
                        descricao,
                        COALESCE(categoria, '') AS categoria,
                        valor,
                        data_gasto,
                        COALESCE(observacao, '') AS observacao,
                        COALESCE(documento_nome, '') AS documento_nome,
                        COALESCE(documento_path, '') AS documento_path
                    FROM gastos
                    WHERE data_gasto >= @inicio
                      AND data_gasto < @fim
                    ORDER BY data_gasto DESC, id DESC";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@inicio", inicio);
                    comando.Parameters.AddWithValue("@fim", fim);

                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        while (leitor.Read())
                        {
                            GastoFinanceiro gasto = new GastoFinanceiro
                            {
                                Id = Convert.ToInt32(leitor["id"]),
                                Descricao = leitor["descricao"].ToString(),
                                Categoria = leitor["categoria"].ToString(),
                                Valor = Convert.ToDecimal(leitor["valor"]),
                                DataGasto = Convert.ToDateTime(leitor["data_gasto"]),
                                Observacao = leitor["observacao"].ToString(),
                                DocumentoNome = leitor["documento_nome"].ToString(),
                                DocumentoPath = leitor["documento_path"].ToString()
                            };

                            gastos.Add(gasto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar gastos: " + ex.Message);
            }
        }

        private bool ConverterDecimal(string texto, out decimal valor)
        {
            texto = texto.Replace("R$", "").Trim();

            if (decimal.TryParse(texto, NumberStyles.Number, new CultureInfo("pt-BR"), out valor))
            {
                return true;
            }

            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
            {
                return true;
            }

            valor = 0;
            return false;
        }

        private void BotaoAnexarDocumento_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Selecionar comprovante do gasto";
            dialog.Filter = "Documentos e imagens|*.pdf;*.jpg;*.jpeg;*.png;*.doc;*.docx;*.xls;*.xlsx|Todos os arquivos|*.*";

            if (dialog.ShowDialog() == true)
            {
                caminhoDocumentoSelecionado = dialog.FileName;
                nomeDocumentoSelecionado = Path.GetFileName(dialog.FileName);

                LabelDocumento.Content = nomeDocumentoSelecionado;
            }
        }

        private void BotaoAdicionarGasto_Click(object sender, RoutedEventArgs e)
        {
            string descricao = TxtDescricaoGasto.Text.Trim();
            string categoria = TxtCategoriaGasto.Text.Trim();
            string observacao = TxtObservacaoGasto.Text.Trim();

            if (string.IsNullOrWhiteSpace(descricao))
            {
                MessageBox.Show("Informe a descrição do gasto.");
                TxtDescricaoGasto.Focus();
                return;
            }

            if (!ConverterDecimal(TxtValorGasto.Text, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Informe um valor válido para o gasto.");
                TxtValorGasto.Focus();
                return;
            }

            if (DpDataGasto.SelectedDate == null)
            {
                MessageBox.Show("Informe a data do gasto.");
                DpDataGasto.Focus();
                return;
            }

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                string pastaDocumentos = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "WRVControl",
                    "DocumentosAnexados"
                );

                if (!Directory.Exists(pastaDocumentos))
                {
                    Directory.CreateDirectory(pastaDocumentos);
                }

                string caminhoDocumentoFinal = "";
                string nomeDocumentoFinal = "";

                if (!string.IsNullOrWhiteSpace(caminhoDocumentoSelecionado))
                {
                    string nomeSeguro = Path.GetFileName(nomeDocumentoSelecionado);
                    nomeDocumentoFinal = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + nomeSeguro;
                    caminhoDocumentoFinal = Path.Combine(pastaDocumentos, nomeDocumentoFinal);

                    File.Copy(caminhoDocumentoSelecionado, caminhoDocumentoFinal, true);
                }

                string sql = @"
                    INSERT INTO gastos
                    (
                        descricao,
                        categoria,
                        valor,
                        data_gasto,
                        observacao,
                        documento_nome,
                        documento_path
                    )
                    VALUES
                    (
                        @descricao,
                        @categoria,
                        @valor,
                        @data_gasto,
                        @observacao,
                        @documento_nome,
                        @documento_path
                    )";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@descricao", descricao);
                    comando.Parameters.AddWithValue("@categoria", categoria);
                    comando.Parameters.AddWithValue("@valor", valor);
                    comando.Parameters.AddWithValue("@data_gasto", DpDataGasto.SelectedDate.Value);
                    comando.Parameters.AddWithValue("@observacao", observacao);
                    comando.Parameters.AddWithValue("@documento_nome", nomeDocumentoFinal);
                    comando.Parameters.AddWithValue("@documento_path", caminhoDocumentoFinal);

                    comando.ExecuteNonQuery();
                }

                MessageBox.Show("Gasto cadastrado com sucesso!");

                TxtDescricaoGasto.Clear();
                TxtCategoriaGasto.Clear();
                TxtValorGasto.Clear();
                TxtObservacaoGasto.Clear();
                DpDataGasto.SelectedDate = DateTime.Today;

                caminhoDocumentoSelecionado = "";
                nomeDocumentoSelecionado = "";
                LabelDocumento.Content = "Nenhum documento anexado";

                CarregarFinanceiro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar gasto: " + ex.Message);
            }
        }

        private void BotaoAbrirDocumento_Click(object sender, RoutedEventArgs e)
        {
            if (DGastos.SelectedItem is not GastoFinanceiro gasto)
            {
                MessageBox.Show("Selecione um gasto para abrir o documento.");
                return;
            }

            if (string.IsNullOrWhiteSpace(gasto.DocumentoPath))
            {
                MessageBox.Show("Esse gasto não possui documento anexado.");
                return;
            }

            if (!File.Exists(gasto.DocumentoPath))
            {
                MessageBox.Show("O arquivo não foi encontrado no computador.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = gasto.DocumentoPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir documento: " + ex.Message);
            }
        }

        private void BotaoExcluirGasto_Click(object sender, RoutedEventArgs e)
        {
            if (DGastos.SelectedItem is not GastoFinanceiro gasto)
            {
                MessageBox.Show("Selecione um gasto para excluir.");
                return;
            }

            MessageBoxResult resposta = MessageBox.Show(
                "Deseja realmente excluir este gasto?",
                "Confirmar exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resposta != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                string sql = "DELETE FROM gastos WHERE id = @id";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@id", gasto.Id);
                    comando.ExecuteNonQuery();
                }

                MessageBox.Show("Gasto excluído com sucesso!");

                CarregarFinanceiro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir gasto: " + ex.Message);
            }
        }

        private void BotaoSalvarMeta_Click(object sender, RoutedEventArgs e)
        {
            if (!ConverterDecimal(TxtMetaVendas.Text, out decimal metaVendas))
            {
                metaVendas = 0;
            }

            if (!ConverterDecimal(TxtMetaLucro.Text, out decimal metaLucro))
            {
                metaLucro = 0;
            }

            string observacao = TxtObservacaoMeta.Text.Trim();

            int mes = ObterMes();
            int ano = ObterAno();

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                int metaId = 0;

                string sqlBuscar = @"
                    SELECT id
                    FROM metas_financeiras
                    WHERE mes = @mes
                      AND ano = @ano
                    ORDER BY id DESC
                    LIMIT 1";

                using (MySqlCommand comandoBuscar = new MySqlCommand(sqlBuscar, ConectBd.Conexao))
                {
                    comandoBuscar.Parameters.AddWithValue("@mes", mes);
                    comandoBuscar.Parameters.AddWithValue("@ano", ano);

                    object resultado = comandoBuscar.ExecuteScalar();

                    if (resultado != null)
                    {
                        metaId = Convert.ToInt32(resultado);
                    }
                }

                if (metaId > 0)
                {
                    string sqlAtualizar = @"
                        UPDATE metas_financeiras
                        SET meta_vendas = @meta_vendas,
                            meta_lucro = @meta_lucro,
                            observacao = @observacao
                        WHERE id = @id";

                    using (MySqlCommand comando = new MySqlCommand(sqlAtualizar, ConectBd.Conexao))
                    {
                        comando.Parameters.AddWithValue("@id", metaId);
                        comando.Parameters.AddWithValue("@meta_vendas", metaVendas);
                        comando.Parameters.AddWithValue("@meta_lucro", metaLucro);
                        comando.Parameters.AddWithValue("@observacao", observacao);

                        comando.ExecuteNonQuery();
                    }
                }
                else
                {
                    string sqlInserir = @"
                        INSERT INTO metas_financeiras
                        (mes, ano, meta_vendas, meta_lucro, observacao)
                        VALUES
                        (@mes, @ano, @meta_vendas, @meta_lucro, @observacao)";

                    using (MySqlCommand comando = new MySqlCommand(sqlInserir, ConectBd.Conexao))
                    {
                        comando.Parameters.AddWithValue("@mes", mes);
                        comando.Parameters.AddWithValue("@ano", ano);
                        comando.Parameters.AddWithValue("@meta_vendas", metaVendas);
                        comando.Parameters.AddWithValue("@meta_lucro", metaLucro);
                        comando.Parameters.AddWithValue("@observacao", observacao);

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Meta salva com sucesso!");

                CarregarFinanceiro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar meta: " + ex.Message);
            }
        }

        private void ComboMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtAno == null || DGastos == null)
            {
                return;
            }

            CarregarFinanceiro();
        }

        private void BotaoAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarFinanceiro();
        }

        private void ResetarBotoes()
        {
            SolidColorBrush corPadrao =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

            BotaoEstoque.Background = corPadrao;
            BotaoVendas.Background = corPadrao;
            BotaoHistorico.Background = corPadrao;
            BotaoFinanceiro.Background = corPadrao;
            BotaoEmpresa.Background = corPadrao;
            BotaoClientes.Background = corPadrao;
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void BotaoVendas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
        }

        private void BotaoHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaHistorico());
        }

        private void BotaoEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }
        private void BotaoClientes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaClientes());
        }
    }
}