using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Entities
{
    public partial class Documento
    {

        public Documento()
        {
            //this.AnulacaoRestoNaoProcessadoItem = new HashSet<AnulacaoRestoNaoProcessadoItem>();
            //this.AnulacaoRestoProcessadoItem = new HashSet<AnulacaoRestoProcessadoItem>();
            //this.AutorizarImpressao = new HashSet<AutorizarImpressao>();
            //this.DocumentoItem = new HashSet<DocumentoItem>();
            this.Documento1 = new HashSet<Documento>();
            this.IdentificadorRestos = "N";
            //this.IndiceTributoMunicipio = new HashSet<IndiceTributoMunicipio>();
            //this.ItemRemessa = new HashSet<ItemRemessa>();
            //this.ItemRetorno = new HashSet<ItemRetorno>();
            this.ItemVariacaoPatrimonialDiminutiva = new HashSet<ItemVariacaoPatrimonialDiminutiva>();
            this.ItemVariacaoPatrimonialDiminutiva1 = new HashSet<ItemVariacaoPatrimonialDiminutiva>();
            //this.LancamentoContabil = new HashSet<LancamentoContabil>();
            //this.LiquidacaoNFE = new HashSet<LiquidacaoNFE>();
            //this.LiquidacaoArquivoAnexo = new HashSet<LiquidacaoArquivoAnexo>();
            //this.Liquidacao = new HashSet<Liquidacao>();
            //this.Liquidacao1 = new HashSet<Liquidacao>();
            //this.MovimentoArrecadacao = new HashSet<MovimentoArrecadacao>();
            //this.NotaInicializacaoItem = new HashSet<NotaInicializacaoItem>();
            //this.NotaLancamento = new HashSet<NotaLancamento>();
            //this.NotaSistemaItem = new HashSet<NotaSistemaItem>();
            //this.OrdemBancariaItem = new HashSet<OrdemBancariaItem>();
            //this.PagamentoEscrituralImposto = new HashSet<PagamentoEscrituralImposto>();
            //this.RetencaoDespesaOriginalID = new HashSet<RetencaoDespesa>();
            //this.RetencaoDespesa = new HashSet<RetencaoDespesa>();
            //this.LiquidacaoNFE = new HashSet<LiquidacaoNFE>();
            //this.LiquidacaoArquivoAnexo = new HashSet<LiquidacaoArquivoAnexo>();
            //this.NotaInicializacaoItem = new HashSet<NotaInicializacaoItem>();
            //this.NotaCotaFinanceiraItem = new HashSet<NotaCotaFinanceiraItem>();
            //this.ReprogramacaoCotaFinanceiraItem = new HashSet<ReprogramacaoCotaFinanceiraItem>();
            //this.NotaProgramacaoFinanceiraItem = new HashSet<NotaProgramacaoFinanceiraItem>();
            //this.ReprogramacaoProgramacaoFinanceiraItem = new HashSet<ReprogramacaoProgramacaoFinanceiraItem>();
            //this.NotaFinanceiraCreditoItem = new HashSet<NotaFinanceiraCreditoItem>();
            //this.AnulacaoRetencaoDespesaItem = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.AnulacaoRetencaoDespesaDocumentoRetencaoDespesa = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.AnulacaoRetencaoDespesaItemDocumentoLiquidacao = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.AnulacaoRetencaoDespesaItemDocumentoEmpenho = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.NotaSistemaItemClassificada = new HashSet<NotaSistemaItemClassificada>();
            //this.DesincorporacaoItens = new HashSet<DesincorporacaoItem>();
            //this.IncorporacaoItens = new HashSet<IncorporacaoItem>();

            //this.DeParaDocumentoEmpenhoDesincorporacao = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();
            //this.DeParaDocumentoAnulacaoRestoNaoProcessado = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();
            //this.DeParaDocumentoEmpenhoIncorporacao = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();
            //this.DeParaDocumentoInscricaoRestoNaoProcessado = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();

            //this.CotaOrcamentaria = new HashSet<CotaOrcamentaria>();
        }

        public Documento(Documento documento)
        {
            //this.AnulacaoRestoNaoProcessadoItem = new HashSet<AnulacaoRestoNaoProcessadoItem>();
            //this.AnulacaoRestoProcessadoItem = new HashSet<AnulacaoRestoProcessadoItem>();
            //this.AutorizarImpressao = documento.AutorizarImpressao == null ? new HashSet<AutorizarImpressao>() : new HashSet<AutorizarImpressao>(documento.AutorizarImpressao);
            //this.DocumentoItem = new HashSet<DocumentoItem>();
            this.Documento1 = documento.Documento1 == null ? new HashSet<Documento>() : new HashSet<Documento>(documento.Documento1);
            //this.ItemRemessa = new HashSet<ItemRemessa>();
            //this.ItemRetorno = new HashSet<ItemRetorno>();
            //this.IndiceTributoMunicipio = new HashSet<IndiceTributoMunicipio>();
            this.IdentificadorRestos = documento.IdentificadorRestos;
            this.ItemVariacaoPatrimonialDiminutiva = new HashSet<ItemVariacaoPatrimonialDiminutiva>();
            this.ItemVariacaoPatrimonialDiminutiva1 = new HashSet<ItemVariacaoPatrimonialDiminutiva>();
            //this.LancamentoContabil = new HashSet<LancamentoContabil>();
            //this.Liquidacao = new HashSet<Liquidacao>();
            //this.Liquidacao1 = new HashSet<Liquidacao>();
            //this.LiquidacaoNFE = new HashSet<LiquidacaoNFE>();
            //this.LiquidacaoArquivoAnexo = new HashSet<LiquidacaoArquivoAnexo>();
            //this.MovimentoArrecadacao = new HashSet<MovimentoArrecadacao>();
            //this.NotaLancamento = new HashSet<NotaLancamento>();
            //this.NotaSistemaItem = new HashSet<NotaSistemaItem>();
            //this.NotaInicializacaoItem = new HashSet<NotaInicializacaoItem>();
            //this.OrdemBancariaItem = new HashSet<OrdemBancariaItem>();
            //this.PagamentoEscrituralImposto = new HashSet<PagamentoEscrituralImposto>();
            //this.RetencaoDespesa = new HashSet<RetencaoDespesa>();
            //this.RetencaoDespesaOriginalID = new HashSet<RetencaoDespesa>();
            //this.AnulacaoRetencaoDespesaItem = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.AnulacaoRetencaoDespesaDocumentoRetencaoDespesa = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.AnulacaoRetencaoDespesaItemDocumentoLiquidacao = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.AnulacaoRetencaoDespesaItemDocumentoEmpenho = new HashSet<AnulacaoRetencaoDespesaItem>();
            //this.NotaCotaFinanceiraItem = new HashSet<NotaCotaFinanceiraItem>();
            //this.ReprogramacaoCotaFinanceiraItem = new HashSet<ReprogramacaoCotaFinanceiraItem>();
            //this.NotaProgramacaoFinanceiraItem = new HashSet<NotaProgramacaoFinanceiraItem>();
            //this.ReprogramacaoProgramacaoFinanceiraItem = new HashSet<ReprogramacaoProgramacaoFinanceiraItem>();
            //this.NotaFinanceiraCreditoItem = new HashSet<NotaFinanceiraCreditoItem>();
            //this.NotaSistemaItemClassificada = new HashSet<NotaSistemaItemClassificada>();
            //this.DesincorporacaoItens = new HashSet<DesincorporacaoItem>();
            //this.IncorporacaoItens = new HashSet<IncorporacaoItem>();
            //this.DeParaDocumentoEmpenhoDesincorporacao = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();
            //this.DeParaDocumentoAnulacaoRestoNaoProcessado = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();
            //this.DeParaDocumentoEmpenhoIncorporacao = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();
            //this.DeParaDocumentoInscricaoRestoNaoProcessado = new HashSet<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao>();

            this.DocumentoOriginalID = documento.DocumentoOriginalID;
            this.DocumentoID = documento.DocumentoID;
            this.CredorIdentificacao = documento.CredorIdentificacao;
            this.FonteRecursoID = documento.FonteRecursoID;
            this.TipoDocumentoContabilID = documento.TipoDocumentoContabilID;
            this.UnidadeGestoraCodigoEmitente = documento.UnidadeGestoraCodigoEmitente;
            this.UnidadeGestoraDataInicioVigenciaEmitente = documento.UnidadeGestoraDataInicioVigenciaEmitente;
            this.UnidadeGestoraCodigoFavorecida = documento.UnidadeGestoraCodigoFavorecida;
            this.UnidadeGestoraDataInicioVigenciaFavorecida = documento.UnidadeGestoraDataInicioVigenciaFavorecida;
            this.DominioIDTipoReferencia = documento.DominioIDTipoReferencia;
            this.DominioIDGrupoDocumentoContabil = documento.DominioIDGrupoDocumentoContabil;
            this.DominioIDSubGrupoDocumentoContabil = documento.DominioIDSubGrupoDocumentoContabil;
            this.EsferaCodigo = documento.EsferaCodigo;
            this.NaturezaDespesaID = documento.NaturezaDespesaID;
            this.LocalizadorID = documento.LocalizadorID;
            this.DataEmissao = documento.DataEmissao;
            this.Numero = documento.Numero;
            this.ValorEstorno = documento.ValorEstorno;
            this.DocumentoReferencia = documento.DocumentoReferencia;
            this.DataDocumentoReferencia = documento.DataDocumentoReferencia;
            this.EventoID = documento.EventoID;
            this.Estorno = documento.Estorno;
            this.Observacao = documento.Observacao;
            this.UsuarioInclusaoRegistro = documento.UsuarioInclusaoRegistro;
            this.DataInclusaoRegistro = documento.DataInclusaoRegistro;
            this.UsuarioAlteracaoRegistro = documento.UsuarioAlteracaoRegistro;
            this.DataAlteracaoRegistro = documento.DataAlteracaoRegistro;
            this.DataContabilizacao = documento.DataContabilizacao;
            this.ValorDocumento = documento.ValorDocumento;
            this.PropostaLOAID = documento.PropostaLOAID;
            this.Situacao = documento.Situacao;
            this.Justificativa = documento.Justificativa;
            this.DominioIDOpcaoLancamento = documento.DominioIDOpcaoLancamento;
            this.IdentificadorUsoCodigo = documento.IdentificadorUsoCodigo;
            this.PedidoAlteracaoID = documento.PedidoAlteracaoID;
            this.IdentificadorRestos = documento.IdentificadorRestos;
            this.Exercicio = documento.Exercicio;
            this.LiquidacaoRestoNaoProcessado = documento.LiquidacaoRestoNaoProcessado;

        }

        public int DocumentoID { get; set; }
        public Nullable<int> DocumentoOriginalID { get; set; }
        public string CredorIdentificacao { get; set; }
        public Nullable<int> FonteRecursoID { get; set; }
        public Nullable<int> TipoDocumentoContabilID { get; set; }
        public int UnidadeGestoraCodigoEmitente { get; set; }
        public System.DateTime UnidadeGestoraDataInicioVigenciaEmitente { get; set; }
        public Nullable<int> UnidadeGestoraCodigoFavorecida { get; set; }
        public Nullable<System.DateTime> UnidadeGestoraDataInicioVigenciaFavorecida { get; set; }
        public Nullable<int> DominioIDTipoReferencia { get; set; }
        public int DominioIDGrupoDocumentoContabil { get; set; }
        public int? DominioIDSubGrupoDocumentoContabil { get; set; }
        public Nullable<int> EsferaCodigo { get; set; }
        public Nullable<int> NaturezaDespesaID { get; set; }
        public Nullable<int> LocalizadorID { get; set; }
        public System.DateTime DataEmissao { get; set; }
        public string Numero { get; set; }
        public decimal ValorEstorno { get; set; }
        public string DocumentoReferencia { get; set; }
        public Nullable<System.DateTime> DataDocumentoReferencia { get; set; }
        public Nullable<int> EventoID { get; set; }
        public string Estorno { get; set; }
        public string Observacao { get; set; }
        public string UsuarioInclusaoRegistro { get; set; }
        public System.DateTime DataInclusaoRegistro { get; set; }
        public string UsuarioAlteracaoRegistro { get; set; }
        public Nullable<System.DateTime> DataAlteracaoRegistro { get; set; }
        public System.DateTime DataContabilizacao { get; set; }
        public decimal ValorDocumento { get; set; }
        public Nullable<int> PropostaLOAID { get; set; }
        public string Situacao { get; set; }
        public string Justificativa { get; set; }
        public Nullable<int> DominioIDOpcaoLancamento { get; set; }
        public Nullable<int> IdentificadorUsoCodigo { get; set; }
        public Nullable<int> PedidoAlteracaoID { get; set; }
        public string IdentificadorRestos { get; set; }
        public int Exercicio { get; set; }
        public int DominioIDPoder { get; set; }
        public bool LiquidacaoRestoNaoProcessado { get; set; }
        public short ValidadoRestos { get; set; }
        //public virtual ICollection<AutorizarImpressao> AutorizarImpressao { get; set; }
        //public virtual Credor Credor { get; set; }
        //public virtual ICollection<DocumentoItem> DocumentoItem { get; set; }
        public virtual ICollection<Documento> Documento1 { get; set; }
        public virtual Documento Documento2 { get; set; }
        //public virtual ICollection<ItemRemessa> ItemRemessa { get; set; }
        //public virtual ICollection<ItemRetorno> ItemRetorno { get; set; }
        //public virtual ICollection<Liquidacao> Liquidacao { get; set; }
        //public virtual ICollection<AnulacaoRestoNaoProcessadoItem> AnulacaoRestoNaoProcessadoItem { get; set; }
        //public virtual ICollection<AnulacaoRestoProcessadoItem> AnulacaoRestoProcessadoItem { get; set; }
        //public virtual ICollection<AnulacaoRestoProcessadoItem> AnulacaoRestoProcessadoItemLiquidacao { get; set; }
        //public virtual NaturezaDespesa NaturezaDespesa { get; set; }
        public virtual ICollection<ItemVariacaoPatrimonialDiminutiva> ItemVariacaoPatrimonialDiminutiva { get; set; }
        //public virtual Empenho Empenho { get; set; }
        //public virtual Evento Evento { get; set; }
        //public virtual GuiaRecebimento GuiaRecebimento { get; set; }
        public virtual ICollection<ItemVariacaoPatrimonialDiminutiva> ItemVariacaoPatrimonialDiminutiva1 { get; set; }
        //public virtual ICollection<LancamentoContabil> LancamentoContabil { get; set; }
        //public virtual ICollection<MovimentoArrecadacao> MovimentoArrecadacao { get; set; }
        //public virtual ICollection<NotaLancamento> NotaLancamento { get; set; }
        //public virtual ICollection<NotaSistemaItem> NotaSistemaItem { get; set; }
        //public virtual OrdemBancaria OrdemBancaria { get; set; }
        //public virtual OrdemBancariaEstornoPagamento OrdemBancariaEstornoPagamento { get; set; }
        //public virtual ICollection<OrdemBancariaItem> OrdemBancariaItem { get; set; }
        //public virtual PagamentoEscritural PagamentoEscritural { get; set; }
        //public virtual PreEmpenho PreEmpenho { get; set; }
        //public virtual ProgramacaoOrcamentariaFinanceira ProgramacaoOrcamentariaFinanceira { get; set; }
        //public virtual ICollection<RetencaoDespesa> RetencaoDespesa { get; set; }
        //public virtual ICollection<RetencaoDespesa> RetencaoDespesaOriginalID { get; set; }
        //public virtual ICollection<Liquidacao> Liquidacao1 { get; set; }
        //public virtual TipoDocumentoContabil TipoDocumentoContabil { get; set; }
        //public virtual UnidadeGestora UnidadeGestora { get; set; }
        //public virtual UnidadeGestora UnidadeGestora1 { get; set; }
        //public virtual ICollection<IndiceTributoMunicipio> IndiceTributoMunicipio { get; set; }
        //public virtual ICollection<PagamentoEscrituralImposto> PagamentoEscrituralImposto { get; set; }
        //public virtual ICollection<LiquidacaoNFE> LiquidacaoNFE { get; set; }
        //public virtual ICollection<LiquidacaoArquivoAnexo> LiquidacaoArquivoAnexo { get; set; }
        //public virtual ICollection<NotaInicializacaoItem> NotaInicializacaoItem { get; set; }
        //public virtual ICollection<InscricaoRestosLiquidacao> InscricaoRestosLiquidacao { get; set; }
        //public virtual ICollection<InscricaoRestosLiquidacaoItem> InscricaoRestosLiquidacaoItem { get; set; }
        //public virtual OrdemBancariaDevolucaoPagamento OrdemBancariaDevolucaoPagamento { get; set; }
        //public virtual ICollection<LancamentoContabilItem> LancamentoContabilItem { get; set; }
        //public virtual InscricaoRestoNaoProcessado InscricaoRestoNaoProcessado { get; set; }
        //public virtual InscricaoRestoProcessado InscricaoRestoProcessado { get; set; }
        //public virtual ICollection<InscricaoRestoProcessado> InscricaoRestosProcessadosDocumentoLiquidacao { get; set; }
        //public virtual ICollection<AnulacaoRetencaoDespesaItem> AnulacaoRetencaoDespesaItem { get; set; }
        //public virtual ICollection<AnulacaoRetencaoDespesaItem> AnulacaoRetencaoDespesaDocumentoRetencaoDespesa { get; set; }
        //public virtual ICollection<AnulacaoRetencaoDespesaItem> AnulacaoRetencaoDespesaItemDocumentoLiquidacao { get; set; }
        //public virtual ICollection<AnulacaoRetencaoDespesaItem> AnulacaoRetencaoDespesaItemDocumentoEmpenho { get; set; }

        //public virtual ICollection<LiquidacaoRetencaoDespesaItem> LiquidacaoRetencaoDespesaItemDocumentoLiquidacao { get; set; }
        //public virtual ICollection<LiquidacaoRetencaoDespesaItem> LiquidacaoRetencaoDespesaItemDocumentoRetencao { get; set; }
        //public virtual ICollection<LiquidacaoRetencaoDespesaItem> LiquidacaoRetencaoDespesaItemDocumentoRetencaoOriginal { get; set; }
        //public virtual ICollection<NotaCotaFinanceiraItem> NotaCotaFinanceiraItem { get; set; }
        //public virtual ICollection<ReprogramacaoCotaFinanceiraItem> ReprogramacaoCotaFinanceiraItem { get; set; }
        //public virtual ICollection<NotaProgramacaoFinanceiraItem> NotaProgramacaoFinanceiraItem { get; set; }
        //public virtual ICollection<ReprogramacaoProgramacaoFinanceiraItem> ReprogramacaoProgramacaoFinanceiraItem { get; set; }
        //public virtual ICollection<NotaFinanceiraCreditoItem> NotaFinanceiraCreditoItem { get; set; }
        //public virtual ICollection<NotaSistemaItemClassificada> NotaSistemaItemClassificada { get; set; }
        //public virtual ICollection<DesincorporacaoItem> DesincorporacaoItens { get; set; }
        //public virtual ICollection<IncorporacaoItem> IncorporacaoItens { get; set; }
        //public virtual ICollection<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao> DeParaDocumentoEmpenhoDesincorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao> DeParaDocumentoAnulacaoRestoNaoProcessado { get; set; }
        //public virtual ICollection<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao> DeParaDocumentoEmpenhoIncorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoEmpenhoDesincorporacaoIncorporacao> DeParaDocumentoInscricaoRestoNaoProcessado { get; set; }

        //public virtual ICollection<DeParaDocumentoLiquidacaoDesincorporacaoIncorporacao> DeParaDocumentoLiquidacaoEmpenhoDesincorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoLiquidacaoDesincorporacaoIncorporacao> DeParaDocumentoLiquidacaoDesincorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoLiquidacaoDesincorporacaoIncorporacao> DeParaDocumentoLiquidacaoAnulacaoRestoProcessado { get; set; }
        //public virtual ICollection<DeParaDocumentoLiquidacaoDesincorporacaoIncorporacao> DeParaDocumentoLiquidacaoEmpenhoIncorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoLiquidacaoDesincorporacaoIncorporacao> DeParaDocumentoLiquidacaoIncorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoLiquidacaoDesincorporacaoIncorporacao> DeParaDocumentoLiquidacaoInscricaoRestoProcessado { get; set; }

        //public virtual ICollection<DeParaDocumentoRetencaoDesincorporacaoIncorporacao> DeParaDocumentoRetencaoEmpenhoDesincorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoRetencaoDesincorporacaoIncorporacao> DeParaDocumentoRetencaoLiquidacaoDesincorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoRetencaoDesincorporacaoIncorporacao> DeParaDocumentoRetencaoDesincorporacao { get; set; }
        //public virtual ICollection<DeParaDocumentoRetencaoDesincorporacaoIncorporacao> DeParaDocumentoRetencaoAnulacaoRetencao { get; set; }
        //public virtual ICollection<DeParaDocumentoRetencaoDesincorporacaoIncorporacao> DeParaDocumentoRetencaoIncorporacao { get; set; }

        //public virtual ICollection<CotaOrcamentaria> CotaOrcamentaria { get; set; }
    }
},D
