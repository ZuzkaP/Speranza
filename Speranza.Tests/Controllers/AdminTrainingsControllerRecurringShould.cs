using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Controllers;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Speranza.Tests.Controllers
{
    [TestClass]
   public class AdminTrainingsControllerRecurringShould
    {
        private Mock<IUserManager> userManager;
        private Mock<ITrainingsManager> trainingManager;
        private Mock<IDateTimeService> dateTimeService;
        private AdminTrainingsController controller;
        private RecurringModel model;
        private List<bool> checkedTrainings;
        private const string DESCRIPTION = "testDesc";
        private const int CAPACITY = 7;

        private const string TRAINER = "trainer";
        private const int WRONG_CAPACITY = -5;

        [TestMethod]
        public void ReturnToCalendar_When_UserIsNotAdmin_AndShowing()
        {
            InitializeAdminTrainingsRecurringController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.Recurring();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void ReturnToCalendar_When_UserIsNotAdmin_AndCreating()
        {
            InitializeAdminTrainingsRecurringController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.CreateRecurring(null);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(It.IsAny<IRecurringModel>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateRecurringTraining_When_TrainerIsEmpty()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareModelWithNoTrainer();

            ActionResult result = controller.CreateRecurring(model);
            
            Assert.AreEqual(RecurringTrainingMessages.NoTrainer, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Recurring", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(It.IsAny<IRecurringModel>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateRecurringTraining_When_DescriptionIsEmpty()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareModelWithNoDescription();

            ActionResult result =controller.CreateRecurring(model);

            Assert.AreEqual(RecurringTrainingMessages.NoDescription, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Recurring", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(It.IsAny<IRecurringModel>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateRecurringTraining_When_CapacityIsLessThan1()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareModelWithNoCapacity();

            ActionResult result = controller.CreateRecurring(model);

            Assert.AreEqual(RecurringTrainingMessages.NoCapacity, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Recurring", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(It.IsAny<IRecurringModel>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateRecurringTraining_When_ModelIsNull()
        {
            InitializeAdminTrainingsRecurringController();

            ActionResult result = controller.CreateRecurring(null);

            Assert.AreEqual(RecurringTrainingMessages.NoModel, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Recurring", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(It.IsAny<IRecurringModel>()), Times.Never);
        }

        [TestMethod]
        public void CreateRecurringTraining_When_AllDataAreFilledIn()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareCorrectModel();

            ActionResult result = controller.CreateRecurring(model);

            Assert.AreEqual(RecurringTrainingMessages.Success, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Recurring", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(model), Times.Once);
        }

        private void PrepareCorrectModel()
        {
            model = new RecurringModel();
            model.Capacity = CAPACITY;
            model.Description = DESCRIPTION;
            model.Trainer = TRAINER;
            PrepareArrayWithCheckedTraining();
            model.IsTrainingInTime = checkedTrainings;
        }

        private void PrepareModelWithNoCapacity()
        {
            model = new RecurringModel();
            model.Capacity = WRONG_CAPACITY;
            model.Description = DESCRIPTION;
            model.Trainer = TRAINER;
            PrepareArrayWithCheckedTraining();
            model.IsTrainingInTime = checkedTrainings;
        }

        private void PrepareModelWithNoDescription()
        {
            model = new RecurringModel();
            model.Capacity = CAPACITY;
            model.Description = string.Empty;
            model.Trainer = TRAINER;
            PrepareArrayWithCheckedTraining();
            model.IsTrainingInTime = checkedTrainings;
        }

        private void PrepareModelWithNoTrainer()
        {
            model = new RecurringModel();
            model.Capacity = CAPACITY;
            model.Description = DESCRIPTION;
            model.Trainer = string.Empty;
            PrepareArrayWithCheckedTraining();
            model.IsTrainingInTime = checkedTrainings;
        }

        private void PrepareArrayWithCheckedTraining()
        {
            checkedTrainings = new List<bool>();
            for (int i = 0; i < 7 * 13; i++)
            {
                    checkedTrainings.Add(false);
            }
            checkedTrainings[3] = true;
        }

       
        public void InitializeAdminTrainingsRecurringController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            controller = new AdminTrainingsController(userManager.Object, trainingManager.Object, dateTimeService.Object, null);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);

        }

    }
}
