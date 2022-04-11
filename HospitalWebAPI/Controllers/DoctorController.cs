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
    public class DoctorController : ControllerBase
    {
        private readonly DataContext _context;
        private List<Task<DoctorP>> _returnDoctors;

        public DoctorController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorP>>> Get()
        {
            var doctors = await GetSortDoctors();

            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorP>> Get(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            var cabinets = await _context.Cabinets.ToListAsync();
            var specializations = await _context.Specializations.ToListAsync();
            var areas = await _context.Areas.ToListAsync();

            if (doctor == null) return BadRequest("Doctor not found!");

            return Ok(await ConverterToDoctorPAsync(doctor, cabinets, specializations, areas));
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> AddDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var doctors = await GetSortDoctors();

            return Ok(doctors);
        }

        [HttpPut]
        public async Task<ActionResult<Doctor>> UpdateDoctor(Doctor request)
        {
            var doctor = await _context.Doctors.FindAsync(request.Id);
            if (doctor == null) return BadRequest("Doctor not found!");

            doctor.FIO = request.FIO;
            doctor.CabinetId = request.CabinetId;
            doctor.SpecializationId = request.SpecializationId;
            doctor.AreaId = request.AreaId;

            await _context.SaveChangesAsync();

            var doctors = await GetSortDoctors();

            return Ok(doctors);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Doctor>> Delete(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return BadRequest("Doctor not found!");

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            var doctors = await GetSortDoctors();

            return Ok(doctors);
        }

        private async Task<List<DoctorP>> GetSortDoctors()
        {
            _returnDoctors = new();
            var doctors = await _context.Doctors.ToListAsync();
            var cabinets = await _context.Cabinets.ToListAsync();
            var specializations = await _context.Specializations.ToListAsync();
            var areas = await _context.Areas.ToListAsync();

            foreach (var doctor in doctors)
            {
                _returnDoctors.Add(ConverterToDoctorPAsync(doctor, cabinets, specializations, areas));
            }

            var result = await Task.WhenAll(_returnDoctors);

            var sortList = result.OrderBy(r => r.Id).ToList();

            return sortList;
        }

        private async Task<DoctorP> ConverterToDoctorPAsync(Doctor doctor, List<Cabinet> cabinets, List<Specialization> specializations, List<Area> areas)
        {
            var doctorP = new DoctorP();
            doctorP.Id = doctor.Id;
            doctorP.FIO = doctor.FIO;

            var cabinet = cabinets.Find(c => c.Id == doctor.CabinetId);
            if (cabinet != null) doctorP.Cabinet = cabinet.Number;
            else doctorP.Cabinet = "Cabinet not found!";

            var specialization = specializations.Find(s => s.Id == doctor.SpecializationId);
            if (specialization != null) doctorP.Specialization = specialization.Name;
            else doctorP.Specialization = "Specialization not found!";

            var area = areas.Find(a => a.Id == doctor.AreaId);
            if (area != null) doctorP.Area = area.Number;
            else doctorP.Area = "Area not found!";

            return doctorP;
        }
    }
}

