#define NUNIT
using System;
using System.Text;
using System.Collections.Generic;
#if ! NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif
namespace Iesi.Collections.Generic.Test
{
    /// <summary>
    /// Summary description for ListSetTest
    /// </summary>
    [TestClass]
    public class ListSetTest : CommonSetTests
    {
        public ListSetTest()
        {
        }

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
        }

        protected override ISet<string> CreateInstance()
        {
            return new ListSet<string>();
        }

        protected override ISet<string> CreateInstance( ICollection<string> init )
        {
            return new ListSet<string>( init );
        }

        protected override Type ExpectedType
        {
            get { return typeof( ListSet<string> ); }
        }


        #region System.IClonable Member Tests

        [TestMethod]
        public override void Clone()
        {
            base.Clone();
        }

        [TestMethod]
        public override void CloneNonGeneric()
        {
            base.CloneNonGeneric();
        }

        #endregion

        #region System.Collections.ICollection Member Tests

        [TestMethod]
        public override void CopyTo()
        {
            base.CopyTo();
        }

        [TestMethod]
        public override void CopyToNonGeneric()
        {
            base.CopyToNonGeneric();
        }

        [TestMethod]
        public override void Count()
        {
            base.Count();
        }

        [TestMethod]
        public override void CountNonGeneric()
        {
            base.CountNonGeneric();
        }

        #endregion

        #region Iesi.Collections.ISet Constructor Tests

        [TestMethod]
        public override void CtorWithDefaults()
        {
            base.CtorWithDefaults();
        }

        [TestMethod]
        public override void CtorWithDefaultsNonGeneric()
        {
            base.CtorWithDefaultsNonGeneric();
        }

        #endregion

        #region Iesi.Collections.ISet Member Tests

        [TestMethod]
        public override void Add()
        {
            base.Add();
        }

        [TestMethod]
        public override void AddNonGeneric()
        {
            base.AddNonGeneric();
        }

        [TestMethod]
        public override void AddAll()
        {
            base.AddAll();

        }

        [TestMethod]
        public override void AddAllNonGeneric()
        {
            base.AddAllNonGeneric();
        }

        [TestMethod]
        public override void Clear()
        {
            base.Clear();
        }

        [TestMethod]
        public override void Contains()
        {
            base.Contains();
        }

        [TestMethod]
        public override void ContainsAll()
        {
            base.ContainsAll();
        }

        [TestMethod]
        public override void ContainsAllNonGeneric()
        {
            base.ContainsAllNonGeneric();
        }

        [TestMethod]
        public override void ExclusiveOr()
        {
            base.ExclusiveOr();
        }

        [TestMethod]
        public override void Intersect()
        {
            base.Intersect();
        }

        [TestMethod]
        public override void IsEmpty()
        {
            base.IsEmpty();
        }

        [TestMethod]
        public override void Minus()
        {
            base.Minus();
        }

        [TestMethod]
        public override void Remove()
        {
            base.Remove();
        }

        [TestMethod]
        public override void RemoveAll()
        {
            base.RemoveAll();
        }

        [TestMethod]
        public override void RemoveAllNonGeneric()
        {
            base.RemoveAllNonGeneric();
        }


        [TestMethod]
        public override void RetainAll()
        {
            base.RetainAll();
        }

        [TestMethod]
        public override void RetainAllNonGeneric()
        {
            base.RetainAllNonGeneric();
        }

        [TestMethod]
        public override void Union()
        {
            base.Union();
        }

        #endregion

        #region Iesi.Collection.ISet Operator Tests
        [TestMethod]
        public override void ExclusiveOrOperator()
        {
            base.ExclusiveOrOperator();

        }

        [TestMethod]
        public override void IntersectOperator()
        {
            base.IntersectOperator();
        }

        [TestMethod]
        public override void MinusOperator()
        {
            base.MinusOperator();

        }

        [TestMethod]
        public override void UnionOperator()
        {
            base.UnionOperator();
        }


        #endregion


    }
}
