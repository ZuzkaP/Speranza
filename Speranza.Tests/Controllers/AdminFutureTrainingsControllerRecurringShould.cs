using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Controllers;
using Speranza.Database.Data;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Speranza.Tests.Controllers
{
    [TestClass]
   public class AdminFutureTrainingsControllerRecurringShould
    {
        private Mock<IUserManager> userManager;
        private Mock<ITrainingsManager> trainingManager;
        private Mock<IDateTimeService> dateTimeService;
        private AdminFutureTrainingsController controller;
        private RecurringModel model;
        private List<bool> checkedTrainings;
        private const string DESCRIPTION = "testDesc";
        private const int CAPACITY = 7;

        private const string TRAINER = "trainer";
        private const int WRONG_CAPACITY = -5;
        private const int DAY = 1;
        private const int TIME = 8;
        private Mock<IRecurringTemplateModel> modelB;
        private Mock<IRecurringTemplateModel> modelA;
        private Mock<ICookieService> cookieService;

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
            Assert.AreEqual("AdminFutureTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
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
            Assert.AreEqual("AdminFutureTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
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
            Assert.AreEqual("AdminFutureTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
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
            Assert.AreEqual("AdminFutureTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
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
            Assert.AreEqual("AdminFutureTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Recurring", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateRecurringTraining(model), Times.Once);
        }

        [TestMethod]
        public void SendMessageToUI_When_ItExists()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareManagerToReturnNoTemplates();
            controller.Session["Message"] = RecurringTrainingMessages.NoCapacity;

            ViewResult result =(ViewResult) controller.Recurring();

            RecurringModel model =(RecurringModel) result.Model;
            Assert.AreEqual(RecurringTrainingMessages.NoCapacity, model.Message);
            Assert.IsNull(controller.Session["Message"]);
        }

        [TestMethod]
        public void ShowClearedTable_When_NoTemplatesExist()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareManagerToReturnNoTemplates();

            ViewResult result = (ViewResult)controller.Recurring();

            RecurringModel model = (RecurringModel)result.Model;
            Assert.AreEqual(7*15, model.IsTrainingInTime.Count);
            Assert.IsFalse(model.IsTrainingInTime.Any(r=> r == true));
        }

        [TestMethod]
        public void ShowTwoItemsInTemplateTable()
        {
            InitializeAdminTrainingsRecurringController();
            PrepareManagerToReturnTwoTemplates();

            ViewResult result = (ViewResult)controller.Recurring();

            RecurringModel model = (RecurringModel)result.Model;
            Assert.AreEqual(7 * 15, model.IsTrainingInTime.Count);
            Assert.AreEqual(2,model.IsTrainingInTime.Count(r => r == true));
            Assert.AreEqual(true,model.IsTrainingInTime[9]);
            Assert.AreEqual(true,model.IsTrainingInTime[32]);

            Assert.AreEqual(modelA.Object,model.Templates[0]);
            Assert.AreEqual(modelB.Object, model.Templates[1]);
            Assert.AreEqual(2, model.Templates.Count);
        }

        [TestMethod]
        public void NotRemoveRecurringTrainingTemplate_When_UserIsNotAdmin()
        {
            InitializeAdminTrainingsRecurringController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.RemoveTemplate(DAY, TIME);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.RemoveTrainingTemplate(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RemoveRecurringTrainingTemplate()
        {
            InitializeAdminTrainingsRecurringController();

            ActionResult result = controller.RemoveTemplate(DAY, TIME);

            trainingManager.Verify(r => r.RemoveTrainingTemplate(DAY, TIME), Times.Once);
            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        private void PrepareManagerToReturnTwoTemplates()
        {
            modelA = new Mock<IRecurringTemplateModel>();
            modelB = new Mock<IRecurringTemplateModel>();
            modelA.SetupGet(r => r.Day).Returns(0);
            modelA.SetupGet(r => r.Time).Returns(15);
            modelB.SetupGet(r => r.Day).Returns(2);
            modelB.SetupGet(r => r.Time).Returns(8);
            trainingManager.Setup(r => r.GetTemplates()).Returns(new List<IRecurringTemplateModel>() {
                modelA.Object,modelB.Object
           });
        }

        private void PrepareManagerToReturnNoTemplates()
        {
            trainingManager.Setup(r => r.GetTemplates()).Returns(new List<IRecurringTemplateModel>());
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
        

       
        private void InitializeAdminTrainingsRecurringController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            cookieService = new Mock<ICookieService>();
            controller = new AdminFutureTrainingsController(userManager.Object, trainingManager.Object, dateTimeService.Object, null,cookieService.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            HttpCookieCollection cookies = new HttpCookieCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems, cookies);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);

        }

    }
}
