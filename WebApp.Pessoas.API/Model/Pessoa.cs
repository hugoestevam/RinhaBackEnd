
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pessoas.API.Model
{
    public struct Pessoa
    {
        private Guid? _id;
        public Pessoa()
        {
        }

        public Guid? Id
        {
            get { return _id; }
            set => _id = value;
        }
        public string? Apelido { get; set; }
        public string? Nome { get; set; }
        public DateOnly? Nascimento { get; set; }
        public string[]? Stack { get; set; }

        internal bool Valida()
        {
            bool invalida = string.IsNullOrEmpty(Apelido) || Apelido.Length > 32
                     || string.IsNullOrEmpty(Nome) || Nome.Length > 100
                     || !Nascimento.HasValue;

            if (invalida)
                return false;

            foreach (var item in Stack ?? Enumerable.Empty<string>())
                if (item.Length > 32 || item.Length == 0)
                    return false;

            Id ??= Guid.NewGuid();

            return true;
        } 
    }
}
