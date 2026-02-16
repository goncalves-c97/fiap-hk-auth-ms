using Core.Helpers;
using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class Cliente : ValidatorClass
{
    public int IdUsuario { get; set; }

    public string Usuario { get; set; }

    public string Email { get; set; }

    public string Senha{ get; set; }

    public Guid Guid { get; set; } = Guid.NewGuid();

    public Cliente() { }

    public Cliente(string usuario, string email, string senha)
    {
        Usuario = usuario;
        Email = email;
        Senha = senha;
        ValidateValueObjects();
    }

    public void ValidateValueObjects()
    {
        Validate();
    }

    protected override void Validate()
    {
        NotEmptyStringValidation(nameof(Usuario), Usuario);
        NotEmptyStringValidation(nameof(Email), Email);
        NotEmptyStringValidation(nameof(Senha), Senha);
    }
}
