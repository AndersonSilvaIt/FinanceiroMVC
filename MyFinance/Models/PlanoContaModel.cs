using Microsoft.AspNetCore.Http;
using MyFinance.Util;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MyFinance.Models
{
    public class PlanoContaModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe a descrição!")]
        public string Descricao { get; set; }
        public string Tipo { get; set; }

        public int Usuario_Id { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public PlanoContaModel()
        {}

        public PlanoContaModel(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        
        private string IdUsuarioLogado()
        {
            return HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");
        }

        public List<PlanoContaModel> ListaPlanoContas()
        {
            List<PlanoContaModel> lista = new List<PlanoContaModel>();
            PlanoContaModel item;
            
            string sql = $"select ID, DESCRICAO, TIPO, USUARIO_ID from plano_contas where USUARIO_ID = {IdUsuarioLogado()}";
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = new PlanoContaModel();
                item.Id = int.Parse(dt.Rows[i]["ID"].ToString());
                item.Descricao = dt.Rows[i]["Descricao"].ToString();
                item.Tipo = dt.Rows[i]["Tipo"].ToString();
                item.Usuario_Id = int.Parse(dt.Rows[i]["USUARIO_ID"].ToString());

                lista.Add(item);
            }

            return lista;
        }


        public void Insert()
        {
            string sql;

            if(Id == 0)
                sql = $"INSERT INTO PLANO_CONTAS (DESCRICAO, TIPO, USUARIO_ID) VALUES ('{Descricao}', '{Tipo}', {IdUsuarioLogado()} )";
            else
                sql = $"UPDATE PLANO_CONTAS SET DESCRICAO = '{Descricao}', TIPO = '{Tipo}' WHERE USUARIO_ID = {IdUsuarioLogado()} and ID = {Id}";

            DAL objDAL = new DAL();
            objDAL.ExecutarComandoSQL(sql);
        }

        public void Excluir(int id)
        {
            new DAL().ExecutarComandoSQL($"DELETE FROM PLANO_CONTA WHERE ID = {id}");
        }

        public PlanoContaModel CarregarRegistro(int? id)
        {
            PlanoContaModel item = new PlanoContaModel();
                
            string sql = $"select id, descricao, tipo, usuario_id from plano_contas where usuario_id = {IdUsuarioLogado()} and id = {id}";
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            item.Id = int.Parse(dt.Rows[0]["id"].ToString());
            item.Descricao = dt.Rows[0]["descricao"].ToString();
            item.Tipo = dt.Rows[0]["tipo"].ToString();
            item.Usuario_Id = int.Parse(dt.Rows[0]["usuario_id"].ToString());

            return item;
        }

    }
}
