using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;
using System.Security.Claims;

namespace Biblioteca.Controllers
{
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca o usuário logado
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdentityUser != null && u.IdentityUser.Id == userId);

            if (usuario == null)
                return Unauthorized();

            //// Busca os livros que o usuário já retirou (todas as movimentações do usuário)
            //var livrosRetiradosIds = await _context.Movimentacoes
            //    .Where(m => m.UsuarioId == usuario.UsuarioId)
            //    .Select(m => m.LivroId)
            //    .Distinct()
            //    .ToListAsync();

            // Filtra as avaliações apenas desses livros e do usuário logado
            var avaliacoes = await _context.Avaliacoes
                .Include(a => a.Livro)
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuario.UsuarioId)
                .ToListAsync();

            return View(avaliacoes);
        }

        // GET: Avaliacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacoes
                .Include(a => a.Livro)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        // GET: Avaliacoes/Create
        public IActionResult Create(int id)
        {
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: Avaliacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AvaliacaoId,Nota,Comentario,DataAvaliacao,LivroId,UsuarioId")] Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId", avaliacao.LivroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", avaliacao.UsuarioId);
            return View(avaliacao);
        }

        // GET: Avaliacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacoes.FindAsync(id);
            if (avaliacao == null)
            {
                return NotFound();
            }
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId", avaliacao.LivroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", avaliacao.UsuarioId);
            return View(avaliacao);
        }

        // POST: Avaliacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoId,Nota,Comentario,DataAvaliacao,LivroId,UsuarioId")] Avaliacao avaliacao)
        {
            if (id != avaliacao.AvaliacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvaliacaoExists(avaliacao.AvaliacaoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivroId"] = new SelectList(_context.Livros, "LivroId", "LivroId", avaliacao.LivroId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", avaliacao.UsuarioId);
            return View(avaliacao);
        }

        // GET: Avaliacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacao = await _context.Avaliacoes
                .Include(a => a.Livro)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.AvaliacaoId == id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            return View(avaliacao);
        }

        // POST: Avaliacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacao = await _context.Avaliacoes.FindAsync(id);
            if (avaliacao != null)
            {
                _context.Avaliacoes.Remove(avaliacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoExists(int id)
        {
            return _context.Avaliacoes.Any(e => e.AvaliacaoId == id);
        }
    }
}
