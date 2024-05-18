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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                
                throw;
            }

            return NoContent();
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.Id }, patient);
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

            return NoContent();
        }
        
        // GET: api/Patients/SearchByBirthDate
        [HttpGet("SearchByBirthDate")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchByBirthDate([FromQuery] string parameter)
        {
            if (string.IsNullOrEmpty(parameter) || parameter.Length < 3)
            {
                return BadRequest("Invalid parameter format. Expected format: [operator][date].");
            }

            // Extract the operator and the date
            var operatorPart = parameter.Substring(0, 2);
            var datePart = parameter.Substring(2);

            if (!DateTime.TryParse(datePart, out DateTime parsedDate))
            {
                return BadRequest("Invalid date format.");
            }

            // Apply filters based on the operator
            switch (operatorPart)
            {
                case "eq":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate == parsedDate)
                        .ToListAsync();

                case "ne":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate != parsedDate)
                        .ToListAsync();

                case "lt":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate < parsedDate)
                        .ToListAsync();

                case "gt":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate > parsedDate)
                        .ToListAsync();

                case "le":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate <= parsedDate)
                        .ToListAsync();

                case "ge":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate >= parsedDate)
                        .ToListAsync();

                case "sa":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate > parsedDate)
                        .ToListAsync();

                case "eb":
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate < parsedDate)
                        .ToListAsync();

                case "ap":
                    var lowerBound = parsedDate.AddDays(-1);
                    var upperBound = parsedDate.AddDays(1);
                    return await _context.Patients
                        .Include(p => p.Name)
                        .Where(p => p.BirthDate >= lowerBound && p.BirthDate <= upperBound)
                        .ToListAsync();

                default:
                    return BadRequest("Invalid operator. Supported operators: eq, ne, lt, gt, le, ge, sa, eb, ap.");
            }
        }

        private bool PatientExists(Guid id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
