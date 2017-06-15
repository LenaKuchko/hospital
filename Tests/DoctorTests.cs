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

    [Fact]
    public void Doctor_SearchByName_ReturnsAllMatches()
    {
      Doctor doctor1 = new Doctor("John", "tom567", "567", "Cardiology");
      doctor1.Save();
      Doctor doctor2 = new Doctor("Johnathan", "tom567", "567", "Cardiology");
      doctor2.Save();
      Doctor doctor3 = new Doctor("john", "tom567", "567", "Cardiology");
      doctor3.Save();
      Doctor doctor4 = new Doctor("JOHNNY", "tom567", "567", "Cardiology");
      doctor4.Save();
      Doctor doctor5 = new Doctor("Samuel", "tom567", "567", "Cardiology");
      doctor5.Save();

      List<Doctor> testList = Doctor.SearchByName("john");
      List<Doctor> controlList = new List<Doctor>{doctor1, doctor2, doctor3, doctor4};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Doctor_SearchBySpecialty_ReturnsAllMatches()
    {
      Doctor doctor1 = new Doctor("John", "tom567", "567", "Physician");
      doctor1.Save();
      Doctor doctor2 = new Doctor("John", "tom567", "567", "Cardiology");
      doctor2.Save();
      Doctor doctor3 = new Doctor("John", "tom567", "567", "Pediatrician");
      doctor3.Save();
      Doctor doctor4 = new Doctor("John", "tom567", "567", "physician");
      doctor4.Save();

      List<Doctor> testList = Doctor.SearchBySpecialty("PHYSICIAN");
      List<Doctor> controlList = new List<Doctor>{doctor1, doctor4};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Doctor_Delete_DeletesSingleDoctor()
    {
      Doctor doctor1 = new Doctor("John", "tom567", "567", "Physician");
      doctor1.Save();
      Doctor doctor2 = new Doctor("John", "tom567", "567", "Cardiology");
      doctor2.Save();

      doctor1.DeleteSingleDoctor();

      List<Doctor> testList = Doctor.GetAll();
      List<Doctor> controlList = new List<Doctor>{doctor2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Doctor_Update_UpdateDoctorInfo()
    {
      Doctor doctor = new Doctor("John", "john567", "567", "Physician");
      doctor.Save();

      doctor.Update("Tom", "tom567", "123", "Cardiology");

      Doctor controlDoctor = new Doctor("Tom", "tom567", "123", "Cardiology", doctor.Id);

      Assert.Equal(controlDoctor, doctor);
    }

    [Fact]
    public void Doctor_Login_ReturnsTrue()
    {
      Doctor doctor1 = new Doctor("Tom", "tom567", "567", "Cardiology");
      doctor1.Save();
      Doctor doctor2 = new Doctor("John", "john567", "567", "Cardiology");
      doctor2.Save();

      List<Doctor> testList = Doctor.Login("john567", "567");
      List<Doctor> controlList = new List<Doctor>{doctor2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Doctor_DeletePatients_DeletesAllOfDoctorsPatients()
    {
      Doctor doctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      doctor.Save();
      Patient patient1 = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient1.Save();
      Patient patient2 = new Patient("Tom", "john123", "123", new DateTime(1996, 04, 25));
      patient2.Save();


      doctor.AddPatient(patient1);
      doctor.AddPatient(patient2);
      doctor.DeletePatients();

      List<Patient> testList = doctor.GetPatients();
      List<Patient> controlList = new List<Patient>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Doctor_DeletePatientRelationship_DeletesRelationship()
    {
      Doctor doctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      doctor.Save();
      Patient patient1 = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient1.Save();
      Patient patient2 = new Patient("Tom", "john123", "123", new DateTime(1996, 04, 25));
      patient2.Save();

      doctor.AddPatient(patient1);
      doctor.AddPatient(patient2);
      doctor.DeletePatientRelationship(patient1);

      List<Patient> testList = doctor.GetPatients();
      List<Patient> controlList = new List<Patient>{patient2};

      Assert.Equal(controlList, testList);
    }

    public void Dispose()
    {
      Doctor.DeleteAll();
    }
  }
}
