using Nancy;
using System.Collections.Generic;
using System;
using Hospital.Objects;

namespace Hospital
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
         return View["index.cshtml"];
      };
      Get["/doctors"] = _ => {
        return View["doctor_login.cshtml"];
      };
      Post["/doctors/new"] = _ => {
        Doctor newDoctor = new Doctor(Request.Form["name"], Request.Form["username"], Request.Form["password"], Request.Form["specialty"]);
        newDoctor.Save();
        return View["doctor_login.cshtml", newDoctor];
      };
    }
  }
}
