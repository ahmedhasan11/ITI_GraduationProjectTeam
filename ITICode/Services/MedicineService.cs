using ITI_Hackathon.Data;
using ITI_Hackathon.Entities;
using ITI_Hackathon.ServiceContracts;
using ITI_Hackathon.ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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
			string imageUrl = null;


			if (request.ImageFile != null && request.ImageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/medicines");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var fileName = Guid.NewGuid() + Path.GetExtension(request.ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await request.ImageFile.CopyToAsync(stream);
				}

				imageUrl = "/images/medicines/" + fileName;
			}
			var medicine = new Medicine
            {
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                ImageUrl = imageUrl
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
                    ImageUrl = m.ImageUrl,
                    Description = m.Description
                })
                .ToListAsync();
        } //Done

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
        }//Done

        //Search About Medicine
        public async Task<IEnumerable<MedicineListDto>> SearchMedicineAsync(string searchTerm)
        {
            IEnumerable<MedicineListDto> Medicines= await _db.Medicines
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

            return Medicines;
        }

        // Update Data of Medicine
        public async Task<MedicineUpdateResponseDto> UpdateMedicineAsync(MedicineUpdateRequestDto request)
        {

            Medicine medicine = await _db.Medicines.FindAsync(request.Id);
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
			//if (request.ImageFile != null && request.ImageFile.Length > 0)
			//{
			//	/
			//	string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/medicines");
			//	if (!Directory.Exists(uploadsFolder))
			//	{
			//		Directory.CreateDirectory(uploadsFolder);
			//	}

			//	string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
			//	string filePath = Path.Combine(uploadsFolder, fileName);

			//	using (var stream = new FileStream(filePath, FileMode.Create))
			//	{
			//		await request.ImageFile.CopyToAsync(stream);
			//	}

			//	medicine.ImageUrl = "/images/medicines/" + fileName;
			//}
			//else
			//{
			//	/
			//	medicine.ImageUrl = request.ImageUrl;
			//}

			_db.Medicines.Update(medicine);
            await _db.SaveChangesAsync();

            return new MedicineUpdateResponseDto
            {
                Success = true,
                Message = "Medicine updated successfully"
            };
        } //Done

        // Remove Medicine
        public async Task<string> DeleteMedicineAsync(int id)
        {
            Medicine medicine = await _db.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return "Medicine not found";
            }

            _db.Medicines.Remove(medicine);
            await _db.SaveChangesAsync();

            return $"Medicine {medicine.Name} has been completely removed from the system.";
		}
    }
}
