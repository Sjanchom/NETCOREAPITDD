using System.Linq;
using HappyKids.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HappyKids.Test.Controllers
{
    class ValueControllerTest
    {

        private Microsoft.AspNetCore.Http.FormCollection GetFormCollection()
        {
            var dic = new System.Collections.Generic.Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            dic.Add("password", "test");
            dic.Add("password1", "test1");
            return new Microsoft.AspNetCore.Http.FormCollection(dic);
        }

        //private HttpRequest SetUpHttpRequest()
        //{
        //    //return new DefaultHttpRequest();
        //    //{
                
        //    //};
        //}


        //[Fact]
        //public void TestMakeRequest()
        //{
        //    var controller = new ValuesController(null, null, null, null, null, null, null);

        //    var httpContext = new Moq.Mock<HttpContext>();
        //    httpContext.Setup(x => x.Request).Returns();

        //    controller.ControllerContext.HttpContext = httpContext.Object;

        //    //var result = await controller.Save();
        //    //var viewResult = Xunit.Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        //    //Xunit.Assert.Equal("~/Views/PasswordRecovery.cshtml", viewResult.ViewName);
        //}

        [Fact]
        void InitialTest()
        {
            var _valuesController = new ValuesController(null, null, null, null, null, null, null)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        Request =
                        {
                            Method = HttpMethods.Get,
                            Host = new HostString("http://localhost:53992"),
                            Path = "api/values"
                        }
                    }
                }
            };

           var result = _valuesController.GetAll();
            Assert.Equal(2,result.Count());
        }
    }
}