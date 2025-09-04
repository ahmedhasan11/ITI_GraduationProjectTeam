using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;

namespace Medicine_Mvc.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly ApplicationDbContext _db;

        public MedicineService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Add New Medicine
        public async Task<MedicineAddResponseDto> AddMedicineAsync(MedicineAddRequestDto request)
        {
            var medicine = new Medicine
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                ImageUrl = request.ImageUrl
            };

            _db.Medicines.Add(medicine);
            await _db.SaveChangesAsync();

            return new MedicineAddResponseDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Message = "Medicine added successfully"
            };
        }//Done

        // Get all the Medicines
        public async Task<IEnumerable<MedicineListDto>> GetAllMedicineAsync()
        {
            return await _db.Medicines
                .Select(m => new MedicineListDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Category = m.Category,
                    Price = m.Price,
                    Stock = m.Stock,
                    ImageUrl = m.ImageUrl
                })
                .ToListAsync();
        }

        // Fetch medicine details by ID
        public async Task<MedicineDetailsDto?> GetMedicineByIdAsync(int id)
        {
            Medicine medicine = await _db.Medicines.FindAsync(id);
            if (medicine == null) return null;

            return new MedicineDetailsDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Category = medicine.Category,
                Description = medicine.Description,
                Price = medicine.Price,
                Stock = medicine.Stock,
                ImageUrl = medicine.ImageUrl
            };
        }

        //Search About Medicine
        public async Task<IEnumerable<MedicineListDto>> SearchMedicineAsync(string searchTerm)
        {
            return await _db.Medicines
                .Where(m => m.Name.Contains(searchTerm) || m.Category.Contains(searchTerm))
                .Select(m => new MedicineListDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Category = m.Category,
                    Price = m.Price,
                    Stock = m.Stock,
                    ImageUrl = m.ImageUrl
                })
                .ToListAsync();
        }




        // Update Data of Medicine
        public async Task<MedicineUpdateResponseDto> UpdateMedicineAsync(MedicineUpdateRequestDto request)
        {
            var medicine = await _db.Medicines.FindAsync(request.Id);
            if (medicine == null)
            {
                return new MedicineUpdateResponseDto
                {
                    Success = false,
                    Message = "Medicine not found"
                };
            }

            medicine.Name = request.Name;
            medicine.Category = request.Category;
            medicine.Description = request.Description;
            medicine.Price = request.Price;
            medicine.Stock = request.Stock;
            medicine.ImageUrl = request.ImageUrl;

            _db.Medicines.Update(medicine);
            await _db.SaveChangesAsync();

            return new MedicineUpdateResponseDto
            {
                Success = true,
                Message = "Medicine updated successfully"
            };
        }

        // Remove Medicine
        public async Task<MedicineDeleteResponseDto> DeleteMedicineAsync(MedicineDeleteRequestDto request)
        {
            var medicine = await _db.Medicines.FindAsync(request.Id);
            if (medicine == null)
            {
                return new MedicineDeleteResponseDto
                {
                    Success = false,
                    Message = "Medicine not found"
                };
            }

            _db.Medicines.Remove(medicine);
            await _db.SaveChangesAsync();

            return new MedicineDeleteResponseDto
            {
                Success = true,
                Message = "Medicine deleted successfully"
            };
        }
    }
}
