using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HospitalWebAPI.Data;

namespace HospitalWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly DataContext _context;
        private List<Task<PatientP>> _returnPatients;

        public PatientController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientP>>> Get()
        {
            var patients = await GetSortPatients();

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            var areas = await _context.Areas.ToListAsync();

            if (patient == null) return BadRequest("Doctor not found!");

            return Ok(ConverterToPatientPAsync(patient, areas));
        }

        [HttpPost]
        public async Task<ActionResult<PatientP>> AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var patients = await GetSortPatients();

            return Ok(patients);
        }

        [HttpPut]
        public async Task<ActionResult<PatientP>> UpdatePatient(Patient request)
        {
            var patient = await _context.Patients.FindAsync(request.Id);
            if (patient == null) return BadRequest("Doctor not found!");

            patient.LastName = request.LastName;
            patient.FirstName = request.FirstName;
            patient.SecondName = request.SecondName;
            patient.Address = request.Address;
            patient.Birthday = request.Birthday;
            patient.Gender = request.Gender;
            patient.AreaId = request.AreaId;

            await _context.SaveChangesAsync();

            var patients = await GetSortPatients();

            return Ok(patients);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<PatientP>> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return BadRequest("Doctor not found!");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            var patients = await GetSortPatients();

            return Ok(patients);
        }

        private async Task<List<PatientP>> GetSortPatients()
        {
            _returnPatients = new();
            var patients = await _context.Patients.ToListAsync();
            var areas = await _context.Areas.ToListAsync();

            foreach (var patient in patients)
            {
                _returnPatients.Add(ConverterToPatientPAsync(patient, areas));
            }

            var result = await Task.WhenAll(_returnPatients);

            var sortList = result.OrderBy(r => r.Id).ToList();

            return sortList;
        }

        private async Task<PatientP> ConverterToPatientPAsync(Patient patient, List<Area> areas)
        {
            var patientP = new PatientP();
            patientP.Id = patient.Id;
            patientP.LastName = patient.LastName;
            patientP.FirstName = patient.FirstName;
            patientP.SecondName = patient.SecondName;
            patientP.Address = patient.Address;
            patientP.Birthday = patient.Birthday;
            if (patient.Gender == Gender.Male) patientP.Gender = "Мужской";
            if (patient.Gender == Gender.Female) patientP.Gender = "Женский";

            var area = areas.Find(a => a.Id == patient.AreaId);
            if (area != null) patientP.Area = area.Number;
            else patientP.Area = "Area not found!";

            return patientP;
        }
    }
}

