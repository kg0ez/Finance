using System;
using AutoMapper;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common;
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

                var category = new Category { Name = categoryName, Type = type,UserId = userId };

                _context.Add(category);
                _context.SaveChanges();
                ListOfSelectedIndexes.SelectedIndexes.Add(userId, _context.Categories.FirstOrDefault(c => c.Name == categoryName)!.Id);
            }
            catch(Exception) {}
        }

        public bool IsExist(Message message, OperationType type)
        {
            if (message.Text.Length==4)
                return false;

            var categoryName = message.Text.Substring(4);

            var categories = _context.Categories.Where(x => x.Name == categoryName && x.Type == type);

            if (categories.Count()>1)
                return true;

            return false;
        }
        public List<CategoryDto> GetAllByType(int type, string userName)
        {
            var userId = _context.Users.AsNoTracking().FirstOrDefault(x => x.UserName == userName).Id;
            IQueryable<Category> query = _context.Categories
                .Where(c => c.Type == (OperationType)type && (c.UserId==userId ||c.UserId==null));

            var categories = query.ToList();

            var categoriesDto = Mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto!;
        }
        public List<CategoryDto> GetAll(string userName)
        {
            var userId = _context.Users.AsNoTracking().FirstOrDefault(x=>x.UserName == userName).Id;
            
            var categories = _context.Categories.AsNoTracking().Where(x=>x.UserId==userId || x.UserId == null).ToList();

            var categoriesDto = Mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto;
        }
    }
}

