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

    [Fact]
    public void Patient_CreateAppointment_CreatesAppointmentObjectForPatient()
    {
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      newPatient.CreateAppointment(new DateTime(2017, 06, 16, 15, 30, 00), newDoctor, "Yearly physical");

      List<Appointment> testList = newPatient.GetAppointments();
      List<Appointment> controlList = new List<Appointment>{new Appointment(new DateTime(2017, 06, 16, 15, 30, 00), newDoctor.Id, newPatient.Id, "Yearly physical", newPatient.GetAppointments()[0].Id)};


      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_SearchByName_ReturnsAllMatches()
    {
      Patient patient1 = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient1.Save();
      Patient patient2 = new Patient("Johnathan", "john123", "123", new DateTime(1996, 04, 25));
      patient2.Save();
      Patient patient3 = new Patient("john", "john123", "123", new DateTime(1996, 04, 25));
      patient3.Save();
      Patient patient4 = new Patient("JOHNNY", "john123", "123", new DateTime(1996, 04, 25));
      patient4.Save();
      Patient patient5 = new Patient("Samuel", "john123", "123", new DateTime(1996, 04, 25));
      patient5.Save();

      List<Patient> testList = Patient.SearchByName("john");
      List<Patient> controlList = new List<Patient>{patient1, patient2, patient3, patient4};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_Delete_DeletesSinglePatient()
    {
      Patient patient1 = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient1.Save();
      Patient patient2 = new Patient("Samuel", "john123", "123", new DateTime(1996, 04, 25));
      patient2.Save();

      patient1.DeleteSinglePatient();

      List<Patient> testList = Patient.GetAll();
      List<Patient> controlList = new List<Patient>{patient2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_Update_UpdatePatientInfo()
    {
      Patient patient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient.Save();

      patient.Update("Tom", "tom123", "123");

      Patient controlPatient = new Patient("Tom", "tom123", "123", new DateTime(1996, 04, 25), patient.Id);

      Assert.Equal(controlPatient, patient);
    }

    [Fact]
    public void Patient_Login_ReturnsTrue()
    {
      Patient patient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient.Save();

      bool testBool = patient.Login("john123", "123");

      Assert.Equal(true, testBool);
    }

    [Fact]
    public void Patient_Login_ReturnsFalse()
    {
      Patient patient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient.Save();

      bool testBool = patient.Login("tom", "567890");

      Assert.Equal(false, testBool);
    }

    [Fact]
    public void Patient_DeleteDoctors_DeletesAllOfPatientsDoctors()
    {
      Patient patient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient.Save();

      Doctor doctor1 = new Doctor("Tom", "tom567", "567", "Cardiology");
      doctor1.Save();
      Doctor doctor2 = new Doctor("John", "john567", "567", "Pediatrics");
      doctor2.Save();

      patient.AddDoctor(doctor1);
      patient.AddDoctor(doctor2);
      patient.DeleteDoctors();

      List<Doctor> testList = patient.GetDoctors();
      List<Doctor> controlList = new List<Doctor>{};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_DeleteAppointment_DeletesAllOfPatientAppointments()
    {
      Doctor doctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      doctor.Save();
      Patient patient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient.Save();

      Appointment controlAppointment1 = new Appointment(new DateTime(2017, 06, 25), doctor.Id, patient.Id, "Yearly physical");
      Appointment controlAppointment2 = new Appointment(new DateTime(2017, 05, 21), doctor.Id, patient.Id, "Yearly physical");

      patient.DeleteAppointments();
      List<Appointment> testList = patient.GetAppointments();
      List<Appointment> controlList = new List<Appointment>{};

      Assert.Equal(testList, controlList);
    }

    [Fact]
    public void Patient_DeleteAppointment_DeletesSingleAppointment()
    {
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      Appointment appointment1 = new Appointment(new DateTime(2017, 05, 21), newDoctor.Id, newPatient.Id, "Yearly physical");
      appointment1.Save();
      Appointment appointment2 = new Appointment(new DateTime(2017, 06, 21), newDoctor.Id, newPatient.Id, "Yearly physical");
      appointment2.Save();

      newPatient.DeleteSingleAppointment(appointment1);

      List<Appointment> testList = newPatient.GetAppointments();
      List<Appointment> controlList = new List<Appointment>{appointment2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_DeleteDoctorRelationship_DeletesRelationship()
    {
      Patient patient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      patient.Save();
      Doctor doctor1 = new Doctor("Tom", "tom567", "567", "Cardiology");
      doctor1.Save();
      Doctor doctor2 = new Doctor("John", "john567", "567", "Pediatrics");
      doctor2.Save();

      patient.AddDoctor(doctor1);
      patient.AddDoctor(doctor2);
      patient.DeleteDoctorRelationship(doctor1);

      List<Doctor> testList = patient.GetDoctors();
      List<Doctor> controlList = new List<Doctor>{doctor2};

      Assert.Equal(controlList, testList);
    }

    [Fact]
    public void Patient_GetMissedAppointments_ReturnsAllMissedAppointments()
    {
      DateTime now = new DateTime(2017, 06, 22);
      Doctor newDoctor = new Doctor("Tom", "tom567", "567", "Cardiology");
      newDoctor.Save();
      Patient newPatient = new Patient("John", "john123", "123", new DateTime(1996, 04, 25));
      newPatient.Save();
      newPatient.CreateAppointment(new DateTime(2017, 05, 21), newDoctor, "Yearly physical");
      newPatient.CreateAppointment(new DateTime(2017, 05, 26), newDoctor, "Heart check");
      newPatient.CreateAppointment(new DateTime(2017, 06, 25), newDoctor, "Vaccination");
      Console.WriteLine(now);
      List<Appointment> testList = newPatient.GetMissedAppointments(now);
      List<Appointment> controlList = new List<Appointment>
      { new Appointment(new DateTime(2017, 05, 21), newDoctor.Id, newPatient.Id, "Yearly physical", newPatient.GetAppointments()[0].Id),
        new Appointment(new DateTime(2017, 05, 26), newDoctor.Id, newPatient.Id, "Heart check", newPatient.GetAppointments()[1].Id)
      };

      Assert.Equal(controlList, testList);
    }

    public void Dispose()
    {
      Patient.DeleteAll();
      Appointment.DeleteAll();
      Doctor.DeleteAll();
    }
  }
}
