using MyFinance.Util;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MyFinance.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o Nome!")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o E-mail!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "O email informado é inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a Senha!")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informe a Data de Nascimento!")]
        public string Data_Nascimento { get; set; }

        public bool ValidarLogin()
        {
            string sql = $"select id, nome, data_nascimento from usuario where email = '{Email}' and senha = '{Senha}'";
            DAL objDAL = new DAL();
            objDAL.RetDataTable(sql);
            DataTable dt = objDAL.RetDataTable(sql);

            if (dt != null)
            {
                if (dt.Rows.Count == 1)
                {
                    Id = int.Parse( dt.Rows[0]["Id"].ToString());
                    Nome = dt.Rows[0]["Nome"].ToString();
                    Data_Nascimento = dt.Rows[0]["Data_Nascimento"].ToString();
                    return true;
                }
            }

            return false;
        }

        public void RegistrarUsuario()
        {
            string dataNascimento = DateTime.Parse(Data_Nascimento).ToString("yyyy-MM-dd");
            string sql = $@"insert into usuario (nome, email, senha, data_nascimento) values 
                                                ('{Nome}', '{Email}', '{Senha}', '{dataNascimento}')";

            DAL obj = new DAL();
            obj.ExecutarComandoSQL(sql);
        }

    }
}
