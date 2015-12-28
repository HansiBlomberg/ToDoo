using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WcfToDoService
{
    public class Global : System.Web.HttpApplication
    {


        //3) Add a global.asax file to your project by right clicking on the project name in the solution explorer,
        //    select Add, New Item...and Global Application Class.Within the Application_Start method, 
        //        add a Route to the RouteTable that specifies an optional relative path, the WebServiceHostFactory, 
        //        and the service type. Here's an example




        protected void Application_Start(object sender, EventArgs e)
        {


            RouteTable.Routes.Add(new ServiceRoute("", new WebServiceHostFactory(), typeof(ToDoService)));
        }


        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.AddHeader("Content-Type", "application/json");
                HttpContext.Current.Response.End();

                

            }

            //if (HttpContext.Current.Request.HttpMethod == "POST")
            //{
            //    HttpContext.Current.Response.AddHeader("Content-Type", "application/json");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            //    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            //    HttpContext.Current.Response.End();

            //}
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}