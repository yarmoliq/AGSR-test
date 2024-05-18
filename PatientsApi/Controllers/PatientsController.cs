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
        public async Task<ActionResult<IEnumerable<Patient>>> SearchByBirthDate(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] DateTime? exactDate = null)
        {
            if (exactDate.HasValue)
            {
                return await _context.Patients
                    .Include(p => p.Name)
                    .Where(p => p.BirthDate.Date == exactDate.Value.Date)
                    .ToListAsync();
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                return await _context.Patients
                    .Include(p => p.Name)
                    .Where(p => p.BirthDate.Date >= fromDate.Value.Date && p.BirthDate.Date <= toDate.Value.Date)
                    .ToListAsync();
            }

            if (fromDate.HasValue)
            {
                return await _context.Patients
                    .Include(p => p.Name)
                    .Where(p => p.BirthDate.Date >= fromDate.Value.Date)
                    .ToListAsync();
            }

            if (toDate.HasValue)
            {
                return await _context.Patients
                    .Include(p => p.Name)
                    .Where(p => p.BirthDate.Date <= toDate.Value.Date)
                    .ToListAsync();
            }

            return BadRequest("Please provide at least one query parameter: fromDate, toDate, or exactDate.");
        }

        private bool PatientExists(Guid id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
