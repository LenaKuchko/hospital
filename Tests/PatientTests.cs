using Xunit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Hospital.Objects;

namespace Hospital
{
  [Collection("Hospital")]

  public class PatientTest : IDisposable
  {
    public PatientTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=hospital_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Patient_GetAll_DatabaseEmptyOnload()
    {
      List<Patient> testList = Patient.GetAll();
      List<Patient> controlList = new List<Patient>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_Save_SaveToDatabase()
    {
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();

      Patient testPatient = Patient.GetAll()[0];
      Assert.Equal(newPatient, testPatient);
    }

    [Fact]
    public void Patient_Equals_PatientEqualsPatient()
    {
      Patient controlPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      Patient testPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));

      Assert.Equal(controlPatient, testPatient);
    }

    [Fact]
    public void Patient_Find_FindsPatientInDB()
    {
      Patient controlPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      controlPatient.Save();

      Patient testPatient = Patient.Find(controlPatient.Id);

      Assert.Equal(controlPatient, testPatient);
    }

    [Fact]
    public void Patient_AddDoctor_AssignsDoctorToPatient()
    {
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();

      newPatient.AddDoctor(newDoctor);
      List<Doctor> testList = newPatient.GetDoctors();
      List<Doctor> controlList = new List<Doctor>{newDoctor};

      Assert.Equal(controlList, testList);
    }

    public void Dispose()
    {
      Patient.DeleteAll();
    }
  }
}
