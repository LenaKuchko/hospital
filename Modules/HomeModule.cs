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
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Doctor newDoctor = new Doctor(Request.Form["name"], Request.Form["username"], Request.Form["password"], Request.Form["specialty"]);
        newDoctor.Save();
        model.Add("newDoctor", newDoctor);
        return View["doctor_login.cshtml", model];
      };
      Get["/doctors/login"] = _ => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Doctor> loggedIn = Doctor.Login(Request.Query["login-username"], Request.Query["login-password"]);
        if (loggedIn.Count == 0)
        {
          model.Add("login-status", false);
          return View["doctor_login.cshtml", model];
        }
        else
        {
          model.Add("doctor", loggedIn[0]);
          model.Add("appointments", loggedIn[0].GetAppointments());
          model.Add("patients", loggedIn[0].GetPatients());
          return View["doctor.cshtml", model];
        }
      };
      Get["/patients"] = _ => {
        return View["patient_login.cshtml"];
      };
      Post["/patients/new"] = _ => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Patient newPatient = new Patient(Request.Form["name"], Request.Form["username"], Request.Form["password"], Request.Form["date"]);
        newPatient.Save();
        model.Add("newPatient", newPatient);
        return View["patient_login.cshtml", model];
      };
      Get["/patients/login"] = _ => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Patient> loggedIn = Patient.Login(Request.Query["login-username"], Request.Query["login-password"]);
        if (loggedIn.Count == 0)
        {
          model.Add("login-status", false);
          return View["patient_login.cshtml", model];
        }
        else
        {
          model.Add("patient", loggedIn[0]);
          model.Add("appointments", loggedIn[0].GetAppointments());
          model.Add("doctors", loggedIn[0].GetDoctors());
          return View["patient.cshtml", model];
        }
      };
    }
  }
}
