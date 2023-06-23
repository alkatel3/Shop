﻿using Shop.DataAccessLayer.Data;
using Shop.DataAccessLayer.Repository.IRepository;

namespace Shop.DataAccessLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;
        public ICategoryRepository Category { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            Category = new CategoryRepository(db);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}