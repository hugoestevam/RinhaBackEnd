using System.Data;
using Npgsql;
using WebApp.Pessoas.API.Model;

namespace WebApp.Pessoas.API.Database
{
    public sealed class Db
    {
        private NpgsqlDataSource _dados; 

        public Db(NpgsqlDataSource dados)
        {
            _dados = dados;
        }

        internal async Task<bool> Existe(string? apelido)
        {
            await using var cmd = _dados.CreateCommand(@"
                select exists(select from pessoas
                where apelido = $1)");

            cmd.Parameters.AddWithValue(apelido);
            
            var leitor = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return leitor as bool? ?? false;
        }

        internal async Task<Pessoa?> BuscarPessoaId(Guid id)
        {
            await using var cmd = _dados.CreateCommand(@"
                select * from pessoas
                where id = $1");
            
            cmd.Parameters.AddWithValue(id);
            
            await using var leitor = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            if (!leitor.HasRows)
                return null;

            await leitor.ReadAsync().ConfigureAwait(false);
            
            var pessoa = await MontaPessoa(leitor);
            return pessoa;
        }

        internal async IAsyncEnumerable<Pessoa> BuscarPessoas(string t)
        {
            await using var cmd = _dados.CreateCommand(@"
                select id, apelido, nome, nascimento, stack 
                from pessoas
                where 
                    apelido like @t
                    or nome like @t
                    or stack @> ARRAY[@a]
                limit 50;
            ");

            cmd.Parameters.AddWithValue("@t", "%" + t + "%");
            cmd.Parameters.AddWithValue("@a", t);

            await using var leitor = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false);

            if(!leitor.HasRows)
                yield break;

            while (await leitor.ReadAsync().ConfigureAwait(false))
                yield return await MontaPessoa(leitor);
        }

        static async Task<Pessoa> MontaPessoa(NpgsqlDataReader leitor) => 
            new Pessoa {
                        Id = leitor.GetGuid(0),
                        Apelido = leitor.GetString(1),
                        Nome = leitor.GetString(2),
                        Nascimento = DateOnly.FromDateTime(leitor.GetDateTime(3)),
                        Stack = await leitor.IsDBNullAsync(4).ConfigureAwait(false) 
                                ? null 
                                : await leitor.GetFieldValueAsync<object>(4) as string[],
            };

        internal async Task<long> ContarPessoas()
        {
            await using var cmd = _dados.CreateCommand(@"
                select count(1) from pessoas;
            ");
            
            var contador = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return contador as long? ?? 0L;
        }

        internal async Task<bool> InserirPessoa(Pessoa pessoa)
        {
            await using var cmd = _dados.CreateCommand(@"
                insert into pessoas
                (id, apelido, nome, nascimento, stack)
                values ($1, $2, $3, $4, $5)
                on conflict do nothing;
            ");

            cmd.Parameters.AddWithValue(pessoa.Id);
            cmd.Parameters.AddWithValue(pessoa.Apelido);
            cmd.Parameters.AddWithValue(pessoa.Nome);
            cmd.Parameters.AddWithValue(pessoa.Nascimento);
            cmd.Parameters.AddWithValue(pessoa.Stack == null ? DBNull.Value : pessoa.Stack);
            
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            return true;
        }
    }
}