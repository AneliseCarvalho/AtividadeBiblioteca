using System.Diagnostics;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ultimosLivros = await _context.Livros
                .Include(l => l.Genero)
                .OrderByDescending(l => l.LivroId)
                .Take(4)
                .ToListAsync();

            var top5LivrosIds = await _context.Reservas
                .GroupBy(r => r.LivroId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToListAsync();

            var livrosMaisReservados = await _context.Livros
                .Include(l => l.Genero)
                .Where(l => top5LivrosIds.Contains(l.LivroId))
                .ToListAsync();

            // Ordena conforme o ranking do top5
            livrosMaisReservados = top5LivrosIds
                .Select(id => livrosMaisReservados.First(l => l.LivroId == id))
                .ToList();

            // Avaliações para todos os livros
            var avaliacoes = await _context.Avaliacoes
                .GroupBy(a => a.LivroId)
                .Select(g => new { LivroId = g.Key, Media = g.Average(a => a.Nota), Quantidade = g.Count() })
                .ToListAsync();

            ViewBag.UltimosLivros = ultimosLivros;
            ViewBag.LivrosMaisReservados = livrosMaisReservados;
            ViewBag.AvaliacoesLivro = avaliacoes.ToDictionary(a => a.LivroId, a => new { a.Media, a.Quantidade });

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}