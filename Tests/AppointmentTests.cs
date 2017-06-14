using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Hospital.Objects;

namespace Hospital
{
  [Collection("Hospital")]

  public class AppointmentTest : IDisposable
  {
    public AppointmentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=hospital_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Appointment_GetAll_DatabaseEmptyOnload()
    {
      List<Appointment> testList = Appointment.GetAll();
      List<Appointment> controlList = new List<Appointment>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Appointment_Save_SaveToDatabase()
    {
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      Appointment newAppointment = new Appointment(new DateTime(2017, 05, 21), newDoctor.Id, newPatient.Id, "Yearly physical");
      newAppointment.Save();

      Appointment testAppointment = Appointment.GetAll()[0];
      Assert.Equal(newAppointment, testAppointment);
    }

    [Fact]
    public void Appointment_Equals_AppointmentEqualsAppointment()
    {
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      Appointment controlAppointment = new Appointment(new DateTime(2017, 05, 21), newDoctor.Id, newPatient.Id, "Yearly physical");
      Appointment testAppointment = new Appointment(new DateTime(2017, 05, 21), newDoctor.Id, newPatient.Id, "Yearly physical");

      Assert.Equal(controlAppointment, testAppointment);
    }

    [Fact]
    public void Appointment_Find_FindsAppointmentInDB()
    {
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      Appointment controlAppointment = new Appointment(new DateTime(2017, 05, 21), newDoctor.Id, newPatient.Id, "Yearly physical");
      controlAppointment.Save();

      Appointment testAppointment = Appointment.Find(controlAppointment.Id);

      Assert.Equal(controlAppointment, testAppointment);
    }

    public void Dispose()
    {
      Appointment.DeleteAll();
      Doctor.DeleteAll();
      Patient.DeleteAll();
    }
  }
}
