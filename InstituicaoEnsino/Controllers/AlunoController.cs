using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using InstituicaoEnsino.Models;

namespace InstituicaoEnsino.Controllers
{
    public class AlunoController : Controller
    {
        private readonly InstituicaoEnsinoDBContext _context;
        private readonly IConfiguration _configuration;

        public AlunoController(InstituicaoEnsinoDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Aluno
        public async Task<IActionResult> Index(int idProfessor)
        {
            var prof = await _context.Professor.FirstOrDefaultAsync(p => p.IdProfessor == idProfessor);
            if (prof != null)
            {
                ViewData["IdProfessor"] = idProfessor;
                ViewData["NomeProfessor"] = prof.Nome;


                var instituicaoEnsinoDBContext = _context.Aluno.Include(a => a.IdProfessorNavigation);
                return View(await instituicaoEnsinoDBContext
                    .Where(a => a.IdProfessor == idProfessor)
                    .ToListAsync());
            }

            return RedirectToAction("Index", "Professor");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAluno(int id)
        {
            var aluno = await _context.Aluno
                .FirstOrDefaultAsync(m => m.IdAluno == id);
            if (aluno == null)
            {
                return NotFound();
            }

            _context.Aluno.Remove(aluno);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> UploadListaAlunos(IFormFile file, [FromQuery(Name = "idProfessor")] int idProfessor)
        {
            if (file != null)
            {
                Models.Professor professor = await _context.Professor.FirstOrDefaultAsync(p => p.IdProfessor == idProfessor);

                // Bloquear importação de alunos por um período de tempo definido em appsettings.json
                string strPeriodo = _configuration.GetValue<string>("Config:PeriodoBloqueioImportacao");
                TimeSpan periodo = TimeSpan.Zero;
                if (!string.IsNullOrEmpty(strPeriodo) && TimeSpan.TryParse(strPeriodo, out periodo))
                {
                    if (professor.DataUltimaImportacao.HasValue)
                    {
                        if (DateTime.Now - professor.DataUltimaImportacao.Value < periodo)
                        {
                            TimeSpan diff = periodo - (DateTime.Now - professor.DataUltimaImportacao.Value);
                            string dias = diff.Days > 0 ? diff.Days.ToString() + " dias," : "";
                            TempData["Error"] = $"Deve-se aguardar {dias} {diff.ToString("hh\\:mm\\:ss")} antes de se poder importar alunos para este professor.";
                            return RedirectToAction("Index", new { idProfessor = idProfessor }); ;
                        }
                    }
                }

                // Formato: NomeAluno||ValorMensalidade||DataVencimento
                using (StreamReader sr = new StreamReader(file.OpenReadStream()))
                {
                    string msg = null;
                    string error = null;
                    int inseridos = 0;

                    string[] lines = sr.ReadToEnd().Split("\r\n")
                        .Where(l => l.Length > 0)
                        .ToArray();

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] lineSplit = lines[i].Split("||");

                        if (lineSplit.Length != 3)
                        {
                            error = $"Registro inválido na linha {i + 1} do arquivo. Formato: NomeAluno||ValorMensalidade||DataVencimento.";
                            break;
                        }

                        string nomeAluno = lineSplit[0];
                        string valorMensal = lineSplit[1];
                        string dataVenc = lineSplit[2];

                        if (string.IsNullOrEmpty(nomeAluno)
                           || string.IsNullOrEmpty(valorMensal)
                           || string.IsNullOrEmpty(dataVenc))
                        {
                            error = $"Registro inválido na linha {i + 1} do arquivo. Formato: NomeAluno||ValorMensalidade||DataVencimento.";
                            break;
                        }

                        // Mensalidade
                        decimal mensalidade = 0.0m;
                        if (!decimal.TryParse(valorMensal, out mensalidade))
                        {
                            error = $"Valor mensal inválido na linha {i + 1} do arquivo -- formato: 0.00.";
                            break;
                        }

                        // Data de vencimento
                        DateTime dataVencimento = DateTime.MinValue;
                        if (!DateTime.TryParseExact(dataVenc, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture
                            , System.Globalization.DateTimeStyles.None, out dataVencimento))
                        {
                            error = $"Data de vencimento inválida na linha {i + 1} do arquivo -- formato: DD/MM/AAAA.";
                            break;
                        }

                        // Inserir Aluno
                        Models.Aluno aluno = new Aluno()
                        {
                            IdProfessor = idProfessor,
                            Nome = nomeAluno,
                            Mensalidade = mensalidade,
                            DataVencimento = dataVencimento
                        };
                        _context.Add(aluno);
                        await _context.SaveChangesAsync();
                        inseridos++;

                        // Salvar data de importação
                        professor.DataUltimaImportacao = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    if (error == null && inseridos > 0)
                        msg = $"{inseridos} registros inseridos.";

                    TempData["Error"] = error;
                    TempData["Message"] = msg;
                }
            }

            return RedirectToAction("Index", new { idProfessor = idProfessor });
        }
    }
}
