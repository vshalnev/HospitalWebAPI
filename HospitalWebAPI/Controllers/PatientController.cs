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
            var patients = await GetSortPatients<int>();

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

        [HttpGet("{pageSize}/{sortName}")]
        public async Task<ActionResult<List<List<PatientP>>>> Get(int pageSize, PatientSort sortName)
        {
            List<PatientP>? patients = new List<PatientP>();
            switch (sortName)
            {
                case PatientSort.Id:
                    patients = await GetSortPatients<int>(r => r.Id);
                    break;
                case PatientSort.LastName:
                    patients = await GetSortPatients<string>(r => r.LastName);
                    break;
                case PatientSort.FirstName:
                    patients = await GetSortPatients<string>(r => r.FirstName);
                    break;
                case PatientSort.SecondName:
                    patients = await GetSortPatients<string>(r => r.SecondName);
                    break;
                case PatientSort.Address:
                    patients = await GetSortPatients<string>(r => r.Address);
                    break;
                case PatientSort.Birthday:
                    patients = await GetSortPatients<string>(r => r.Birthday);
                    break;
                case PatientSort.Gender:
                    patients = await GetSortPatients<string>(r => r.Gender);
                    break;
                case PatientSort.Area:
                    patients = await GetSortPatients<string>(r => r.Area);
                    break;
                default:
                    patients = await GetSortPatients<int>(r => r.Id);
                    break;
            }

            List<PatientP> patientsList = new();
            List<List<PatientP>> patientsPages = new();
            int nowPageSize = 0;
            double patientsPagesCount = 0;
            foreach (var patient in patients)
            {
                if (nowPageSize < pageSize)
                {
                    patientsList.Add(patient);
                    nowPageSize++;
                }
                else
                {
                    nowPageSize = 0;
                    patientsPages.Add(patientsList);
                    patientsPagesCount++;
                    patientsList = new();

                    patientsList.Add(patient);
                    nowPageSize++;
                }
            }

            if (patientsPagesCount < ((double)patients.Count / (double)pageSize)) patientsPages.Add(patientsList);

            return Ok(patientsPages);
        }

        [HttpPost]
        public async Task<ActionResult<PatientP>> AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var patients = await GetSortPatients<int>();

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

            var patients = await GetSortPatients<int>();

            return Ok(patients);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<PatientP>> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return BadRequest("Doctor not found!");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            var patients = await GetSortPatients<int>();

            return Ok(patients);
        }

        private async Task<List<PatientP>> GetSortPatients<TKey>(Func<PatientP, TKey>? orderBy = null)
        {
            _returnPatients = new();
            var patients = await _context.Patients.ToListAsync();
            var areas = await _context.Areas.ToListAsync();

            foreach (var patient in patients)
            {
                _returnPatients.Add(ConverterToPatientPAsync(patient, areas));
            }

            var result = await Task.WhenAll(_returnPatients);

            List<PatientP>? sortList;
            if (orderBy == null) sortList = result.OrderBy(r => r.Id).ToList();
            else sortList = result.OrderBy(orderBy).ToList();

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

