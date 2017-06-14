using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Hospital.Objects;

namespace Hospital
{
  [Collection("Hospital")]

  public class DoctorTest : IDisposable
  {
    public DoctorTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=hospital_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Doctor_GetAll_DatabaseEmptyOnload()
    {
      List<Doctor> testList = Doctor.GetAll();
      List<Doctor> controlList = new List<Doctor>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Doctor_Save_SaveToDatabase()
    {
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();

      Doctor testDoctor = Doctor.GetAll()[0];
      Assert.Equal(newDoctor, testDoctor);
    }

    [Fact]
    public void Doctor_Equals_DoctorEqualsDoctor()
    {
      Doctor controlDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      Doctor testDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");

      Assert.Equal(controlDoctor, testDoctor);
    }

    [Fact]
    public void Doctor_Find_FindsDoctorInDB()
    {
      Doctor controlDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      controlDoctor.Save();

      Doctor testDoctor = Doctor.Find(controlDoctor.Id);

      Assert.Equal(controlDoctor, testDoctor);
    }

    [Fact]
    public void Doctor_AddPatient_AssignsPatientToDoctor()
    {
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();

      newDoctor.AddPatient(newPatient);
      List<Patient> testList = newDoctor.GetPatients();
      List<Patient> controlList = new List<Patient>{newPatient};

      Assert.Equal(controlList, testList);
    }

    public void Dispose()
    {
      Doctor.DeleteAll();
    }
  }
}
