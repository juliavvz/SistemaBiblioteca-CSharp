
using System;
using System.Collections.Generic;
using System.Linq;

class Emprestimo
{
    public DateTime DtEmprestimo { get; set; } = DateTime.Now;
    public DateTime? DtDevolucao { get; set; }
}

class Exemplar
{
    public int Tombo { get; set; }
    public List<Emprestimo> Emprestimos { get; set; } = new List<Emprestimo>();

    public bool Disponivel()
    {
        // Está disponível se não tem empréstimos ou se o último empréstimo já foi devolvido
        if (Emprestimos.Count == 0) return true;
        return Emprestimos.Last().DtDevolucao != null;
    }

    public bool Emprestar()
    {
        if (!Disponivel()) return false;
        Emprestimos.Add(new Emprestimo());
        return true;
    }

    public bool Devolver()
    {
        if (Disponivel()) return false;
        Emprestimos.Last().DtDevolucao = DateTime.Now;
        return true;
    }

    public int QtdeEmprestimos() => Emprestimos.Count;
}

class Livro
{
    public int Isbn { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string Editora { get; set; }
    public List<Exemplar> Exemplares { get; set; } = new List<Exemplar>();

    public void AdicionarExemplar(Exemplar exemplar) => Exemplares.Add(exemplar);
    public int QtdeExemplares() => Exemplares.Count;
    public int QtdeDisponiveis() => Exemplares.Count(e => e.Disponivel());
    public int QtdeEmprestimos() => Exemplares.Sum(e => e.QtdeEmprestimos());

    public double PercDisponibilidade()
    {
        int total = QtdeExemplares();
        if (total == 0) return 0;
        return ((double)QtdeDisponiveis() / total) * 100;
    }
}

class Livros
{
    public List<Livro> Acervo { get; set; } = new List<Livro>();

    public void Adicionar(Livro livro) => Acervo.Add(livro);

    public Livro Pesquisar(int isbn) => Acervo.FirstOrDefault(l => l.Isbn == isbn);
}

class Program
{
    static void Main()
    {
        Livros biblioteca = new Livros();
        int opcao;

        do
        {
            Console.WriteLine("\n--- MENU BIBLIOTECA ---");
            Console.WriteLine("0. Sair");
            Console.WriteLine("1. Adicionar livro");
            Console.WriteLine("2. Pesquisar livro (sintético)");
            Console.WriteLine("3. Pesquisar livro (analítico)");
            Console.WriteLine("4. Adicionar exemplar");
            Console.WriteLine("5. Registrar empréstimo");
            Console.WriteLine("6. Registrar devolução");
            Console.Write("Escolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao)) opcao = -1;

            switch (opcao)
            {
                case 0:
                    Console.WriteLine("Saindo...");
                    break;
                case 1:
                    Console.Write("ISBN: ");
                    int isbn = int.Parse(Console.ReadLine());
                    Console.Write("Título: ");
                    string titulo = Console.ReadLine();
                    Console.Write("Autor: ");
                    string autor = Console.ReadLine();
                    Console.Write("Editora: ");
                    string editora = Console.ReadLine();

                    biblioteca.Adicionar(new Livro { Isbn = isbn, Titulo = titulo, Autor = autor, Editora = editora });
                    Console.WriteLine("Livro adicionado com sucesso!");
                    break;
                case 2:
                    Console.Write("ISBN do livro: ");
                    var livroSintetico = biblioteca.Pesquisar(int.Parse(Console.ReadLine()));
                    if (livroSintetico != null)
                    {
                        Console.WriteLine($"\n[Sintético] {livroSintetico.Titulo} ({livroSintetico.Autor})");
                        Console.WriteLine($"Total de Exemplares: {livroSintetico.QtdeExemplares()}");
                        Console.WriteLine($"Disponíveis: {livroSintetico.QtdeDisponiveis()}");
                        Console.WriteLine($"Empréstimos Totais: {livroSintetico.QtdeEmprestimos()}");
                        Console.WriteLine($"Disponibilidade: {livroSintetico.PercDisponibilidade():F2}%");
                    }
                    else Console.WriteLine("Livro não encontrado.");
                    break;
                case 3:
                    Console.Write("ISBN do livro: ");
                    var livroAnalitico = biblioteca.Pesquisar(int.Parse(Console.ReadLine()));
                    if (livroAnalitico != null)
                    {
                        Console.WriteLine($"\n[Analítico] {livroAnalitico.Titulo} - {livroAnalitico.Editora}");
                        Console.WriteLine($"Exemplares: {livroAnalitico.QtdeExemplares()} | Disponíveis: {livroAnalitico.QtdeDisponiveis()} | % Disp: {livroAnalitico.PercDisponibilidade():F2}%");
                        foreach (var ex in livroAnalitico.Exemplares)
                        {
                            Console.WriteLine($"  -> Tombo: {ex.Tombo} | Disponível: {(ex.Disponivel() ? "Sim" : "Não")} | Empréstimos: {ex.QtdeEmprestimos()}");
                            foreach (var emp in ex.Emprestimos)
                            {
                                string dev = emp.DtDevolucao.HasValue ? emp.DtDevolucao.Value.ToString("dd/MM/yyyy HH:mm") : "Pendente";
                                Console.WriteLine($"     - Empréstimo em: {emp.DtEmprestimo:dd/MM/yyyy HH:mm} | Devolução: {dev}");
                            }
                        }
                    }
                    else Console.WriteLine("Livro não encontrado.");
                    break;
                case 4:
                    Console.Write("ISBN do livro: ");
                    var lExemplar = biblioteca.Pesquisar(int.Parse(Console.ReadLine()));
                    if (lExemplar != null)
                    {
                        Console.Write("Número do Tombo do exemplar: ");
                        int tombo = int.Parse(Console.ReadLine());
                        lExemplar.AdicionarExemplar(new Exemplar { Tombo = tombo });
                        Console.WriteLine("Exemplar adicionado com sucesso!");
                    }
                    else Console.WriteLine("Livro não encontrado.");
                    break;
                case 5:
                    Console.Write("ISBN do livro: ");
                    var lEmp = biblioteca.Pesquisar(int.Parse(Console.ReadLine()));
                    if (lEmp != null)
                    {
                        var exemplarDisp = lEmp.Exemplares.FirstOrDefault(e => e.Disponivel());
                        if (exemplarDisp != null)
                        {
                            exemplarDisp.Emprestar();
                            Console.WriteLine($"Empréstimo registrado com sucesso para o Tombo: {exemplarDisp.Tombo}");
                        }
                        else Console.WriteLine("Não há exemplares disponíveis para empréstimo.");
                    }
                    else Console.WriteLine("Livro não encontrado.");
                    break;
                case 6:
                    Console.Write("ISBN do livro: ");
                    var lDev = biblioteca.Pesquisar(int.Parse(Console.ReadLine()));
                    if (lDev != null)
                    {
                        Console.Write("Número do Tombo a devolver: ");
                        int tomboDev = int.Parse(Console.ReadLine());
                        var exemplarDev = lDev.Exemplares.FirstOrDefault(e => e.Tombo == tomboDev);
                        if (exemplarDev != null)
                        {
                            if (exemplarDev.Devolver())
                                Console.WriteLine("Devolução registrada com sucesso!");
                            else
                                Console.WriteLine("Este exemplar já consta como disponível.");
                        }
                        else Console.WriteLine("Exemplar não encontrado.");
                    }
                    else Console.WriteLine("Livro não encontrado.");
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        } while (opcao != 0);
    }
}

