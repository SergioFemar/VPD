using CFP.ModelData.Logic;
using CFP.ModelData.Logic.Execucao;
using CFP.ModelData.Logic.Execucao.ViewModel;
using CFP.ModelData.Logic.Orcamento;
using CFP.ModelData.Logic.TabelaGeral.Enum;
using CFP.Util;
using CFP.Util.Execucao;
using CFP.Util.Generico;
using CFPMVC.Util;
using CFPMVC.Util.Execucao;
using CPF.DataApplicator.Logic.Execucao;
using CPF.DataApplicator.Logic.Orcamento;
using MVC.Util;
using SGI.Web.MVC;
using SGI.Web.MVC.Report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace CFPMVC.Areas.Execucao.Controllers
{
    [GridViewState("#grid", "ConsultarGrid")]
    public class VPDPreviaController : VPDBaseController
    {
        protected Permissoes permissoes;
        private BloqueioUGDataApplicator dtfecharUG;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            permissoes = new Permissoes(filterContext);
            permissoes.AddBotao("Criar", "Criar", icone: "icon icon-file")
            .AddBotao("Visualizar", "Visualizar", icone: "icon icon-search")
            .AddBotao("VisualizarLancamentoContabil", "Visualizar Lanç. Contábil", icone: "icon-search", ignorarpermissao: true);
            TempData["BOTOES"] = permissoes.SetaTempData();

            permissoes = new Permissoes(filterContext);
            permissoes.AddBotao("ImprimirVPD", "Imprimir", new Dictionary<string, string>() { { "class", "btn btn-primary" } }, "icon icon-print icon-white", true);
            //permissoes.AddBotao("ImprimirOBGenerica", "Imprimir Doc. Associado(s)", new Dictionary<string, string>() { { "class", "btn btn-primary" } }, "icon icon-print icon-white", true);
            TempData["BOTAO_IMPRIMIR"] = permissoes.SetaTempData();
        }

        // GET: /Execucao/VPDPrevia/
        public ActionResult Index()
        {
            VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.Dispose();
            CriaViewBagIndex();
            return View();
        }

        public ActionResult ConsultaModeloDeLancamentoVPD(string id)
        {
            var mensagem = string.Empty;
            if (UnidadeGestoraEhFechada(ref mensagem, DateTime.Now))
            {
                EmitirMensagem(mensagem, ETipoMensagem.Erro);
                return RedirectToAction("Index");
            }
            VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.Dispose();
            return RedirectToAction("ConsultaModeloDeLancamento", "Documento", new { id = "VPD" });
        }

        public ActionResult Criar()
        {
            String msg = string.Empty;
            if (this.UnidadeGestoraEhFechada(ref msg, DateTime.Now))
            {
                this.EmitirMensagem(msg, ETipoMensagem.Aviso);
                return RedirectToAction("Index");
            }

            if (VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM == null)
                VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM = new VariacaoPatrimonialDiminutivaViewModel();

            var sessionVm = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao;
            CriarViewBag();

            return View(sessionVm.variacaoPatrimonialDiminutivaVM);
        }

        [HttpPost]
        public ActionResult Criar(VariacaoPatrimonialDiminutivaViewModel instancia)
        {
            try
            {
                var now = DateTime.Now;
                String msg = string.Empty;
                if (this.UnidadeGestoraEhFechada(ref msg, now))
                {
                    this.EmitirMensagem(msg, ETipoMensagem.Aviso);
                    {
                        CriarViewBag();
                        return View(instancia);
                    }
                }

                var dataExercicioAtual = ExercicioAnoSessao.Sessao.AnoExercicioCorrente;
                if (instancia.DataEmissao.Year < dataExercicioAtual)
                {
                    this.EmitirMensagem("A Data informada não pode ser anterior ao Exercício atual", ETipoMensagem.Aviso);
                    CriarViewBag();
                    return View(instancia);
                }

                var vpdVM = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM;
                if (vpdVM.ListaItemVPD == null || !vpdVM.ListaItemVPD.Any())
                {
                    this.EmitirMensagem("Informe ao menos um item da VPD", ETipoMensagem.Aviso);
                    CriarViewBag();
                    return View(instancia);
                }
                var documento = MontaObjetoVPDPrevia(instancia, now);
                const string str = "Campo Obrigatório";
                if (String.IsNullOrWhiteSpace(instancia.CredorIdentificacao))
                    ModelState.AddModelError("CredorIdentificacao", str);

                if (!ModelState.IsValid)
                {
                    CriarViewBag();
                    return View(instancia);
                }

                #region Monta Lista de Processos a ser executados ao criar uma VPD
                List<ProcessosDoLancamentoContabil> processosVPD = new List<ProcessosDoLancamentoContabil>();
                var configVPD = new ConfiguracaoLancamentoContabil()
                {
                    Exercicio = ExercicioAnoSessao.Sessao.AnoExercicioCorrente.Value,
                    CodigoDoDocumento = "VPD",
                    Evento = false,
                    UsarEventoDoDocumento = false,
                    PermitirInscricaoDoEventoInvalida = true,
                    Estorno = false,
                    Atualizar = false,
                    NaturezaDespesa = true,

                };
                using (SimplesLancamentoContabil contabil = new SimplesLancamentoContabil())
                {
                    contabil.SetDocumento(documento);
                    contabil.SetConfiguracao(configVPD);
                    processosVPD = contabil.MontarProcessos();
                }
                #endregion

                #region Criar VPD
                List<ChaveConta> chaves = new List<ChaveConta>();
                SimplesLancamentoContabil.AdicionarChaves(chaves, processosVPD);

                var move = new MovimentacaoFinanceira();
                move.AdicionarProcesso(processosVPD);
                move.Validar();
                move.LiberarDocumento(documento);
                using (ControleConta cc = new ControleConta(chaves))
                {
                    cc.Validar();
                    using (var ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = ControleConta.IsolationLevelLancamentoContabil }))
                    {
                        using (GenericoSalvarLancamento save = new GenericoSalvarLancamento(documento, configVPD))
                        {
                            save.GerarLancamentos(processosVPD);
                            save.Salvar();
                        }
                        ts.Complete();
                    }
                }
                #endregion

                DocumentoGerenciarSession.Sessao.Dispose("DocumentoGerenciarSession_SESSAO");
                EmitirMensagem(ConfigurationManager.AppSettings["MENSAGEM_SUCESSO"] + " VPD Prévia Nº " + documento.Numero);
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException e)
            {
                EmitirMensagem(e);
            }
            catch (CFP.Util.SPFException ex)
            {
                EmitirMensagem(ex.Message, ETipoMensagem.Erro);
            }
            CriarViewBag();
            return View(instancia);
        }

        public ActionResult Visualizar(int id)
        {
            if (!ControllerContext.VerificaAcessoTabelaDocumento(id))
                return RedirectToAction("Index");

            var vpdPrevia = documentoDA.FindById(id);

            if (vpdPrevia == null)
            {
                EmitirMensagem(ConfigurationManager.AppSettings["MENSAGEM_REGISTRO_NAO_ENCONTRADO"], ETipoMensagem.Erro);
                return RedirectToAction("Index");
            }
            VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.Dispose();
            CriarViewBagVisualizar(vpdPrevia);

            if (!vpdPrevia.DocumentoOriginalID.HasValue && itemVariacaoPatrimonialDiminutivaDA.RetornaValorAEmpenharByDocumento(vpdPrevia) > 0
                && ControllerContext.RetornaValidacaoBotaoEstorno(vpdPrevia.UnidadeGestoraCodigoEmitente))
            {
                var permissaoEstornar = new Permissoes("Execucao/VPDPrevia");
                var atributos = new Dictionary<string, string> { { "class", "btn btn-primary" } };
                permissaoEstornar.AddBotao("Estornar", "Estornar", atributos);
                TempData["BOTAO_ESTORNAR"] = permissaoEstornar.SetaTempData();
            }

            return View(vpdPrevia);
        }

        public ActionResult ImprimirVPD(int documentoId)
        {
            if (!ControllerContext.VerificaAcessoTabelaDocumento(documentoId))
                return RedirectToAction("Index");

            var documento = documentoDA.ObterUnico(x => x.DocumentoID == documentoId);

            if (documento == null)
            {
                throw new CFP.Util.SPFException("Variação Patrimonial Diminutiva Prévia não localizada.");
            }

            //var documento = documentoDA.ObterUnico(x => x.DocumentoID == documentoId);
            var responsavelEmissao = GsiUtil.RetornaUsuarioGsi(UsuarioLogado.GetFullName());
            var er = new EmissorDeRelatorio("Execucao/VariacaoPatrimonialDiminutivaPrevia_Retrato", EFormatoImpressao.PDF);
            var parametros = new Dictionary<string, object>();
            parametros.Add("DataEmissao", DateTime.Now);
            parametros.Add("fraseCabecalho", unidadeGestoraDA.RetornaTituloRelatorio(ExercicioAnoSessao.Sessao.UnidadeGestoraAtual.Value));
            parametros.Add("UG", ExercicioAnoSessao.Sessao.UnidadeGestoraAtual.Value + " - " + ExercicioAnoSessao.Sessao.UnidadeGestoraAtualNome);
            parametros.Add("DocumentoID", documentoId);
            Guid Hashcode = auditoriaDA.AuditaImpressao(this.UsuarioLogado.Nome, this.UsuarioLogado.GetFullName(),
                this.UsuarioLogado.CPF,
                er.CaminhoDoRelatorio,
                parametros,
                DateTime.Now); //Aqui é invocado o método que gera um código único para a impressão.
            parametros.Add("Hashcode", Hashcode);
            parametros.Add("FonteDeDados", documentoDA.Contexto.Database.Connection.ConnectionString);


            er.Parametros = parametros;
            return er.Imprimir(true, "Documento", "Imprimir");
        }

        public ActionResult Estornar(int id)
        {
            if (!ControllerContext.VerificaAcessoTabelaDocumento(id))
                return RedirectToAction("Index");

            String msg = string.Empty;
            if (this.UnidadeGestoraEhFechada(ref msg, DateTime.Now))
            {
                this.EmitirMensagem(msg, ETipoMensagem.Aviso);
                return RedirectToAction("Index");
            }

            var vpdPrevia = documentoDA.FindById(id);
            if (vpdPrevia == null)
            {
                EmitirMensagem(ConfigurationManager.AppSettings["MENSAGEM_REGISTRO_NAO_ENCONTRADO"], ETipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (itemVariacaoPatrimonialDiminutivaDA.RetornaValorAEmpenharByDocumento(vpdPrevia) == 0)
            {
                EmitirMensagem("Saldo Insuficiente para realizar o Estorno.", ETipoMensagem.Erro);
                return RedirectToAction("Visualizar", "VPDPrevia", new { id = id });
            }

            var vpdEstorno = (VPDEstorno)vpdPrevia;
            CriarObjetoEstorno(vpdPrevia, ref vpdEstorno);

            return View(vpdEstorno);
        }

        [HttpPost]
        public JsonResult Estornar(VPDEstorno vpdEstorno)
        {
            string status = "OK";
            string mensagem = string.Empty;
            string numeroDocumentoGerado = string.Empty;
            try
            {
                var now = DateTime.Now;
                if (this.UnidadeGestoraEhFechada(ref mensagem, now))
                    throw new CFP.Util.SPFException(mensagem);

                if (string.IsNullOrEmpty(vpdEstorno.DataEmissao.ToString()) || vpdEstorno.DataEmissao.Year == 1)
                {
                    throw new CFP.Util.SPFException("A data de emissão é obrigatória!");
                }

                var documento = MontaObjetoVPDPreviaEstorno(vpdEstorno, now);

                #region Monta Lista de Processos a ser executados ao estornar uma VPD
                List<ProcessosDoLancamentoContabil> processosVPD = new List<ProcessosDoLancamentoContabil>();
                var configVPD = new ConfiguracaoLancamentoContabil()
                {
                    Exercicio = ExercicioAnoSessao.Sessao.AnoExercicioCorrente.Value,
                    CodigoDoDocumento = "VPD",
                    Evento = false,
                    UsarEventoDoDocumento = false,
                    PermitirInscricaoDoEventoInvalida = true,
                    Estorno = true,
                    Atualizar = false,
                    NaturezaDespesa = true,

                };
                using (SimplesLancamentoContabil contabil = new SimplesLancamentoContabil())
                {
                    contabil.SetDocumento(documento);
                    contabil.SetConfiguracao(configVPD);
                    processosVPD = contabil.MontarProcessos();
                }
                #endregion

                #region Criar VPD
                List<ChaveConta> chaves = new List<ChaveConta>();
                SimplesLancamentoContabil.AdicionarChaves(chaves, processosVPD);

                var move = new MovimentacaoFinanceira();
                move.AdicionarProcesso(processosVPD);
                move.Validar();
                move.LiberarDocumento(documento);
                using (ControleConta cc = new ControleConta(chaves))
                {
                    cc.Validar();
                    using (var ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = ControleConta.IsolationLevelLancamentoContabil }))
                    {
                        using (GenericoSalvarLancamento save = new GenericoSalvarLancamento(documento, configVPD))
                        {
                            save.GerarLancamentos(processosVPD);
                            save.Salvar();
                        }
                        ts.Complete();

                        numeroDocumentoGerado = documento.Numero;

                    }
                }
                #endregion
            }
            catch (CFP.Util.SPFException ex)
            {
                status = "Erro";
                mensagem = ex.Message;
            }

            return Json(new { Status = status, Mensagem = mensagem, NumeroDocumentoGerado = numeroDocumentoGerado }, JsonRequestBehavior.AllowGet);

            //return View(vpdEstorno);
        }

        [IgnorarPermissaoPortal]
        public JsonResult ConsultarGrid(GridSettings gridSettings)
        {
            int qtdRegistros;
            var situacaoVPD = new Rule() { field = "Situacao", data = "A" };
            var unidadeGestoraEmitente = new Rule() { field = "UnidadeGestoraCodigoEmitente", data = ExercicioAnoSessao.Sessao.UnidadeGestoraAtual.Value.ToString() };


            if (gridSettings.Where == null)
                gridSettings.Where = new MVC.Util.Filter() { rules = new Rule[] { situacaoVPD, unidadeGestoraEmitente } };
            else
            {
                var regras = gridSettings.Where.rules.ToList();
                if (!ExercicioAnoSessao.Sessao.EhUsuarioSemac && !regras.Any(el => el.field == "UnidadeGestoraCodigoEmitente"))
                    regras.Add(unidadeGestoraEmitente);

                regras.Add(situacaoVPD);
                gridSettings.Where.rules = regras.ToArray();
            }

            var lista = documentoDA.GridObterVPDPrevia(gridSettings, out qtdRegistros);

            var list = lista.AsEnumerable()
                .Select(d => new GridRow
                {
                    id = d.DocumentoID,
                    cell = new object[]
                    {
                        d.DocumentoID,
                        d.Numero,
                        d.DataEmissao.ToString("dd/MM/yyyy"),
                        d.UnidadeGestora.UnidadeGestoraCodigo + " - " + d.UnidadeGestora.Nome,
                        d.Credor != null ? d.Credor.CredorIdentificacao + " - " + d.Credor.Nome : string.Empty,
                        d.ValorDocumento.ToString("N2"),
                        d.ItemVariacaoPatrimonialDiminutiva.Select(x => new { TipoVPD = x.TipoVPD.Equals("A") ? "Prévia" : "Posterior" }).Select(x => x.TipoVPD).FirstOrDefault(),
                        d.Documento2 != null ? d.Documento2.Numero : string.Empty,
                    }
                });

            return list.Retornar(gridSettings, qtdRegistros);
        }

        [IgnorarPermissaoPortal]
        public JsonResult ConsultarSubGrid(GridSettings gridSettings)
        {
            int qtdRegistros;

            var arrayRequest = Request.CurrentExecutionFilePath.Split('/');
            var sizeArray = arrayRequest.Count();
            var rowId = arrayRequest[sizeArray - 1].ObjToInt32();

            var lista = documentoDA.ConsultarTodosSemExercicio().FirstOrDefault(el => el.DocumentoID == rowId).ItemVariacaoPatrimonialDiminutiva;

            qtdRegistros = lista.Count();
            var list = lista.AsEnumerable()
            .Select(d => new
            {
                id = d.VariacaoPatrimonialDiminutivaItemID,
                cell = new object[]
                    {
                        d.VariacaoPatrimonialDiminutivaItemID.ToString(),
                        d.FonteRecursoID.HasValue ? d.IdentificadorUsoCodigo.ToString() + fonteRecursoDA.RetornaNumeroDescricaoFonteRecurso(d.FonteRecursoID.Value) : string.Empty,
                        d.NaturezaDespesaID.HasValue ? naturezaDespesaDA.RetornaCodigoDescricaoNaturezaDespesa(d.NaturezaDespesaID.Value, d.Documento.Exercicio) : string.Empty,
                        d.ValorSolicitado.ToString("N2"),
                        !d.Documento.DocumentoOriginalID.HasValue? itemVariacaoPatrimonialDiminutivaDA.RetornaValorEstorno(d.VariacaoPatrimonialDiminutivaItemID).ToString("N2") : d.ValorEstorno.ToString("N2"),
                        itemVariacaoPatrimonialDiminutivaDA.RetornaSaldoDisponivel(d.VariacaoPatrimonialDiminutivaItemID).ToString("N2"),
                    }
            });

            var jsonData = new
            {
                total = (int)Math.Ceiling(qtdRegistros / (decimal)gridSettings.PageSize),
                page = gridSettings.PageIndex,
                records = qtdRegistros,
                rows = list
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [IgnorarPermissaoPortal]
        public JsonResult ConsultarGridCriarItemVPD(GridSettings gridSettings)
        {
            var lista = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM.ListaItemVPD;
            int qtdRegistros = lista != null ? lista.Count : 0;
            decimal valorTotal = lista != null ? lista.Sum(el => el.ValorSolicitado) - lista.Sum(el => el.ValorEstorno) : 0;

            var list = lista != null ? lista.AsEnumerable()
                .Select(d => new GridRow
                {
                    id = d.VariacaoPatrimonialDiminutivaItemID,
                    cell = new object[]
                    {
                        d.VariacaoPatrimonialDiminutivaItemID,
                        d.FonteRecursoDescricao,
                        d.FonteRecursoID,
                        d.NaturezaDespesaCodigoDescricao,
                        d.NaturezaDespesaID,
                        d.ValorSolicitado.ToString("N2"),
                        valorTotal.ToString("N2"),
                    }
                }) : new List<GridRow>();

            return list.Retornar(gridSettings, qtdRegistros);
        }

        [IgnorarPermissaoPortal]
        public JsonResult ConsultarGridItemVPD(GridSettings gridSettings)
        {
            var lista = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM.ListaItemVPD;
            int qtdRegistros = lista != null ? lista.Count : 0;
            decimal valorTotal = lista != null ? lista.Sum(el => el.ValorSolicitado) - lista.Sum(el => el.ValorEstorno) : 0;

            var list = lista != null ? lista.AsEnumerable()
                .Select(d => new GridRow
                {
                    id = d.VariacaoPatrimonialDiminutivaItemID,
                    cell = new object[]
                    {
                        d.VariacaoPatrimonialDiminutivaItemID,
                        d.FonteRecursoDescricao,
                        d.FonteRecursoID,
                        d.NaturezaDespesaCodigoDescricao,
                        d.NaturezaDespesaID,
                        d.ValorSolicitado.ToString("N2"),
                        valorTotal.ToString("N2"),
                        itemVariacaoPatrimonialDiminutivaDA.RetornaValorAEmpenhar(d.VariacaoPatrimonialDiminutivaItemID).ToString("N2"),
                        d.ValorEstorno.ToString("N2"),
                        (itemVariacaoPatrimonialDiminutivaDA.RetornaValorEmpenhado(d.VariacaoPatrimonialDiminutivaItemID) - itemVariacaoPatrimonialDiminutivaDA.RetornaValorDevolvido(d.VariacaoPatrimonialDiminutivaItemID) ).ToString("N2"),
                        itemVariacaoPatrimonialDiminutivaDA.RetornaSaldoDisponivel(d.VariacaoPatrimonialDiminutivaItemID).ToString("N2")
                    }
                }) : new List<GridRow>();

            return list.Retornar(gridSettings, qtdRegistros);
        }

        [IgnorarPermissaoPortal]
        public JsonResult VerificaEventoTipoDocumento(string eventoTipoDocumentoID)
        {
            if (string.IsNullOrWhiteSpace(eventoTipoDocumentoID))
                return Json(new { Status = "Erro", Mensagem = "Evento Tipo Documento não informado" }, JsonRequestBehavior.AllowGet);

            var eventoTipoDocumentoSplit = eventoTipoDocumentoID.Split('_');
            var eventoID = eventoTipoDocumentoSplit[0].ObjToInt32();

            var evento = eventoDA.ConsultarTodos().FirstOrDefault(el => el.EventoID == eventoID);
            var dominioTipoInscricao = dominioDA.RetornaNome(evento.DominioIDTipoInscricao);

            return Json(new { Status = "OK", DominioIDTipoInscricao = evento.DominioIDTipoInscricao, DominioTipoInscricao = dominioTipoInscricao }, JsonRequestBehavior.AllowGet);
        }

        [IgnorarPermissaoPortal]
        public JsonResult ValidaContaNatureza(int id)
        {
            var status = "OK";
            var mensagem = string.Empty;
            var camposObrigatorios = new List<string>();
            try
            {
                camposObrigatorios = ValidarContaNatureza(id);
            }
            catch (CFP.Util.SPFException ex)
            {
                status = "Erro";
                mensagem = ex.Message;
            }
            return Json(new { Status = status, Mensagem = mensagem, CamposObrigatorios = camposObrigatorios }, JsonRequestBehavior.AllowGet);
        }

        [IgnorarPermissaoPortal]
        public JsonResult AdicionarItem(ItemVPD itemVPD)
        {
            string status = "OK";
            string mensagem = string.Empty;
            try
            {
                if (!ModelState.IsValid)
                {
                    mensagem = "Campo Obrigatório";

                    if (!itemVPD.NaturezaDespesaID.HasValue)
                        return Json(new { Status = "Erro", Campo = "ItemVPD_NaturezaDespesaID", Mensagem = mensagem }, JsonRequestBehavior.AllowGet);

                    if (itemVPD.ValorSolicitado == 0)
                        return Json(new { Status = "Erro", Campo = "ItemVPD_Valor", Mensagem = mensagem }, JsonRequestBehavior.AllowGet);
                }
                AdicionarItemVPD(itemVPD);
                mensagem = "Item adicionado com sucesso.";
            }
            catch (CFP.Util.SPFException ex)
            {
                status = "Erro";
                mensagem = ex.Message;
            }
            return Json(new { Status = status, Mensagem = mensagem }, JsonRequestBehavior.AllowGet);
        }

        [IgnorarPermissaoPortal]
        public JsonResult ExcluirItem(int vpdItem, string naturezaDespesaID)
        {
            string status = "OK";
            string mensagem = string.Empty;
            try
            {
                ExcluirItemVPD(vpdItem, naturezaDespesaID);
                mensagem = "Item removido com sucesso.";

            }
            catch (CFP.Util.SPFException ex)
            {
                status = "Erro";
                mensagem = ex.Message;
            }

            return Json(new { Status = status, Mensagem = mensagem }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, IgnorarPermissaoPortal]
        public JsonResult FonteRecursoFinanceiroLOASelect2Query(Select2Param param)
        {
            int ugCodigo;

            if (!int.TryParse(param.campoFiltro, out ugCodigo) || string.IsNullOrWhiteSpace(param.query))
                return Json(new Select2 { }, JsonRequestBehavior.AllowGet);

            int idUso;
            string numeroFonte;
            string texto = param.query.ToLower();
            if (param.query.Length > 1)
            {
                int.TryParse(param.query.Substring(0, 1), out idUso);
                numeroFonte = param.query.Substring(1);
            }
            else
            {
                int.TryParse(param.query, out idUso);
                numeroFonte = string.Empty;
            }

            var consultaFonteFinanceiroLOA = financeiroLOADA.ConsultarTodos()
                .Where(t => t.PropostaLOA.ExercicioLOA == ExercicioAnoSessao.Sessao.AnoExercicioCorrente.Value
                && t.PropostaLOA.Atual == "S"
                && t.Situacao == "A"
                && t.IdentificadorUsoCodigo == idUso
                && (t.FonteRecurso.NumeroFonteRecurso.StartsWith(numeroFonte) || t.FonteRecurso.DescricaoFonteRecurso.Contains(texto)));

            var consultaFontePedidoAlteracao = new PedidoAlteracaoAcrescimoNovaNaturezaDataApplicator(contextoORCAM).ConsultarTodos()
                .Where(t => t.PedidoAlteracao.Exercicio == ExercicioAnoSessao.Sessao.AnoExercicioCorrente.Value
                       && t.IdentificadorUsoCodigo == idUso
                       && (t.FonteRecurso.NumeroFonteRecurso.StartsWith(numeroFonte) || t.FonteRecurso.DescricaoFonteRecurso.Contains(texto)));

            var resultadoConsulta = consultaFonteFinanceiroLOA.ToList().Select(x => new { x.FonteRecursoID, IdentificadorUsoCodigo = x.IdentificadorUsoCodigo ?? 0, x.FonteRecurso.NumeroFonteRecurso, x.FonteRecurso.DescricaoFonteRecurso })
                   .Union(consultaFontePedidoAlteracao.ToList().Select(x => new { x.FonteRecursoID, x.IdentificadorUsoCodigo, x.FonteRecurso.NumeroFonteRecurso, x.FonteRecurso.DescricaoFonteRecurso }));

            if (param.edicao)
            {
                return Json(new { id = string.Empty, text = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(resultadoConsulta
                    .Take(System.Configuration.ConfigurationManager.AppSettings["QtdRegistrosSelect2"].StrToInt32())
                    .OrderBy(x => x.NumeroFonteRecurso)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        id = string.Format("{0}_{1}", x.FonteRecursoID, x.IdentificadorUsoCodigo),
                        text =
                            string.Format("{0}{1} - {2}", x.IdentificadorUsoCodigo, x.NumeroFonteRecurso,
                                x.DescricaoFonteRecurso)
                    }).Distinct(), JsonRequestBehavior.AllowGet);
            }
        }

        #region Métodos privados
        private IEnumerable<FinanceiroLOA> Obter(int unidadeGestoraCodigo)
        {
            return financeiroLOADA.ConsultarTodos().Where(x => x.PropostaLOA.Situacao.Equals("A") && x.PropostaLOA.Atual.Equals("S")
                && x.PropostaLOA.UnidadeGestoraCodigo == unidadeGestoraCodigo).AsEnumerable();
        }

        private void CriaViewBagIndex()
        {
            if (!ExercicioAnoSessao.Sessao.EhUsuarioSemac)
            {
                var ugAtual = unidadeGestoraDA.ObterUnico(el => el.UnidadeGestoraCodigo == ExercicioAnoSessao.Sessao.UnidadeGestoraAtual.Value);
                ViewBag.UnidadeGestora = ugAtual.UnidadeGestoraCodigo + " - " + ugAtual.Nome;
                ViewBag.UnidadeGestoraCodigo = ugAtual.UnidadeGestoraCodigo;
            }
            ViewBag.ListaIduso = identificadorUsoDA.ConsultarTodos().Where(x => x.Situacao.Equals("A")).AsEnumerable()
                .ToSelectList(x => x.IdentificadorUsoCodigo, x => x.IdentificadorUsoCodigo.ToString() + " - " + x.Descricao);
        }

        private void CriarViewBag()
        {
            var sessionVm = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao;
            var now = DateTime.Now;
            ViewBag.DataEmissao = now;
            ViewBag.BloqueioAnosAnteriores = ("01/01/" + ExercicioAnoSessao.Sessao.AnoExercicioCorrente).StrToDateTime();
            dtfecharUG = new BloqueioUGDataApplicator(contextoEXEFIN);
            ViewBag.ListaUGFechadas = dtfecharUG.RetornaListaMesesBloqueados(ExercicioAnoSessao.Sessao.UnidadeGestoraAtual.Value);
            if (RetornaDataAtualDocumento() != null)
                sessionVm.variacaoPatrimonialDiminutivaVM.DataEmissao = now;
            sessionVm.variacaoPatrimonialDiminutivaVM.UnidadeGestoraCodigo = ExercicioAnoSessao.Sessao.UnidadeGestoraAtual.Value;
            sessionVm.variacaoPatrimonialDiminutivaVM.ItemVPD = new ItemVPD();

            permissoes.AddBotao("SalvarItemVPD", "Adicionar", ignorarpermissao: true);
            permissoes.AddBotao("ExcluirItemVPD", "Excluir", icone: "icon icon-trash", ignorarpermissao: true);
            TempData["BOTOES"] = permissoes.SetaTempData();
        }

        private void CriarViewBagVisualizar(Documento documento)
        {
            var sessionVm = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao;
            if (sessionVm.variacaoPatrimonialDiminutivaVM == null)
                sessionVm.variacaoPatrimonialDiminutivaVM = new VariacaoPatrimonialDiminutivaViewModel();

            sessionVm.variacaoPatrimonialDiminutivaVM.ListaItemVPD = new List<ListaItemVPD>();

            foreach (var i in documento.ItemVariacaoPatrimonialDiminutiva)
            {
                var listaItemVPD = new ListaItemVPD();
                listaItemVPD.VariacaoPatrimonialDiminutivaItemID = i.VariacaoPatrimonialDiminutivaItemID;
                listaItemVPD.FonteRecursoDescricao = i.FonteRecursoID.HasValue ? i.IdentificadorUsoCodigo.Value.ToString() + fonteRecursoDA.RetornaNumeroDescricaoFonteRecurso(i.FonteRecursoID.Value) : string.Empty;
                listaItemVPD.FonteRecursoID = i.FonteRecursoID;
                listaItemVPD.NaturezaDespesaID = i.NaturezaDespesaID;
                listaItemVPD.NaturezaDespesaCodigoDescricao = i.NaturezaDespesaID.HasValue ? naturezaDespesaDA.RetornaCodigoDescricaoNaturezaDespesa(i.NaturezaDespesaID.Value, i.Documento.Exercicio) : string.Empty;
                listaItemVPD.ValorSolicitado = i.ValorSolicitado;
                listaItemVPD.ValorEmpenhar = itemVariacaoPatrimonialDiminutivaDA.RetornaValorAEmpenhar(i.VariacaoPatrimonialDiminutivaItemID);
                listaItemVPD.ValorEstorno = !i.Documento.DocumentoOriginalID.HasValue ? itemVariacaoPatrimonialDiminutivaDA.RetornaValorEstorno(i.VariacaoPatrimonialDiminutivaItemID) : i.ValorEstorno;
                listaItemVPD.ValorEmpenhado = itemVariacaoPatrimonialDiminutivaDA.RetornaValorEmpenhado(i.VariacaoPatrimonialDiminutivaItemID);
                listaItemVPD.ValorTotal = documento.ItemVariacaoPatrimonialDiminutiva.Sum(el => el.ValorSolicitado) - documento.ItemVariacaoPatrimonialDiminutiva.Sum(el => el.ValorEstorno);
                sessionVm.variacaoPatrimonialDiminutivaVM.ListaItemVPD.Add(listaItemVPD);
            }
        }

        private void CriarObjetoEstorno(Documento documento, ref VPDEstorno vpdEstorno)
        {
            vpdEstorno.ListaVPDEstorno = new List<ItemVPDEstorno>();
            foreach (var i in documento.ItemVariacaoPatrimonialDiminutiva)
            {
                var itemVPDEstorno = new ItemVPDEstorno();
                itemVPDEstorno.ItemVariacaoPatrimonialDiminutivaID = i.VariacaoPatrimonialDiminutivaItemID;
                itemVPDEstorno.FonteRecursoDescricao = i.FonteRecursoID.HasValue ? i.IdentificadorUsoCodigo.ToString() + fonteRecursoDA.RetornaNumeroDescricaoFonteRecurso(i.FonteRecursoID.Value) : string.Empty;
                itemVPDEstorno.FonteRecursoID = i.FonteRecursoID;
                itemVPDEstorno.NaturezaDespesaID = i.NaturezaDespesaID;
                itemVPDEstorno.NaturezaDespesaCodigoDescricao = i.NaturezaDespesaID.HasValue ? naturezaDespesaDA.RetornaCodigoDescricaoNaturezaDespesa(i.NaturezaDespesaID.Value, i.Documento.Exercicio) : string.Empty;
                itemVPDEstorno.ValorEstorno = 0;
                itemVPDEstorno.ValorEmpenhar = itemVariacaoPatrimonialDiminutivaDA.RetornaSaldoDisponivel(i.VariacaoPatrimonialDiminutivaItemID);
                if (itemVPDEstorno.ValorEmpenhar > 0)
                    vpdEstorno.ListaVPDEstorno.Add(itemVPDEstorno);
            }
            ViewBag.DataVPDOriginal = documento.DataEmissao;
            vpdEstorno.Numero = documento.Numero;
            vpdEstorno.DataEmissao = DateTime.Today;
            if (RetornaDataAtualDocumento() != null)
                vpdEstorno.DataEmissao = DateTime.Today;
            ViewBag.MesesBloqueados = fecharUGDA.RetornaListaMesesBloqueados(documento.UnidadeGestoraCodigoEmitente);
            vpdEstorno.TotalEstorno = vpdEstorno.ListaVPDEstorno.Sum(el => el.ValorEmpenhar);
        }

        private Documento MontaObjetoVPDPrevia(VariacaoPatrimonialDiminutivaViewModel instancia, DateTime now)
        {
            Documento documento = new Documento();
            documento.CredorIdentificacao = instancia.CredorIdentificacao;
            documento.DataEmissao = instancia.DataEmissao;
            documento.DataContabilizacao = instancia.DataEmissao;
            documento.NaturezaDespesaID = instancia.NaturezaDespesaID;
            documento.Observacao = instancia.Observacao;
            documento.UnidadeGestoraCodigoEmitente = instancia.UnidadeGestoraCodigo;

            var unidadeGestora = unidadeGestoraDA.ConsultarTodos().FirstOrDefault(el => el.UnidadeGestoraCodigo == documento.UnidadeGestoraCodigoEmitente);
            documento.ItemVariacaoPatrimonialDiminutiva = new List<ItemVariacaoPatrimonialDiminutiva>();
            foreach (var i in VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM.ListaItemVPD)
            {
                var vpdItem = new ItemVariacaoPatrimonialDiminutiva();
                vpdItem.FonteRecursoID = i.FonteRecursoID;
                vpdItem.NaturezaDespesaID = i.NaturezaDespesaID;
                vpdItem.ValorSolicitado = i.ValorSolicitado;
                vpdItem.ValorEstorno = 0;
                vpdItem.TipoVPD = "A";
                vpdItem.IdentificadorUsoCodigo = i.IdentificadorUsoCodigo;
                documento.ItemVariacaoPatrimonialDiminutiva.Add(vpdItem);
            }
            documento.UnidadeGestoraDataInicioVigenciaEmitente = unidadeGestora.UnidadeGestoraDataInicioVigencia;
            documento.Situacao = "A";
            documento.Estorno = "N";
            documento.ValorEstorno = 0;
            documento.DominioIDGrupoDocumentoContabil = dominioDA.RetornaDominioID(DominioCategoriaEnum.GRUPODOCUMENTOCONTABIL, "VPD");
            documento.DataContabilizacao = instancia.DataEmissao;
            documento.DataInclusaoRegistro = now;
            documento.UsuarioInclusaoRegistro = UsuarioLogado.GetFullName();
            documento.ValorDocumento = VariacaoPatrimonialDiminutivaGerenciarSessao.Sessao.variacaoPatrimonialDiminutivaVM.ListaItemVPD.Sum(el => el.ValorSolicitado);
            return documento;
        }

        private Documento MontaObjetoVPDPreviaEstorno(VPDEstorno vpdPreviaEstorno, DateTime now)
        {
            var documentoVPDOriginal = documentoDA.ConsultarTodos().FirstOrDefault(el => el.DocumentoID == vpdPreviaEstorno.DocumentoID);

            var documento = new Documento();
            documento.DocumentoOriginalID = documentoVPDOriginal.DocumentoID;
            documento.UnidadeGestoraCodigoEmitente = documentoVPDOriginal.UnidadeGestoraCodigoEmitente;
            documento.UnidadeGestoraDataInicioVigenciaEmitente = documentoVPDOriginal.UnidadeGestoraDataInicioVigenciaEmitente;
            documento.CredorIdentificacao = documentoVPDOriginal.CredorIdentificacao;
            documento.DataContabilizacao = vpdPreviaEstorno.DataEmissao;
            documento.DataEmissao = vpdPreviaEstorno.DataEmissao;
            documento.DataInclusaoRegistro = now;
            documento.Situacao = "A";
            documento.Estorno = "S";
            documento.ValorEstorno = vpdPreviaEstorno.TotalEstorno;
            documento.DominioIDGrupoDocumentoContabil = dominioDA.RetornaDominioID(DominioCategoriaEnum.GRUPODOCUMENTOCONTABIL, "VPD");
            documento.UsuarioInclusaoRegistro = UsuarioLogado.GetFullName();
            documento.ValorDocumento = vpdPreviaEstorno.TotalEstorno;
            documento.TipoDocumentoContabilID = documentoVPDOriginal.TipoDocumentoContabilID;
            documento.DominioIDGrupoDocumentoContabil = documentoVPDOriginal.DominioIDGrupoDocumentoContabil;
            documento.Observacao = vpdPreviaEstorno.Justificativa;
            documento.Justificativa = vpdPreviaEstorno.Justificativa;

            documento.ItemVariacaoPatrimonialDiminutiva = new List<ItemVariacaoPatrimonialDiminutiva>();

            if (vpdPreviaEstorno.TotalEstorno <= 0)
                throw new CFP.Util.SPFException("Informe o valor a ser estornado.");

            foreach (var i in vpdPreviaEstorno.ListaVPDEstorno.Where(el => el.ValorEstorno > 0))
            {
                var itemVPDOriginal = documentoVPDOriginal.ItemVariacaoPatrimonialDiminutiva.FirstOrDefault(el => el.VariacaoPatrimonialDiminutivaItemID == i.ItemVariacaoPatrimonialDiminutivaID);

                if (i.ValorEstorno > itemVariacaoPatrimonialDiminutivaDA.RetornaValorAEmpenhar(i.ItemVariacaoPatrimonialDiminutivaID))
                    throw new CFP.Util.SPFException("Valor informado no estorno é superior ao valor disponível.");

                var itemVPD = new ItemVariacaoPatrimonialDiminutiva();
                itemVPD.TipoVPD = "A";
                itemVPD.ValorSolicitado = 0;

                itemVPD.ValorEstorno = i.ValorEstorno;
                itemVPD.NaturezaDespesaID = itemVPDOriginal.NaturezaDespesaID;
                itemVPD.FonteRecursoID = itemVPDOriginal.FonteRecursoID;
                itemVPD.IdentificadorUsoCodigo = itemVPDOriginal.IdentificadorUsoCodigo;
                documento.ItemVariacaoPatrimonialDiminutiva.Add(itemVPD);
            }

            return documento;
        }

        private IEnumerable<SelectListItem> RetornaListaDocumentoEvento(int eventoID, int dominioIDMomento)
        {
            var evento = eventoDA.ConsultarTodos().FirstOrDefault(el => el.EventoID == eventoID);

            return evento.EventoTipoDocumentoPermitido.Where(el => el.TipoDocumentoContabil.DominioIDMomento == dominioIDMomento).Select(el => new Select2
            {
                text = el.Evento.Codigo + " - " + el.TipoDocumentoContabil.Titulo,
                id = el.EventoID + "_" + el.TipoDocumentoContabilID
            }).AsEnumerable().ToSelectList(el => el.id, el => el.text).ToList();
        }
        #endregion
    }
}