using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientApi.Data;
using PatientApi.Models;

namespace PatientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly PatientContext _context;

        public PatientsController(PatientContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.Include(p => p.Name).ToListAsync();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(Guid id)
        {
            var patient = await _context.Patients
                .Include(p => p.Name)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(Guid id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }

            _context.Entry(patient).State = EntityState.Modified;
            _context.Entry(patient.Name).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Guid>> PostPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return patient.Id;
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        // GET: api/Patients/SearchByBirthDate
        [HttpGet("SearchByBirthDate")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchByBirthDate([FromQuery] string birthDate)
        {
            if (string.IsNullOrEmpty(birthDate) || birthDate.Length < 3)
            {
                return BadRequest("Invalid parameter format. Expected format: [operator][date].");
            }

            var operatorPart = birthDate.Substring(0, 2);
            var datePart = birthDate.Substring(2);

            if (!DateTime.TryParse(datePart, out DateTime parsedDate))
            {
                return BadRequest("Invalid date format.");
            }

            var patients = _context.Patients.Include(p => p.Name).AsQueryable();

            var operation = GetBirthDatePredicate(operatorPart, parsedDate);
            if (operation == null)
            {
                return BadRequest("Invalid operator. Supported operators: eq, ne, lt, gt, le, ge, sa, eb, ap.");
            }
            
            return await patients.Where(operation).ToListAsync();
        }
        
        private Expression<Func<Patient, bool>>? GetBirthDatePredicate(string condition, DateTime parsedDate)
        {
            return condition switch
            {
                "eq" => p => p.BirthDate == parsedDate,
                "ne" => p => p.BirthDate != parsedDate,
                "lt" => p => p.BirthDate < parsedDate,
                "gt" => p => p.BirthDate > parsedDate,
                "le" => p => p.BirthDate <= parsedDate,
                "ge" => p => p.BirthDate >= parsedDate,
                "sa" => p => p.BirthDate > parsedDate,
                "eb" => p => p.BirthDate < parsedDate,
                "ap" => p => p.BirthDate >= parsedDate.AddDays(-1) && p.BirthDate <= parsedDate.AddDays(1),
                _ => null
            };
        }

        private bool PatientExists(Guid id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
