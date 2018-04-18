using System;
using System.Collections.Generic;
using System.Linq;


namespace CFPMVC.Areas.Execucao.Controllers
{
    public class VPDBaseController : DocumentoController
    {
        private NaturezaReceitaDataApplicator _naturezaReceitaDA;

        protected NaturezaReceitaDataApplicator naturezaReceitaDA
        {
            get
            {
                return _naturezaReceitaDA ?? (_naturezaReceitaDA = new NaturezaReceitaDataApplicator(contextoEXEFIN));
            }
        }


        protected void ExcluirItemVPD(int vpdItem, string naturezaDespesaID)
        {
            if (vpdItem > 0 && string.IsNullOrWhiteSpace(naturezaDespesaID))
                throw new CFP.Util.SPFException("Informe o registro a ser removido");

            var vpdVM = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM;

            if (vpdVM.ListaItemVPD == null || !vpdVM.ListaItemVPD.Any())
                throw new CFP.Util.SPFException("VPD Prévia não possui nenhum item a ser removido.");

            ListaItemVPD itemVPD = null;
            if (string.IsNullOrWhiteSpace(naturezaDespesaID))
                itemVPD = vpdVM.ListaItemVPD.FirstOrDefault(el => el.VariacaoPatrimonialDiminutivaItemID == vpdItem);
            else
            {
                int naturezaID = default(int);

                if (int.TryParse(naturezaDespesaID, out naturezaID))
                {
                    itemVPD = vpdVM.ListaItemVPD.FirstOrDefault(el => el.VariacaoPatrimonialDiminutivaItemID == vpdItem && el.NaturezaDespesaID == naturezaID);
                }
            }

            if (itemVPD == null)
                throw new CFP.Util.SPFException("Não foi localizado o item.");

            vpdVM.ListaItemVPD.Remove(itemVPD);
        }

        protected void AdicionarItemVPD(ItemVPD itemVPD)
        {
            var vpdVM = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM;
            if (vpdVM.ListaItemVPD != null)
            {
                var itemVPDAntigo = vpdVM.ListaItemVPD.FirstOrDefault(el => el.NaturezaDespesaID == itemVPD.NaturezaDespesaID &&
                                                                            el.FonteRecursoID == itemVPD.FonteRecursoID &&
                                                                            el.EmpenhoID == itemVPD.EmpenhoID);
                if (itemVPDAntigo != null)
                    AtualizaItemVPD(itemVPDAntigo, itemVPD);
                else
                    AdicionaItemVPD(itemVPD);
            }
            else
                AdicionaItemVPD(itemVPD);
        }

        private void AdicionaItemVPD(ItemVPD itemVPD)
        {
            var vpdVM = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM;

            if (vpdVM.ListaItemVPD == null)
                vpdVM.ListaItemVPD = new List<ListaItemVPD>();

            var listaVPD = new ListaItemVPD();
            listaVPD.VariacaoPatrimonialDiminutivaItemID = vpdVM.ListaItemVPD.Any() ? vpdVM.ListaItemVPD.OrderByDescending(el => el.VariacaoPatrimonialDiminutivaItemID).FirstOrDefault().VariacaoPatrimonialDiminutivaItemID + 1 : 1;
            listaVPD.NaturezaDespesaCodigoDescricao = itemVPD.NaturezaDespesaID.HasValue ? naturezaDespesaDA.RetornaCodigoDescricaoNaturezaDespesa(itemVPD.NaturezaDespesaID.Value) : string.Empty;
            listaVPD.NaturezaDespesaID = itemVPD.NaturezaDespesaID;
            listaVPD.FonteRecursoID = itemVPD.FonteRecursoID;
            listaVPD.FonteRecursoDescricao = itemVPD.FonteRecursoID.HasValue ? itemVPD.IdentificadorUsoCodigo.ToString() + fonteRecursoDA.RetornaNumeroDescricaoFonteRecurso(itemVPD.FonteRecursoID.Value) : string.Empty;
            listaVPD.ValorSolicitado = itemVPD.ValorSolicitado;
            listaVPD.IdentificadorUsoCodigo = itemVPD.IdentificadorUsoCodigo;
            listaVPD.EmpenhoID = itemVPD.EmpenhoID;
            listaVPD.NumeroEmpenho = itemVPD.EmpenhoID.HasValue ? documentoDA.ObterUnico(x => x.DocumentoID == itemVPD.EmpenhoID.Value).Numero : string.Empty;
            listaVPD.ValorTotal = vpdVM.ListaItemVPD.Sum(el => el.ValorSolicitado) + itemVPD.ValorSolicitado;
            vpdVM.ListaItemVPD.Add(listaVPD);
        }

        private void AtualizaItemVPD(ListaItemVPD itemVPDAntigo, ItemVPD itemVPD)
        {
            var vpdVM = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM;

            itemVPDAntigo.ValorSolicitado += itemVPD.ValorSolicitado;
            itemVPDAntigo.ValorTotal = vpdVM.ListaItemVPD.Sum(el => el.ValorSolicitado);
        }

        protected List<string> ValidarContaNatureza(int naturezaDespesaId)
        {
            var naturezaDespesa = naturezaDespesaDA.ConsultarTodos().FirstOrDefault(el => el.NaturezaDespesaID == naturezaDespesaId);
            List<string> retorno = new List<string>();
            if (naturezaDespesa != null)
            {
                if (string.IsNullOrWhiteSpace(naturezaDespesa.ContaCredora))
                    throw new CFP.Util.SPFException("Não será possível gerar VPD. A classificação " + naturezaDespesa.NaturezaDespesaCodigo + " não possui ContaCredora cadastrada.");

                if (string.IsNullOrWhiteSpace(naturezaDespesa.ContaDevedora))
                    throw new CFP.Util.SPFException("Não será possível gerar VPD. A classificação " + naturezaDespesa.NaturezaDespesaCodigo + " não possui ContaDevedora cadastrada.");

                int contaContabilDevedoraCodigo;
                ContaContabil contaContabilDebito = null;
                if (!int.TryParse(naturezaDespesa.ContaDevedora, out contaContabilDevedoraCodigo))
                {
                    contaContabilDevedoraCodigo = RetirarMascaraNatureza(naturezaDespesa.NaturezaDespesaID, "D", true);
                    contaContabilDebito = contaContabilDA.ConsultarTodos().FirstOrDefault(el => el.ContaContabilCodigo == contaContabilDevedoraCodigo);
                }
                else
                {
                    contaContabilDebito = contaContabilDA.ConsultarTodos().FirstOrDefault(el => el.ContaContabilCodigo == contaContabilDevedoraCodigo);
                    if (contaContabilDebito == null)
                        throw new CFP.Util.SPFException("Não será possível gerar VPD. A conta devedora " + naturezaDespesa.ContaDevedora + " não esta cadastrada na tabela Conta Contabil. Classificação (" + naturezaDespesa.NaturezaDespesaCodigo + ")");

                    retorno.AddRange(ValidarMontaContaCorrente(contaContabilDebito));
                }

                int contaContabilCredoraCodigo;
                ContaContabil contaContabilCredito = null;
                if (!int.TryParse(naturezaDespesa.ContaCredora, out contaContabilCredoraCodigo))
                {
                    contaContabilCredoraCodigo = RetirarMascaraNatureza(naturezaDespesa.NaturezaDespesaID, "C", true);
                    contaContabilCredito = contaContabilDA.ConsultarTodos().FirstOrDefault(el => el.ContaContabilCodigo == contaContabilCredoraCodigo);
                }
                else
                {
                    contaContabilCredito = contaContabilDA.ConsultarTodos().FirstOrDefault(el => el.ContaContabilCodigo == contaContabilCredoraCodigo);
                    if (contaContabilCredito == null)
                        throw new CFP.Util.SPFException("Não será possível gerar VPD. A conta credora (" + naturezaDespesa.ContaCredora + ") não esta cadastrada na tabela Conta Contabil. Classificação (" + naturezaDespesa.NaturezaDespesaCodigo + ")");

                    retorno.AddRange(ValidarMontaContaCorrente(contaContabilCredito));
                }
            }
            return retorno;
        }

        protected List<string> ValidarMontaContaCorrente(ContaContabil contaContabil)
        {
            var camposObrigatorios = new List<string>();
            if (contaContabil.TipoContaCorrenteContabilCodigo.HasValue)
            {
                switch ((TipoContaCorrenteContabilEnum)contaContabil.TipoContaCorrenteContabilCodigo.Value)
                {
                    case TipoContaCorrenteContabilEnum.BANCO_AGE_CONTA_BANCARIA_DEST_REC:
                        break;
                    case TipoContaCorrenteContabilEnum.CELULA_DE_DESPESA_COM_ND_DETALHADA:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        camposObrigatorios.Add("LocalizadorID");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        camposObrigatorios.Add("UnidadeGestoraCodigo");
                        break;
                    case TipoContaCorrenteContabilEnum.CELULA_FINANCEIRA:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.CELULA_FINANCEIRA_ANO_MES:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.CELULA_ORCAMENTARIA_DA_DESPESA:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        camposObrigatorios.Add("LocalizadorID");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        camposObrigatorios.Add("UnidadeGestoraCodigo");
                        break;
                    case TipoContaCorrenteContabilEnum.CELULA_ORCAMENTARIA_DA_RECEITA:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.CLASSIF_RECEITA_DEST_RECURSOS:
                        break;
                    case TipoContaCorrenteContabilEnum.CLASSIFICACAO_DA_RECEITA:
                        break;
                    case TipoContaCorrenteContabilEnum.CNPJ_OU_CPF_OU_UG_GESTAO_DESTINACAO_DE_RECURSOS:
                        camposObrigatorios.Add("CredorIdentificacao");
                        break;
                    case TipoContaCorrenteContabilEnum.CNPJ_CPF_999_UG_GEST_DEST_RECURSOS:
                        camposObrigatorios.Add("CredorIdentificacao");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.CNPJ_CPF_UG_GESTAO_MODAL_LICITACAO:
                        //camposObrigatorios.Add("NumeroDocumento");
                        camposObrigatorios.Add("CredorIdentificacao");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.DEST_REC_CATEG_GASTO_ANO_MES:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.DESTINACAO_DE_RECURSOS:
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.EXERCICIO_DESTINACAO_DE_RECURSOS:
                        camposObrigatorios.Add("CredorIdentificacao");
                        break;
                    case TipoContaCorrenteContabilEnum.EXERCICIO_CNPJ_OU_CPF:
                        camposObrigatorios.Add("CredorIdentificacao");
                        break;
                    case TipoContaCorrenteContabilEnum.FONTE_SOF:
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.GUIA_DE_RECEBIMENTO_DEST_RECURSOS:
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        break;
                    case TipoContaCorrenteContabilEnum.INSCRICAO_GENERICA:
                        break;
                    case TipoContaCorrenteContabilEnum.NAO_TEM_CC:
                        break;
                    case TipoContaCorrenteContabilEnum.NUMERO_DE_CONVENIO:
                        camposObrigatorios.Add("NumeroConvenio");
                        break;
                    case TipoContaCorrenteContabilEnum.NUMERO_DO_CONTRATO_DESTINACAO_DE_RECURSOS:
                        camposObrigatorios.Add("NumeroContrato");
                        break;
                    case TipoContaCorrenteContabilEnum.NUMERO_DO_EMPENHO_ANO_MES:
                        //camposObrigatorios.Add("DocumentoNumero");
                        break;
                    case TipoContaCorrenteContabilEnum.NUMERO_DO_REPASSE_FINANCEIRO_CNPJ:
                        break;
                    case TipoContaCorrenteContabilEnum.NUMERO_DO_SUPRIMENTO_DE_FUNDOS_CPF:
                        break;
                    case TipoContaCorrenteContabilEnum.NUMERO_DO_EMPENHO:
                    case TipoContaCorrenteContabilEnum.ORDEM_BANCARIA:
                    case TipoContaCorrenteContabilEnum.GUIA_DE_RECEBIMENTO:
                        //camposObrigatorios.Add("DocumentoNumero");
                        break;
                    case TipoContaCorrenteContabilEnum.PROGRAMA_ACAO:
                        break;
                    case TipoContaCorrenteContabilEnum.REGISTRO_GERAL_DE_IMOVEIS:
                        camposObrigatorios.Add("RegistroGeralDeImoveis");
                        break;
                    case TipoContaCorrenteContabilEnum.SUBITEM_YY_DA_NATUREZA_DA_DESPESA:
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");
                        break;
                    case TipoContaCorrenteContabilEnum.UG_CELULA_FINANCEIRA_ANO_MES:
                        camposObrigatorios.Add("ItemVPD_FonteRecursoID");
                        camposObrigatorios.Add("UnidadeGestoraCodigo");
                        camposObrigatorios.Add("ItemVPD_NaturezaDespesaID");

                        break;
                    case TipoContaCorrenteContabilEnum.UNIDADE_GESTORA_DESTINACAO_DE_RECURSOS:
                        camposObrigatorios.Add("VPDEvento_FonteRecursoID");
                        camposObrigatorios.Add("UnidadeGestoraCodigo");
                        break;
                    case TipoContaCorrenteContabilEnum.UNIDADE_GESTORA_GESTAO:
                        camposObrigatorios.Add("UnidadeGestoraCodigo");
                        break;
                }
            }
            return camposObrigatorios;
        }

        protected int RetirarMascaraNatureza(int naturezaID, string tipoSaldo, bool naturezaDespesa = true, bool apropriacao = false)
        {
            #region igualdade
            Func<char, char, bool> Equality = (mascara, valor) =>
            {

                int val;
                string mask = Convert.ToString(mascara);
                if (int.TryParse(mask, out val))
                {
                    if (mascara == valor)
                        return true;
                }

                int value = int.Parse(valor.ToString());

                switch (mascara)
                {
                    case 'D':
                        if (value == 1 || value == 2)
                        { return true; }
                        return false;
                    case 'C':
                        if (value >= 1 && value <= 5)
                        { return true; }
                        return false;
                    case 'A':
                        if (value >= 1 && value <= 9)
                        { return true; }
                        return false;
                    case 'B':
                        if (value >= 1 && value <= 9)
                        { return true; }
                        return false;
                    case 'L':
                        if (value >= 1 && value <= 9)
                        { return true; }
                        return false;
                    case 'Z':
                        if (value >= 1 && value <= 9)
                        { return true; }
                        return false;
                    default:
                        return false;
                }
            };
            #endregion

            var conta = string.Empty;
            if (naturezaDespesa)
            {
                var natureza = naturezaDespesaDA.ConsultarTodos().FirstOrDefault(el => el.NaturezaDespesaID == naturezaID);
                if (!apropriacao)
                    conta = (tipoSaldo.Equals("C")) ? natureza.ContaCredora : natureza.ContaDevedora;
                else
                    conta = (tipoSaldo.Equals("C")) ? natureza.ContaApropriacaoCredora : natureza.ContaApropriacaoDevedora;

                if (string.IsNullOrWhiteSpace(conta))
                    throw new CFP.Util.SPFException("Conta Contabil (" + tipoSaldo + ")  não cadastrada para a Natureza de Despesa " + natureza.NaturezaDespesaCodigo);
            }
            else
            {
                var natureza = naturezaReceitaDA.ConsultarTodos().FirstOrDefault(el => el.NaturezaReceitaID == naturezaID);
                if (!apropriacao)
                    conta = (tipoSaldo.Equals("C")) ? natureza.ContaCredora : natureza.ContaDevedora;
                else
                    conta = (tipoSaldo.Equals("C")) ? natureza.ContaApropriacaoCredora : natureza.ContaApropriacaoDevedora;

                if (string.IsNullOrWhiteSpace(conta))
                    throw new CFP.Util.SPFException("Conta Contabil (" + tipoSaldo + ") não cadastrada para a Natureza de Receita " + natureza.NaturezaReceitaCodigo);
            }

            var dados = from CC in contaContabilDA.ConsultarTodos()
                        where CC.Situacao == "A" && CC.TipoSaldo == tipoSaldo
                        select CC;

            String[] validos = dados.ToList().Select(x => x.ContaContabilCodigo.ToString()).Where(x =>
                 Equality(conta[0], x[0])
                 &&
                 Equality(conta[1], x[1])
                 &&
                 Equality(conta[2], x[2])
                  &&
                 Equality(conta[3], x[3])
                  &&
                 Equality(conta[4], x[4])
                  &&
                 Equality(conta[5], x[5])
                  &&
                 Equality(conta[6], x[6])
                  &&
                 Equality(conta[7], x[7])
                ).ToArray();

            if (validos.Length != 0)
                return int.Parse(validos.FirstOrDefault());

            return 0;
        }

    }
}