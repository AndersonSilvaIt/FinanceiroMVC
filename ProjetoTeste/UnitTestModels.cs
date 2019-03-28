using System;
using Xunit;
using MyFinance.Models;

namespace ProjetoTeste
{
    public class UnitTestModels
    {
        [Fact]
        public void TesteLoginUsuario()
        {
            UsuarioModel usuarioModel = new UsuarioModel();
            usuarioModel.Email = "teste@gmail.com";
            usuarioModel.Senha = "123456";

            Assert.True(usuarioModel.ValidarLogin());
        }

        [Fact]
        public void TesteRegistrarUsuario()
        {
            UsuarioModel usuarioModel = new UsuarioModel();
            usuarioModel.Nome = "Teste";
            usuarioModel.Data_Nascimento = "1994/12/30";
            usuarioModel.Email = "usuario@gmail.com";
            usuarioModel.Senha = "123456";
            usuarioModel.RegistrarUsuario();

            Assert.True(usuarioModel.ValidarLogin());
        }


    }
}
