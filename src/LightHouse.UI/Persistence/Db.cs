using CSharpFunctionalExtensions;
using LiteDB;

namespace LightHouse.UI.Persistence
{
    public class Db
    {
        private string _databaseFileLocation = @".\Lighthouse.db";

        public Maybe<T> FindById<T>(string collection, int id) where T : IModelBase
        {
            using (var db = new LiteDatabase(_databaseFileLocation))
            {
                var col = db.GetCollection<T>(collection);
                return col.Query().Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public void Save<T>(T model, string collection) where T : IModelBase
        {
            if (FindById<T>(collection, model.Id).HasValue)
            {
                Update(model, collection);
            }
            else
            {
                Insert(model, collection);
            }
        }

        public void Update<T>(T model, string collection)
        {
            using (var db = new LiteDatabase(_databaseFileLocation))
            {
                var col = db.GetCollection<T>(collection);
                col.Update(model);
            }
        }

        public void Insert<T>(T model, string collection)
        {
            using (var db = new LiteDatabase(_databaseFileLocation))
            {
                var col = db.GetCollection<T>(collection);
                col.Insert(model);
            }
        }
    }
}