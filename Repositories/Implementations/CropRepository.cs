﻿using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class CropRepository : GenericRepository<Crop>, ICropRepository
    {
        private readonly FarmContext _context;

        public CropRepository(FarmContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Crop>> GetCropsByFieldId(Guid fieldId)
        {
            return await _context.Crops
                .Include(c => c.Field)
                .Where(c => c.Field.Id == fieldId)
                .ToListAsync();
        }

        public override async Task<Crop> GetById(Guid id)
        {
            return await _context.Crops
                .Include(c => c.Field)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Crop>> GetCropsByUserId(string userId)
        {
            return await _context.Crops
                .Include(c => c.Field.Farm.User)
                .Where(c => c.Field.Farm.User.Id == userId)
                .ToListAsync();
        }
    }
}
