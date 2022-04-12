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
            var doctors = await GetSortDoctors<int>();

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

        [HttpGet("{pageSize}/{sortName}")]
        public async Task<ActionResult<List<List<DoctorP>>>> Get(int pageSize, DoctorsSort sortName)
        {
            List<DoctorP>? doctors = new List<DoctorP>();
            switch (sortName)
            {
                case DoctorsSort.Id:
                    doctors = await GetSortDoctors<int>(r => r.Id);
                    break;
                case DoctorsSort.FIO:
                    doctors = await GetSortDoctors<string>(r => r.FIO);
                    break;
                case DoctorsSort.Cabinet:
                    doctors = await GetSortDoctors<string>(r => r.Cabinet);
                    break;
                case DoctorsSort.Specialization:
                    doctors = await GetSortDoctors<string>(r => r.Specialization);
                    break;
                case DoctorsSort.Area:
                    doctors = await GetSortDoctors<string>(r => r.Area);
                    break;
                default:
                    doctors = await GetSortDoctors<int>(r => r.Id);
                    break;
            }

            List<DoctorP> doctorsList = new();
            List<List<DoctorP>> doctorsPages = new();
            int nowPageSize = 0;
            double doctorPagesCount = 0;
            foreach (var doctor in doctors)
            {
                if (nowPageSize < pageSize)
                {
                    doctorsList.Add(doctor);
                    nowPageSize++;
                }
                else
                {
                    nowPageSize = 0;
                    doctorsPages.Add(doctorsList);
                    doctorPagesCount++;
                    doctorsList = new();

                    doctorsList.Add(doctor);
                    nowPageSize++;
                }
            }

            if (doctorPagesCount < ((double)doctors.Count / (double)pageSize)) doctorsPages.Add(doctorsList);

            return Ok(doctorsPages);
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> AddDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var doctors = await GetSortDoctors<int>();

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

            var doctors = await GetSortDoctors<int>();

            return Ok(doctors);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Doctor>> Delete(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return BadRequest("Doctor not found!");

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            var doctors = await GetSortDoctors<int>();

            return Ok(doctors);
        }

        private async Task<List<DoctorP>> GetSortDoctors<TKey>(Func<DoctorP, TKey>? orderBy = null)
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

            List<DoctorP>? sortList;
            if (orderBy == null) sortList = result.OrderBy(r => r.Id).ToList();
            else sortList = result.OrderBy(orderBy).ToList();

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

