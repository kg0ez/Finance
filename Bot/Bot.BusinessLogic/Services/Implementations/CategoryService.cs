using System;
using AutoMapper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Common.Enums;
using Bot.Models.Data;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class CategoryService: ICategoryService
	{
        private readonly ApplicationContext _context;
        private readonly ISheetService _sheetService;
        private readonly IUserService _userService;

        public CategoryService(ApplicationContext context,
            ISheetService sheetService,
            IUserService userService)
        {
            _userService = userService;
            _context = context;
            _sheetService = sheetService;
        }
        public IMapper Mapper { get; set; }

        public void Add(Message message,OperationType type)
        {
            var categoryName = message.Text.Substring(4);

            try
            {
                var userId = _userService.Get(message.From.Username).Id;
                _sheetService.AddCellData(message, type);
                var ct = new Category { Name = categoryName, Type = type,UserId = userId };
                _context.Add(ct);
                _context.SaveChanges();
                OperationService.CategoryId = _context.Categories.FirstOrDefault(c => c.Name == categoryName)!.Id;
            }
            catch(Exception) {}
        }
        public bool IsExist(Message message, OperationType type)
        {
            var categoryName = message.Text.Substring(4);

            var categories = _context.Categories.Where(x => x.Name == categoryName && x.Type == type);
            if (categories.Count()>1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<CategoryDto> Get(int type)
        {
            IQueryable<Category> query = _context.Categories;
            query = query.Where(c => c.Type == (OperationType)type);
            var categories = query.ToList();
            var categoriesDto = Mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto!;
        }
        public List<CategoryDto> Get()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var categoriesDto = Mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto;
        }
    }
}

