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
          // model.Add("patients-delete", false);
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
          model.Add("patient-doctors", loggedIn[0].GetDoctors());
          return View["patient.cshtml", model];
        }
      };
      Get["/patients/{id}/appointments/new"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Doctor> allDoctors = Doctor.GetAll();
        model.Add("patient-id", parameters.id);
        model.Add("patient-doctors", Patient.Find(parameters.id).GetDoctors());
        return View["appointment_form.cshtml", model];
      };
      Post["/patients/{id}/appointments/added"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Patient selectedPatient = Patient.Find(parameters.id);
        selectedPatient.CreateAppointment(Request.Form["date"], Doctor.Find(Request.Form["doctor"]), Request.Form["description"]);
        model.Add("patient", selectedPatient);
        model.Add("appointments", selectedPatient.GetAppointments());
        model.Add("patient-doctors", selectedPatient.GetDoctors());
        return View["patient.cshtml", model];
      };
      Get["/patients/{id}/doctors/choose"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        List<Doctor> allDoctors = Doctor.GetAll();
        model.Add("patient-id", parameters.id);
        model.Add("patient-doctors", Patient.Find(parameters.id).GetDoctors());
        model.Add("all-doctors", allDoctors);
        return View["choose_doctor_form.cshtml", model];
      };
      Post["/patients/{id}/doctors/added"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Patient selectedPatient = Patient.Find(parameters.id);
        selectedPatient.AddDoctor(Doctor.Find(Request.Form["selected-doctor"]));
        model.Add("patient", selectedPatient);
        model.Add("appointments", selectedPatient.GetAppointments());
        model.Add("patient-doctors", selectedPatient.GetDoctors());
        return View["patient.cshtml", model];
      };
      Get["/doctors/{id}/patients/delete"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Doctor selectedDoctor = Doctor.Find(parameters.id);
        model.Add("doctor", selectedDoctor);
        model.Add("appointments", selectedDoctor.GetAppointments());
        model.Add("patients", selectedDoctor.GetPatients());
        model.Add("patients-delete", true);
        return View["doctor.cshtml", model];
      };
      Delete["/doctors/{id}/patients/delete"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Doctor selectedDoctor = Doctor.Find(parameters.id);
        string patients = Request.Form["patient"];
        if(patients != null)
        {
          string[] values = patients.Split(',');
          foreach(string patientId in values)
          {
            selectedDoctor.DeletePatientRelationship(Patient.Find(int.Parse(patientId)));
          }
        }
        model.Add("doctor", selectedDoctor);
        model.Add("appointments", selectedDoctor.GetAppointments());
        model.Add("patients", selectedDoctor.GetPatients());
        return View["doctor.cshtml", model];
      };
    }
  }
}
