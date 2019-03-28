using Microsoft.AspNetCore.Http;
using MyFinance.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MyFinance.Models
{
    public class TransacaoModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a Data!")]
        public string Data { get; set; }

        public string DataFinal { get; set; }
        public string Tipo { get; set; }

        public double Valor { get; set; }

        [Required(ErrorMessage = "Informe a descrição!")]
        public string Descricao { get; set; }

        public int Conta_Id { get; set; }
        public int Plano_Conta_Id { get; set; }

        public string NomeConta { get; set; }
        public string DescricaoPlanoConta { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public TransacaoModel()
        { }

        public TransacaoModel(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public List<TransacaoModel> ListaTransacao()
        {
            List<TransacaoModel> lista = new List<TransacaoModel>();
            TransacaoModel item;

            //Utilizado pela view Extrato

            string filtro = "";
            if (Data != null && DataFinal != null)
                filtro += $" and t.Data >='{DateTime.Parse(Data).ToString("yyyy/MM/dd")}' and t.Data <= '{DateTime.Parse(DataFinal).ToString("yyyy/MM/dd")}' ";

            if (Tipo != null && Tipo != "A")
                filtro += $" and t.tipo = '{Tipo}' ";

            if (Conta_Id != 0)
                filtro += $" and t.Conta_Id = '{Conta_Id}' ";

            //Fim
            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");
            string sql = $@"select t.id, t.data, t.tipo, t.valor, t.descricao as historico, t.conta_id, c.Nome as conta,  t.plano_contas_id, p.descricao as plano_conta
                        from transacao as t
                        inner join conta c
                        on t.conta_id = c.id inner join Plano_Contas as p
                        on t.plano_contas_id = p.id
                        where t.Usuario_Id = {id_usuario_logado} {filtro} order by t.data desc limit 10";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = new TransacaoModel();
                item.Id = int.Parse(dt.Rows[i][0].ToString());
                item.Data = DateTime.Parse(dt.Rows[i][1].ToString()).ToString("dd/MM/yyyy");
                item.Tipo = dt.Rows[i][2].ToString();
                item.Valor = double.Parse(dt.Rows[i][3].ToString());
                item.Descricao = dt.Rows[i][4].ToString();
                item.Conta_Id = int.Parse(dt.Rows[i][5].ToString());
                item.NomeConta = dt.Rows[i][6].ToString();
                item.Plano_Conta_Id = int.Parse(dt.Rows[i][7].ToString());
                item.DescricaoPlanoConta = dt.Rows[i][8].ToString();

                lista.Add(item);
            }

            return lista;
        }

        public void Excluir(int id)
        {
            new DAL().ExecutarComandoSQL($"DELETE FROM TRANSACAO WHERE ID = {id}");
        }

        public void Insert()
        {
            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");
            string sql;

            if (Id == 0)
                sql = $@"INSERT INTO TRANSACAO (DATA, TIPO, DESCRICAO, VALOR, CONTA_ID, PLANO_CONTAS_ID, USUARIO_ID) 
                                    VALUES ('{DateTime.Parse(Data).ToString("yyyy/MM/dd")}', '{Tipo}', '{Descricao}', '{Valor}', '{Conta_Id}', '{Plano_Conta_Id}', '{id_usuario_logado}' )";
            else
                sql = $@"UPDATE TRANSACAO SET DATA = '{DateTime.Parse(Data).ToString("yyyy/MM/dd")}',
                                              TIPO = '{Tipo}', 
                                              DESCRICAO = '{Descricao}', 
                                              VALOR = '{Valor}',
                                              CONTA_ID = {Conta_Id}, 
                                              PLANO_CONTAS_ID = {Plano_Conta_Id},
                                              USUARIO_ID = '{id_usuario_logado}' 
                                              WHERE ID = {Id}";

            DAL objDAL = new DAL();
            objDAL.ExecutarComandoSQL(sql);
        }

        public TransacaoModel CarregarRegistro(int? id)
        {
            TransacaoModel item;

            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");
            string sql = $@"select t.id, t.data, t.tipo, t.valor, t.descricao as historico, t.conta_id, c.Nome as conta,  t.plano_contas_id, p.descricao as plano_conta
                        from transacao as t
                        inner join conta c
                        on t.conta_id = c.id inner join Plano_Contas as p
                        on t.plano_contas_id = p.id
                        where t.Usuario_Id = {id_usuario_logado} and t.ID = {id}";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            item = new TransacaoModel();
            item.Id = int.Parse(dt.Rows[0][0].ToString());
            item.Data = DateTime.Parse(dt.Rows[0][1].ToString()).ToString("dd/MM/yyyy");
            item.Tipo = dt.Rows[0][2].ToString();
            item.Valor = double.Parse(dt.Rows[0][3].ToString());
            item.Descricao = dt.Rows[0][4].ToString();
            item.Conta_Id = int.Parse(dt.Rows[0][5].ToString());
            item.NomeConta = dt.Rows[0][6].ToString();
            item.Plano_Conta_Id = int.Parse(dt.Rows[0][7].ToString());
            item.DescricaoPlanoConta = dt.Rows[0][8].ToString();

            return item;
        }

    }

    public class DashBoard
    {
        public double Total { get; set; }
        public string PlanoConta { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public DashBoard()
        { }

        public DashBoard(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public List<DashBoard> RetornarDadosGraficoPie(TransacaoModel formulario)
        {
            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");

            List<DashBoard> lista = new List<DashBoard>();
            DashBoard item;
            string filtro = "";
            string sql = $@"select p.Descricao, sum(t.valor) as total from transacao as t
                            inner join plano_contas as p
                            on t.Plano_Contas_Id = p.Id where t.tipo = '{formulario.Tipo}' and t.usuario_id = {id_usuario_logado} 
                            {filtro}
                            group by p.Descricao";

            if (formulario.Conta_Id > 0)
                filtro += $"and t.Conta_Id = {formulario.Plano_Conta_Id} ";

            if (formulario.Data != null && formulario.DataFinal != null)
                filtro += $" and t.Data >='{DateTime.Parse(formulario.Data).ToString("yyyy/MM/dd")}' and t.Data <= '{DateTime.Parse(formulario.DataFinal).ToString("yyyy/MM/dd")}' ";

            DAL objDAL = new DAL();
            DataTable dt = new DataTable();
            dt = objDAL.RetDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = new DashBoard();
                item.Total = double.Parse(dt.Rows[i]["total"].ToString());
                item.PlanoConta = dt.Rows[i]["Descricao"].ToString();
                lista.Add(item);
            }

            return lista;
        }
    }
}
