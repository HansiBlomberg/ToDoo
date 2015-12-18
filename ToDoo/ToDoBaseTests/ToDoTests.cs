using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ToDoBase.Tests
{
    [TestClass()]
    public class ToDoTests
    {
        [TestMethod()]
        public void CompareToTest()
        {
            // arrange
            
            var toDoOne = new ToDo();
            var toDoTwo = new ToDo();
            var toDoThree = new ToDo();
            var toDoFour = new ToDo();

            toDoOne.CreatedDate = DateTime.Now;
            toDoOne.DeadLine = DateTime.Now;
            toDoOne.Description = "MyDescription";
            toDoOne.EstimationTime = 10;
            toDoOne.Finnished = false;
            toDoOne.Name = "Mr Test";

            toDoTwo.CreatedDate = DateTime.Now;
            toDoTwo.DeadLine = DateTime.Now;
            toDoTwo.Description = "MyDescription";
            toDoTwo.EstimationTime = 10;
            toDoTwo.Finnished = false;
            toDoTwo.Name = "Mr Test";


            toDoThree.CreatedDate = DateTime.Now;
            toDoThree.DeadLine = DateTime.Now;
            toDoThree.Description = "MyOtherDescription";
            toDoThree.EstimationTime = 10;
            toDoThree.Finnished = false;
            toDoThree.Name = "Mr Test";

            toDoFour.CreatedDate = DateTime.Now;
            toDoFour.DeadLine = DateTime.Now;
            toDoFour.Description = "MyDescription";
            toDoFour.EstimationTime = 10;
            toDoFour.Finnished = false;
            toDoFour.Name = "Mr OtherGuy";

            // act

            bool result = toDoOne == toDoTwo;
            Assert.AreEqual(true, result);

            result = toDoOne == toDoThree;
            Assert.AreEqual(false, result);

          
        }


       
    }
}