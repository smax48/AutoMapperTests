using System;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestAutoMapper
{
    [TestClass]
    public class TestExpressionMap
    {
        [TestMethod]
        public void TestExpressionMapWithLocalVariables()
        {
            var mapper = CreateMapper();

            //works ok with value-typed variables
            int id1 = 48;
            Expression<Func<Data1, bool>> f1 = s => s.ID == id1;

            var result = (Expression<Func<Model1, bool>>)mapper.Map(
                f1,
                typeof(Expression<Func<Data1, bool>>),
                typeof(Expression<Func<Model1, bool>>));

            Console.WriteLine(result);


            //fails with class-typed variables:
            //System.NullReferenceException: Object reference not set to an instance of an object.
            var req1 = new Request1 {ID = 48};
            Expression<Func<Data1, bool>> f2 = s => s.ID == req1.ID;

            result = (Expression<Func<Model1, bool>>)mapper.Map(
                f2,
                typeof(Expression<Func<Data1, bool>>),
                typeof(Expression<Func<Model1, bool>>));

            Console.WriteLine(result);

        }

        [TestMethod]
        public void TestExpressionMapWithMatchingPropertyNames()
        {
            var mapper = CreateMapper();

            //WORKS OK
            Expression<Func<Data1, bool>> f1 = s => s.Data2Prop.Name == "test";
            var result = (Expression<Func<Model1, bool>>) mapper.Map(
                f1,
                typeof(Expression<Func<Data1, bool>>),
                typeof(Expression<Func<Model1, bool>>));

            Console.WriteLine(result);

            //FAILS:
            //System.ArgumentException: Property 'System.DateTime CreationDate' is not defined for type 'TestAutoMapper.Model2'
            Expression<Func<Data1, bool>> f2 = s => s.Data2Prop.CreationDate > DateTime.Now;

            result = (Expression<Func<Model1, bool>>)mapper.Map(
                f2,
                typeof(Expression<Func<Data1, bool>>),
                typeof(Expression<Func<Model1, bool>>));

            Console.WriteLine(result);

        }

        private static IMapper CreateMapper()
        {
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Model2, Data2>()
                    .ForMember(d => d.CreationDate, o => o.MapFrom(m => m.ModelCreationDate))
                    .ForMember(d => d.Name, o => o.MapFrom(m => m.ModelName));

                config.CreateMap<Model1, Data1>()
                    .ForMember(d => d.Data2Prop, o => o.MapFrom(m => m.Model2Prop));
            }).CreateMapper();
            return mapper;
        }
    }

    public class Request1
    {
        public int ID { get; set; }
    }

    public class Model1
    {
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }
        public Model2 Model2Prop { get; set; }
    }

    public class Model2
    {
        public string ModelName { get; set; }
        public DateTime ModelCreationDate { get; set; }

    }
    public class Data1
    {
        public int ID { get; set; }
        public DateTime CreationDate { get; set; }

        public Data2 Data2Prop { get; set; }
    }

    public class Data2
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
